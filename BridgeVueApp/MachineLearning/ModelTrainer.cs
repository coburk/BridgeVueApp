using BridgeVueApp.Database;
using Microsoft.Data.SqlClient;
using Microsoft.ML;
using Microsoft.ML.AutoML;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace BridgeVueApp.MachineLearning
{
    public sealed class TrainingStatus
    {
        public string Message { get; init; } = "";
        public string? CurrentTrainer { get; init; }
        public double ElapsedSeconds { get; init; }
        public double TotalSeconds { get; init; }
        public double? ValAccuracy { get; init; }       // last trial's val micro-accuracy
        public string? BestTrainer { get; init; }
        public double? BestValAccuracy { get; init; }
        public bool Done { get; init; }
    }

    public sealed class TrainingResult
    {
        public int ModelId { get; init; }
        public string Report { get; init; } = "";
    }

    // Used when enumerating scored predictions (multiclass)
    public sealed class ExitPrediction
    {
        public string PredictedLabel { get; set; } = "";
        public float[] Score { get; set; } = Array.Empty<float>();
    }

    public static class ModelTrainer
    {
        private static readonly string modelDirectory =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MachineLearning", "MLArtifacts");

        // Multiclass label
        private const string LabelColumn = "ExitReason";

        private static readonly string connectionString = DatabaseConfig.FullConnection;

        public static TrainingResult TrainAndLogModel(
            IDataView trainData,
            IDataView testData,
            IProgress<TrainingStatus>? progress = null,
            int maxExperimentSeconds = 60)
        {
            Directory.CreateDirectory(modelDirectory);

            var ml = new MLContext(seed: 0);
            var sw = Stopwatch.StartNew();
            double bestValAcc = double.NaN;
            string? bestValTrainer = null;

            progress?.Report(new TrainingStatus
            {
                Message = $"Starting AutoML (~{maxExperimentSeconds}s)…",
                ElapsedSeconds = 0,
                TotalSeconds = maxExperimentSeconds
            });

            // Per-trial progress for MULTICLASS (tracks validation MicroAccuracy)
            var trialProgress = new Progress<RunDetail<MulticlassClassificationMetrics>>(rd =>
            {
                if (rd == null) return;
                var name = rd.TrainerName ?? "Unknown";
                var micro = rd.ValidationMetrics?.MicroAccuracy;

                if (micro.HasValue && (double.IsNaN(bestValAcc) || micro.Value > bestValAcc))
                {
                    bestValAcc = micro.Value;
                    bestValTrainer = name;
                }

                progress?.Report(new TrainingStatus
                {
                    Message = $"Trying {name}  |  Val MicroAcc: {(micro?.ToString("P2") ?? "n/a")}",
                    CurrentTrainer = name,
                    ValAccuracy = micro,
                    BestTrainer = bestValTrainer,
                    BestValAccuracy = double.IsNaN(bestValAcc) ? null : bestValAcc,
                    ElapsedSeconds = sw.Elapsed.TotalSeconds,
                    TotalSeconds = maxExperimentSeconds
                });
            });

            // Run AutoML (MULTICLASS)
            int secs = maxExperimentSeconds <= 0 ? 60 : maxExperimentSeconds;
            uint expSeconds = (uint)secs;

            var exp = ml.Auto().CreateMulticlassClassificationExperiment(maxExperimentTimeInSeconds: expSeconds);
            var result = exp.Execute(trainData, labelColumnName: LabelColumn, progressHandler: trialProgress);

            // Evaluate on TEST
            progress?.Report(new TrainingStatus
            {
                Message = "Evaluating on test set…",
                ElapsedSeconds = sw.Elapsed.TotalSeconds,
                TotalSeconds = maxExperimentSeconds
            });

            var best = result.BestRun;
            var scoredTest = best.Model.Transform(testData);

            var mc = ml.MulticlassClassification.Evaluate(
                scoredTest,
                labelColumnName: LabelColumn,
                scoreColumnName: "Score",
                predictedLabelColumnName: "PredictedLabel");

            // Save artifact (sanitize filename)
            string algo = best.TrainerName ?? "UnknownTrainer";
            string safe = string.Concat(algo.Select(c => Path.GetInvalidFileNameChars().Contains(c) ? '_' : c));
            string ts = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            string modelPath = Path.Combine(modelDirectory, $"Model_{safe}_{ts}.zip");

            progress?.Report(new TrainingStatus
            {
                Message = "Saving model artifact…",
                ElapsedSeconds = sw.Elapsed.TotalSeconds,
                TotalSeconds = maxExperimentSeconds
            });

            ml.Model.Save(best.Model, trainData.Schema, modelPath);

            // Log to DB
            progress?.Report(new TrainingStatus
            {
                Message = "Logging metrics to database…",
                ElapsedSeconds = sw.Elapsed.TotalSeconds,
                TotalSeconds = maxExperimentSeconds
            });

            int modelId;
            using (var conn = new SqlConnection(DatabaseConfig.FullConnection))
            {
                conn.Open();
                using var tx = conn.BeginTransaction();

                // reset previous best
                using (var reset = new SqlCommand(
                    $"UPDATE {DatabaseConfig.TableModelPerformance} SET IsCurrentBest=0 WHERE IsCurrentBest=1", conn, tx))
                    reset.ExecuteNonQuery();

                // insert performance (map Micro→Accuracy, Macro→F1; others NULL)
                using (var cmd = new SqlCommand($@"
INSERT INTO {DatabaseConfig.TableModelPerformance}
(TrainingDate, ModelName, ModelType, Hyperparameters, TrainingDurationSec,
 Accuracy, F1Score, AUC, Precision, Recall,
 TrainingDataSize, TestDataSize, IsCurrentBest, ModelFilePath)
OUTPUT INSERTED.ModelID
VALUES
(@td,@name,'ML.NET MulticlassClassification (AutoML)',@hyper,@dur,
 @acc,@f1,@auc,@prec,@rec,
 @tr,@te,1,@path);", conn, tx))
                {
                    cmd.Parameters.AddWithValue("@td", DateTime.UtcNow);
                    cmd.Parameters.AddWithValue("@name", algo);
                    cmd.Parameters.AddWithValue("@hyper", JsonSerializer.Serialize(new { TrainerName = algo, AutoML = "MulticlassClassification", Seed = 0 }));
                    cmd.Parameters.AddWithValue("@dur", (int)sw.Elapsed.TotalSeconds);

                    cmd.Parameters.AddWithValue("@acc", (decimal)mc.MicroAccuracy);
                    cmd.Parameters.AddWithValue("@f1", (decimal)mc.MacroAccuracy);
                    cmd.Parameters.Add(new SqlParameter("@auc", SqlDbType.Decimal) { Value = DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@prec", SqlDbType.Decimal) { Value = DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@rec", SqlDbType.Decimal) { Value = DBNull.Value });

                    cmd.Parameters.AddWithValue("@tr", (int)(trainData.GetRowCount() ?? 0));
                    cmd.Parameters.AddWithValue("@te", (int)(testData.GetRowCount() ?? 0));
                    cmd.Parameters.AddWithValue("@path", modelPath);

                    modelId = (int)cmd.ExecuteScalar();
                }

                // metrics history (same mapping; NULL the binary-only slots)
                using (var hist = new SqlCommand($@"
INSERT INTO {DatabaseConfig.TableModelMetricsHistory}
(ModelID, Accuracy, F1Score, AUC, Precision, Recall)
VALUES (@m,@a,@f,@u,@p,@r);", conn, tx))
                {
                    hist.Parameters.AddWithValue("@m", modelId);
                    hist.Parameters.AddWithValue("@a", (decimal)mc.MicroAccuracy);
                    hist.Parameters.AddWithValue("@f", (decimal)mc.MacroAccuracy);
                    hist.Parameters.Add(new SqlParameter("@u", SqlDbType.Decimal) { Value = DBNull.Value });
                    hist.Parameters.Add(new SqlParameter("@p", SqlDbType.Decimal) { Value = DBNull.Value });
                    hist.Parameters.Add(new SqlParameter("@r", SqlDbType.Decimal) { Value = DBNull.Value });
                    hist.ExecuteNonQuery();
                }

                tx.Commit();
            }

            sw.Stop();

            var report =
$@"Model Training Summary
-----------------------
Model ID:        {modelId}
Trainer:         {algo}
Duration:        {sw.Elapsed.TotalSeconds:N0} sec
Train Rows:      {(int)(trainData.GetRowCount() ?? 0)}
Test Rows:       {(int)(testData.GetRowCount() ?? 0)}
Saved To:        {modelPath}

Test Metrics (Multiclass)
  MicroAcc:      {mc.MicroAccuracy:P2}
  MacroAcc:      {mc.MacroAccuracy:P2}
  LogLoss:       {mc.LogLoss:N3}";

            progress?.Report(new TrainingStatus
            {
                Message = $"✅ Training complete. MicroAcc {mc.MicroAccuracy:P2}, MacroAcc {mc.MacroAccuracy:P2}",
                BestTrainer = bestValTrainer,
                BestValAccuracy = double.IsNaN(bestValAcc) ? null : bestValAcc,
                ElapsedSeconds = sw.Elapsed.TotalSeconds,
                TotalSeconds = maxExperimentSeconds,
                Done = true
            });

            return new TrainingResult { ModelId = modelId, Report = report };
        }

        /// <summary>
        /// Resets previous best and inserts this run as current best in one transaction.
        /// If you also have a MetricsHistory table, append a row here too.
        /// </summary>
        private static int SaveModelPerformanceTransactional(ModelPerformance perf)
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();
            using var tx = conn.BeginTransaction();

            try
            {
                // 1) Clear current best
                using (var reset = new SqlCommand(
                    $"UPDATE {DatabaseConfig.TableModelPerformance} SET IsCurrentBest = 0 WHERE IsCurrentBest = 1",
                    conn, tx))
                {
                    reset.ExecuteNonQuery();
                }

                // 2) Insert this run
                string insertSql = $@"
                INSERT INTO {DatabaseConfig.TableModelPerformance}
                (
                    TrainingDate, ModelName, ModelType, Hyperparameters, TrainingDurationSec,
                    Accuracy, F1Score, AUC, Precision, Recall,
                    TrainingDataSize, TestDataSize, IsCurrentBest, ModelFilePath
                )
                OUTPUT INSERTED.ModelID
                VALUES
                (
                    @TrainingDate, @ModelName, @ModelType, @Hyperparameters, @TrainingDurationSec,
                    @Accuracy, @F1Score, @AUC, @Precision, @Recall,
                    @TrainingDataSize, @TestDataSize, @IsCurrentBest, @ModelFilePath
                );";

                int modelId;
                using (var cmd = new SqlCommand(insertSql, conn, tx))
                {
                    cmd.Parameters.AddWithValue("@TrainingDate", perf.TrainingDate);
                    cmd.Parameters.AddWithValue("@ModelName", (object?)perf.ModelName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ModelType", (object?)perf.ModelType ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Hyperparameters", (object?)perf.Hyperparameters ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@TrainingDurationSec", (object?)perf.TrainingDurationSec ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Accuracy", (object?)perf.Accuracy ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@F1Score", (object?)perf.F1Score ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AUC", (object?)perf.AUC ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Precision", (object?)perf.Precision ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Recall", (object?)perf.Recall ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@TrainingDataSize", perf.TrainingDataSize);
                    cmd.Parameters.AddWithValue("@TestDataSize", perf.TestDataSize);
                    cmd.Parameters.AddWithValue("@IsCurrentBest", perf.IsCurrentBest);
                    cmd.Parameters.AddWithValue("@ModelFilePath", (object?)perf.ModelFilePath ?? DBNull.Value);

                    modelId = (int)cmd.ExecuteScalar();
                }

                // 3) Optional: append a metrics-history row
                string histSql = $@"
                INSERT INTO {DatabaseConfig.TableModelMetricsHistory}
                    (ModelID, Accuracy, F1Score, AUC, Precision, Recall)
                VALUES
                    (@ModelID, @Accuracy, @F1Score, @AUC, @Precision, @Recall);";

                using (var hist = new SqlCommand(histSql, conn, tx))
                {
                    hist.Parameters.AddWithValue("@ModelID", modelId);
                    hist.Parameters.AddWithValue("@Accuracy", (object?)perf.Accuracy ?? DBNull.Value);
                    hist.Parameters.AddWithValue("@F1Score", (object?)perf.F1Score ?? DBNull.Value);
                    hist.Parameters.AddWithValue("@AUC", (object?)perf.AUC ?? DBNull.Value);
                    hist.Parameters.AddWithValue("@Precision", (object?)perf.Precision ?? DBNull.Value);
                    hist.Parameters.AddWithValue("@Recall", (object?)perf.Recall ?? DBNull.Value);
                    hist.ExecuteNonQuery();
                }

                tx.Commit();
                return modelId;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public static List<ModelInput> LoadTrainingDataFromDatabase()
        {
            var data = new List<ModelInput>();
            const string sql = "SELECT * FROM dbo.vStudentMLTrainingData ORDER BY StudentID ASC";

            using var conn = new SqlConnection(connectionString);
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            int Ord(string name) => reader.GetOrdinal(name);
            bool Has(string name)
            {
                try { return reader.GetOrdinal(name) >= 0; } catch { return false; }
            }
            float F(string col) => reader.IsDBNull(Ord(col)) ? 0f : Convert.ToSingle(reader[col]);
            int I(string col) => reader.IsDBNull(Ord(col)) ? 0 : reader.GetInt32(Ord(col));
            bool B(string col) => !reader.IsDBNull(Ord(col)) && reader.GetBoolean(Ord(col));

            while (reader.Read())
            {
                data.Add(new ModelInput
                {
                    Grade = I("Grade"),
                    Age = I("Age"),
                    GenderNumeric = I("GenderNumeric"),
                    EthnicityNumeric = I("EthnicityNumeric"),
                    SpecialEd = B("SpecialEd"),
                    IEP = B("IEP"),
                    EntryReasonNumeric = I("EntryReasonNumeric"),
                    PriorIncidents = I("PriorIncidents"),
                    OfficeReferrals = I("OfficeReferrals"),
                    Suspensions = I("Suspensions"),
                    Expulsions = I("Expulsions"),
                    EntryAcademicLevelNumeric = I("EntryAcademicLevelNumeric"),
                    CheckInOut = B("CheckInOut"),
                    StructuredRecess = B("StructuredRecess"),
                    StructuredBreaks = B("StructuredBreaks"),
                    SmallGroups = I("SmallGroups"),
                    SocialWorkerVisits = I("SocialWorkerVisits"),
                    PsychologistVisits = I("PsychologistVisits"),
                    EntrySocialSkillsLevelNumeric = I("EntrySocialSkillsLevelNumeric"),
                    RiskScore = F("RiskScore"),
                    StudentStressLevelNormalized = F("StudentStressLevelNormalized"),
                    FamilySupportNormalized = F("FamilySupportNormalized"),
                    AcademicAbilityNormalized = F("AcademicAbilityNormalized"),
                    EmotionalRegulationNormalized = F("EmotionalRegulationNormalized"),
                    AvgVerbalAggression = Has("AvgVerbalAggression") ? F("AvgVerbalAggression") : 0f,
                    AvgPhysicalAggression = Has("AvgPhysicalAggression") ? F("AvgPhysicalAggression") : 0f,
                    AvgAcademicEngagement = Has("AvgAcademicEngagement") ? F("AvgAcademicEngagement") : 0f,
                    AvgSocialInteractions = Has("AvgSocialInteractions") ? F("AvgSocialInteractions") : 0f,
                    AvgEmotionalRegulation = Has("AvgEmotionalRegulation") ? F("AvgEmotionalRegulation") : 0f,
                    AvgAggressionRisk = Has("AvgAggressionRisk") ? F("AvgAggressionRisk") : 0f,
                    AvgEngagementLevel = Has("AvgEngagementLevel") ? F("AvgEngagementLevel") : 0f,
                    RedZonePct = Has("RedZonePct") ? F("RedZonePct") : 0f,
                    YellowZonePct = Has("YellowZonePct") ? F("YellowZonePct") : 0f,
                    BlueZonePct = Has("BlueZonePct") ? F("BlueZonePct") : 0f,
                    GreenZonePct = Has("GreenZonePct") ? F("GreenZonePct") : 0f,
                    BehaviorDays = Has("BehaviorDays") ? I("BehaviorDays") : 0,

                    // LABEL for multiclass
                    ExitReason = reader.IsDBNull(Ord("ExitReason")) ? "" : reader.GetString(Ord("ExitReason"))

                    // IMPORTANT: Do NOT include DidSucceed as a feature when predicting ExitReason
                });
            }

            return data;
        }

        private static int InsertDatasetSnapshot(
            SqlConnection conn, SqlTransaction tx,
            string sourceView, string selectionQuery, int rowCount,
            int? minRecordId = null, int? maxRecordId = null,
            string? featureSchemaHash = null, string? dataHash = null, string? notes = null)
        {
            const string sql = @"
            INSERT INTO dbo.DatasetSnapshot
             (SourceView, SelectionQuery, RowCount, MinRecordID, MaxRecordID, FeatureSchemaHash, DataContentHash, Notes)
            OUTPUT INSERTED.SnapshotID
            VALUES
             (@src, @sel, @cnt, @min, @max, @fhash, @dhash, @notes);";

            using var cmd = new SqlCommand(sql, conn, tx);
            cmd.Parameters.AddWithValue("@src", sourceView);
            cmd.Parameters.AddWithValue("@sel", selectionQuery);
            cmd.Parameters.AddWithValue("@cnt", rowCount);
            cmd.Parameters.AddWithValue("@min", (object?)minRecordId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@max", (object?)maxRecordId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@fhash", (object?)featureSchemaHash ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@dhash", (object?)dataHash ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@notes", (object?)notes ?? DBNull.Value);
            return (int)cmd.ExecuteScalar();
        }

        private static void LinkModelToSnapshot(SqlConnection conn, SqlTransaction tx, int modelId, int snapshotId, string role)
        {
            const string sql = @"INSERT INTO dbo.ModelDataUsage (ModelID, SnapshotID, Role) VALUES (@m, @s, @r);";
            using var cmd = new SqlCommand(sql, conn, tx);
            cmd.Parameters.AddWithValue("@m", modelId);
            cmd.Parameters.AddWithValue("@s", snapshotId);
            cmd.Parameters.AddWithValue("@r", role);
            cmd.ExecuteNonQuery();
        }

        private static void InsertMetricsHistory(SqlConnection conn, SqlTransaction tx, int modelId,
            decimal accuracy, decimal f1, decimal auc, decimal precision, decimal recall)
        {
            string sql = $@"
            INSERT INTO {DatabaseConfig.TableModelMetricsHistory}
             (ModelID, Accuracy, F1Score, AUC, Precision, Recall)
            VALUES (@mid, @acc, @f1, @auc, @prec, @rec);";

            using var cmd = new SqlCommand(sql, conn, tx);
            cmd.Parameters.AddWithValue("@mid", modelId);
            cmd.Parameters.AddWithValue("@acc", (object?)accuracy ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@f1", (object?)f1 ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@auc", (object?)auc ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@prec", (object?)precision ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@rec", (object?)recall ?? DBNull.Value);
            cmd.ExecuteNonQuery();
        }

        // Hash of ordered (Name|Type|Role) to catch schema/feature order drift
        private static string ComputeSchemaHash(DataViewSchema schema, string labelColumn)
        {
            var sb = new StringBuilder();
            foreach (var col in schema)
            {
                var role = string.Equals(col.Name, labelColumn, StringComparison.OrdinalIgnoreCase) ? "Label" : "Feature";
                sb.Append(col.Name).Append('|')
                  .Append(col.Type.RawType.FullName).Append('|')
                  .Append(role).Append(';');
            }
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
            return Convert.ToHexString(bytes); // uppercase hex
        }

        public static class ModelTracking
        {
            private static readonly string Conn = DatabaseConfig.FullConnection;
            private const string Label = "ExitReason";
            private static readonly string ArtifactDir = Path.Combine("MachineLearning", "MLArtifacts");

            public static int BackfillTrainingRunFromCurrentData()
            {
                Directory.CreateDirectory(ArtifactDir);
                var ml = new MLContext(seed: 0);

                // Load training records (guaranteed label via the view)
                var rows = ModelTrainer.LoadTrainingDataFromDatabase();
                var data = ml.Data.LoadFromEnumerable(rows);

                // Deterministic split so lineage is meaningful
                var split = ml.Data.TrainTestSplit(data, testFraction: 0.2, seed: 0);

                // AutoML quick experiment (multiclass)
                var exp = ml.Auto().CreateMulticlassClassificationExperiment(maxExperimentTimeInSeconds: 60);
                var result = exp.Execute(split.TrainSet, labelColumnName: Label);

                var best = result.BestRun.Model;
                var scored = best.Transform(split.TestSet);
                var test = ml.MulticlassClassification.Evaluate(scored, labelColumnName: Label, scoreColumnName: "Score");

                // Save artifact
                var algo = result.BestRun.TrainerName ?? "AutoML";
                var stamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
                var modelPath = Path.Combine(ArtifactDir, $"Model_{algo}_{stamp}.zip");
                ml.Model.Save(best, split.TrainSet.Schema, modelPath);

                // Insert everything in one transaction
                using var conn = new SqlConnection(Conn);
                conn.Open();
                using var tx = conn.BeginTransaction();

                try
                {
                    // reset current best
                    new SqlCommand($"UPDATE {DatabaseConfig.TableModelPerformance} SET IsCurrentBest=0 WHERE IsCurrentBest=1", conn, tx)
                        .ExecuteNonQuery();

                    // ModelPerformance
                    var insertPerf = $@"
                    INSERT INTO {DatabaseConfig.TableModelPerformance}
                    (TrainingDate, ModelName, ModelType, Hyperparameters, TrainingDurationSec,
                        Accuracy, F1Score, AUC, Precision, Recall, TrainingDataSize, TestDataSize, IsCurrentBest, ModelFilePath)
                    OUTPUT INSERTED.ModelID
                    VALUES (@td,@name,@type,@hyper,@dur,@acc,@f1,@auc,@prec,@rec,@tr,@te,1,@path);";

                    using var perfCmd = new SqlCommand(insertPerf, conn, tx);
                    perfCmd.Parameters.AddWithValue("@td", DateTime.UtcNow);
                    perfCmd.Parameters.AddWithValue("@name", algo);
                    perfCmd.Parameters.AddWithValue("@type", "ML.NET MulticlassClassification (AutoML)");
                    perfCmd.Parameters.AddWithValue("@hyper", JsonSerializer.Serialize(new { TrainerName = algo, Seed = 0 }));
                    perfCmd.Parameters.AddWithValue("@dur", 60);
                    perfCmd.Parameters.AddWithValue("@acc", (decimal)test.MicroAccuracy);
                    perfCmd.Parameters.AddWithValue("@f1", (decimal)test.MacroAccuracy);
                    perfCmd.Parameters.Add(new SqlParameter("@auc", SqlDbType.Decimal) { Value = DBNull.Value });
                    perfCmd.Parameters.Add(new SqlParameter("@prec", SqlDbType.Decimal) { Value = DBNull.Value });
                    perfCmd.Parameters.Add(new SqlParameter("@rec", SqlDbType.Decimal) { Value = DBNull.Value });
                    perfCmd.Parameters.AddWithValue("@tr", (int)(split.TrainSet.GetRowCount() ?? 0));
                    perfCmd.Parameters.AddWithValue("@te", (int)(split.TestSet.GetRowCount() ?? 0));
                    perfCmd.Parameters.AddWithValue("@path", modelPath);
                    int modelId = (int)perfCmd.ExecuteScalar();

                    // Metrics history (append one point from the test eval)
                    using (var hist = new SqlCommand($@"
                        INSERT INTO {DatabaseConfig.TableModelMetricsHistory}
                        (ModelID, Accuracy, F1Score, AUC, Precision, Recall) VALUES (@m,@a,@f,@u,@p,@r);", conn, tx))
                    {
                        hist.Parameters.AddWithValue("@m", modelId);
                        hist.Parameters.AddWithValue("@a", (decimal)test.MicroAccuracy);
                        hist.Parameters.AddWithValue("@f", (decimal)test.MacroAccuracy);
                        hist.Parameters.Add(new SqlParameter("@u", SqlDbType.Decimal) { Value = DBNull.Value });
                        hist.Parameters.Add(new SqlParameter("@p", SqlDbType.Decimal) { Value = DBNull.Value });
                        hist.Parameters.Add(new SqlParameter("@r", SqlDbType.Decimal) { Value = DBNull.Value });
                        hist.ExecuteNonQuery();
                    }

                    // Dataset snapshots (simple descriptors; enhance later with hashes)
                    int trainSnap = InsertSnapshot(conn, tx, "dbo.vStudentMLTrainingData", "(AutoML split TRAIN 80%)", (int)(split.TrainSet.GetRowCount() ?? 0));
                    int testSnap = InsertSnapshot(conn, tx, "dbo.vStudentMLTrainingData", "(AutoML split TEST 20%)", (int)(split.TestSet.GetRowCount() ?? 0));

                    // Link usage
                    using (var link = new SqlCommand($@"INSERT INTO {DatabaseConfig.TableModelDataUsage}(ModelID,SnapshotID,Role) VALUES (@m,@s,'TRAIN'),(@m,@t,'TEST');", conn, tx))
                    {
                        link.Parameters.AddWithValue("@m", modelId);
                        link.Parameters.AddWithValue("@s", trainSnap);
                        link.Parameters.AddWithValue("@t", testSnap);
                        link.ExecuteNonQuery();
                    }

                    tx.Commit();
                    return modelId;
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }

            private static int InsertSnapshot(SqlConnection conn, SqlTransaction tx, string source, string sel, int count)
            {
                using var cmd = new SqlCommand($@"
                INSERT INTO {DatabaseConfig.TableDatasetSnapshot}
                (SourceView, SelectionQuery, RowCount)
                OUTPUT INSERTED.SnapshotID
                VALUES (@src,@sel,@cnt);", conn, tx);
                cmd.Parameters.AddWithValue("@src", source);
                cmd.Parameters.AddWithValue("@sel", sel);
                cmd.Parameters.AddWithValue("@cnt", count);
                return (int)cmd.ExecuteScalar();
            }
        }
    }
}
