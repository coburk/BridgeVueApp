//using BridgeVueApp.Database;
//using Microsoft.Data.SqlClient;
//using Microsoft.ML;
//using Microsoft.ML.Data;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using System.Text.Json;

//namespace BridgeVueApp.MachineLearning
//{
//    public sealed class FeatureImportanceRow
//    {
//        public string Feature { get; set; } = "";
//        public double Importance { get; set; }            // BaselineAcc - PermutedAcc
//        public double BaselineAccuracy { get; set; }
//        public double PermutedAccuracy { get; set; }
//    }

//    internal sealed class FIPredRow
//    {
//        public string PredictedLabel { get; set; } = "";
//        public float[] Score { get; set; } = Array.Empty<float>();
//    }

//    internal sealed class FIInput
//    {
//        // label
//        public string ExitReason { get; set; } = "";

//        // features (mirror vStudentMLTrainingData)
//        public int Grade { get; set; }
//        public int Age { get; set; }
//        public int GenderNumeric { get; set; }
//        public int EthnicityNumeric { get; set; }
//        public bool SpecialEd { get; set; }
//        public bool IEP { get; set; }
//        public int EntryReasonNumeric { get; set; }
//        public int PriorIncidents { get; set; }
//        public int OfficeReferrals { get; set; }
//        public int Suspensions { get; set; }
//        public int Expulsions { get; set; }
//        public int EntryAcademicLevelNumeric { get; set; }
//        public bool CheckInOut { get; set; }
//        public bool StructuredRecess { get; set; }
//        public bool StructuredBreaks { get; set; }
//        public int SmallGroups { get; set; }
//        public int SocialWorkerVisits { get; set; }
//        public int PsychologistVisits { get; set; }
//        public int EntrySocialSkillsLevelNumeric { get; set; }
//        public float RiskScore { get; set; }
//        public float StudentStressLevelNormalized { get; set; }
//        public float FamilySupportNormalized { get; set; }
//        public float AcademicAbilityNormalized { get; set; }
//        public float EmotionalRegulationNormalized { get; set; }
//        public float AvgVerbalAggression { get; set; }
//        public float AvgPhysicalAggression { get; set; }
//        public float AvgAcademicEngagement { get; set; }
//        public float AvgSocialInteractions { get; set; }
//        public float AvgEmotionalRegulation { get; set; }
//        public float AvgAggressionRisk { get; set; }
//        public float AvgEngagementLevel { get; set; }
//        public float RedZonePct { get; set; }
//        public float YellowZonePct { get; set; }
//        public float BlueZonePct { get; set; }
//        public float GreenZonePct { get; set; }
//        public int BehaviorDays { get; set; }
//    }

//    public static class FeatureImportance
//    {
//        private static readonly string ArtifactsDir =
//            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MachineLearning", "MLArtifacts");

//        // The features we’ll permute (order shown in output)
//        private static readonly string[] FeatureColumns =
//        {
//            "Grade","Age","GenderNumeric","EthnicityNumeric","SpecialEd","IEP",
//            "EntryReasonNumeric","PriorIncidents","OfficeReferrals","Suspensions","Expulsions",
//            "EntryAcademicLevelNumeric","CheckInOut","StructuredRecess","StructuredBreaks",
//            "SmallGroups","SocialWorkerVisits","PsychologistVisits","EntrySocialSkillsLevelNumeric",
//            "RiskScore","StudentStressLevelNormalized","FamilySupportNormalized",
//            "AcademicAbilityNormalized","EmotionalRegulationNormalized",
//            "AvgVerbalAggression","AvgPhysicalAggression","AvgAcademicEngagement",
//            "AvgSocialInteractions","AvgEmotionalRegulation","AvgAggressionRisk","AvgEngagementLevel",
//            "RedZonePct","YellowZonePct","BlueZonePct","GreenZonePct","BehaviorDays"
//        };

//        public static List<FeatureImportanceRow> ComputeAndSave(
//            int? overrideModelId = null,
//            int maxRows = 3000,
//            int seed = 0,
//            IProgress<string>? progress = null)
//        {
//            Directory.CreateDirectory(ArtifactsDir);
//            var ml = new MLContext(seed: seed);

//            // 1) Load current best model
//            var (modelId, modelPath) = GetModelToUse(overrideModelId);
//            if (modelId == 0 || string.IsNullOrWhiteSpace(modelPath) || !File.Exists(modelPath))
//                throw new InvalidOperationException("No current best model found. Train a model first.");

//            progress?.Report($"Using model #{modelId}: {Path.GetFileName(modelPath)}");
//            ITransformer model;
//            DataViewSchema modelSchema;
//            using (var fs = File.OpenRead(modelPath))
//                model = ml.Model.Load(fs, out modelSchema);

//            // 2) Load labeled data from training view
//            var rows = LoadRows(maxRows);
//            if (rows.Count == 0)
//                throw new InvalidOperationException("No data in vStudentMLTrainingData.");

//            // 3) Baseline accuracy (Top-1 against ExitReason)
//            var baseline = EvaluateTop1Acc(ml, model, rows, out var classLabels);
//            progress?.Report($"Baseline Top-1 accuracy: {baseline:P2}");

//            // 4) For each feature, shuffle that column and re-evaluate
//            var rnd = new Random(seed);
//            var props = typeof(FIInput).GetProperties(BindingFlags.Public | BindingFlags.Instance)
//                                       .ToDictionary(p => p.Name, p => p, StringComparer.Ordinal);

//            var results = new List<FeatureImportanceRow>(FeatureColumns.Length);

//            for (int i = 0; i < FeatureColumns.Length; i++)
//            {
//                var col = FeatureColumns[i];
//                if (!props.TryGetValue(col, out var pi)) continue; // skip unknown

//                progress?.Report($"Permuting feature {i + 1}/{FeatureColumns.Length}: {col}…");

//                var permuted = PermuteColumn(rows, pi, rnd);
//                var permAcc = EvaluateTop1Acc(ml, model, permuted, out _); // labels unchanged
//                results.Add(new FeatureImportanceRow
//                {
//                    Feature = col,
//                    BaselineAccuracy = baseline,
//                    PermutedAccuracy = permAcc,
//                    Importance = Math.Max(0.0, baseline - permAcc)
//                });
//            }

//            // 5) Save CSV
//            var stamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
//            var csvPath = Path.Combine(ArtifactsDir, $"feature_importance_{modelId}_{stamp}.csv");
//            SaveCsv(csvPath, results.OrderByDescending(r => r.Importance).ToList());
//            progress?.Report($"Saved: {csvPath}");

//            // Optional: save a JSON with the class labels used
//            File.WriteAllText(Path.Combine(ArtifactsDir, $"class_labels_{modelId}_{stamp}.json"),
//                JsonSerializer.Serialize(classLabels));

//            return results.OrderByDescending(r => r.Importance).ToList();
//        }

//        // --- helpers ---

//        private static (int ModelID, string ModelPath) GetModelToUse(int? overrideModelId)
//        {
//            using var conn = new SqlConnection(DatabaseConfig.FullConnection);
//            conn.Open();

//            if (overrideModelId.HasValue)
//            {
//                using var cmd = new SqlCommand($@"
//                    SELECT ModelID, ModelFilePath 
//                    FROM {DatabaseConfig.TableModelPerformance}
//                    WHERE ModelID = @id;", conn);
//                cmd.Parameters.AddWithValue("@id", overrideModelId.Value);
//                using var r = cmd.ExecuteReader();
//                if (r.Read()) return (r.GetInt32(0), r.IsDBNull(1) ? "" : r.GetString(1));
//                return (0, "");
//            }

//            using (var cmd = new SqlCommand($@"
//                SELECT TOP(1) ModelID, ModelFilePath
//                FROM {DatabaseConfig.TableModelPerformance}
//                WHERE IsCurrentBest = 1
//                ORDER BY TrainingDate DESC;", conn))
//            using (var r = cmd.ExecuteReader())
//            {
//                if (r.Read()) return (r.GetInt32(0), r.IsDBNull(1) ? "" : r.GetString(1));
//            }

//            return (0, "");
//        }

//        private static List<FIInput> LoadRows(int maxRows)
//        {
//            var list = new List<FIInput>(Math.Max(512, maxRows));
//            var sql = $"SELECT TOP {maxRows} * FROM {DatabaseConfig.vStudentMLTrainingData} ORDER BY StudentID ASC";

//            using var conn = new SqlConnection(DatabaseConfig.FullConnection);
//            conn.Open();
//            using var cmd = new SqlCommand(sql, conn);
//            using var reader = cmd.ExecuteReader();

//            int Ord(string name) => reader.GetOrdinal(name);
//            bool Has(string name)
//            {
//                try { return reader.GetOrdinal(name) >= 0; } catch { return false; }
//            }
//            float F(string col) => reader.IsDBNull(Ord(col)) ? 0f : Convert.ToSingle(reader[col]);
//            int I(string col) => reader.IsDBNull(Ord(col)) ? 0 : reader.GetInt32(Ord(col));
//            bool B(string col) => !reader.IsDBNull(Ord(col)) && reader.GetBoolean(Ord(col));
//            string S(string col) => reader.IsDBNull(Ord(col)) ? "" : reader.GetString(Ord(col));

//            while (reader.Read())
//            {
//                list.Add(new FIInput
//                {
//                    ExitReason = Has("ExitReason") ? S("ExitReason") : "",

//                    Grade = I("Grade"),
//                    Age = I("Age"),
//                    GenderNumeric = I("GenderNumeric"),
//                    EthnicityNumeric = I("EthnicityNumeric"),
//                    SpecialEd = B("SpecialEd"),
//                    IEP = B("IEP"),
//                    EntryReasonNumeric = I("EntryReasonNumeric"),
//                    PriorIncidents = I("PriorIncidents"),
//                    OfficeReferrals = I("OfficeReferrals"),
//                    Suspensions = I("Suspensions"),
//                    Expulsions = I("Expulsions"),
//                    EntryAcademicLevelNumeric = I("EntryAcademicLevelNumeric"),
//                    CheckInOut = B("CheckInOut"),
//                    StructuredRecess = B("StructuredRecess"),
//                    StructuredBreaks = B("StructuredBreaks"),
//                    SmallGroups = I("SmallGroups"),
//                    SocialWorkerVisits = I("SocialWorkerVisits"),
//                    PsychologistVisits = I("PsychologistVisits"),
//                    EntrySocialSkillsLevelNumeric = I("EntrySocialSkillsLevelNumeric"),
//                    RiskScore = F("RiskScore"),
//                    StudentStressLevelNormalized = F("StudentStressLevelNormalized"),
//                    FamilySupportNormalized = F("FamilySupportNormalized"),
//                    AcademicAbilityNormalized = F("AcademicAbilityNormalized"),
//                    EmotionalRegulationNormalized = F("EmotionalRegulationNormalized"),
//                    AvgVerbalAggression = Has("AvgVerbalAggression") ? F("AvgVerbalAggression") : 0f,
//                    AvgPhysicalAggression = Has("AvgPhysicalAggression") ? F("AvgPhysicalAggression") : 0f,
//                    AvgAcademicEngagement = Has("AvgAcademicEngagement") ? F("AvgAcademicEngagement") : 0f,
//                    AvgSocialInteractions = Has("AvgSocialInteractions") ? F("AvgSocialInteractions") : 0f,
//                    AvgEmotionalRegulation = Has("AvgEmotionalRegulation") ? F("AvgEmotionalRegulation") : 0f,
//                    AvgAggressionRisk = Has("AvgAggressionRisk") ? F("AvgAggressionRisk") : 0f,
//                    AvgEngagementLevel = Has("AvgEngagementLevel") ? F("AvgEngagementLevel") : 0f,
//                    RedZonePct = Has("RedZonePct") ? F("RedZonePct") : 0f,
//                    YellowZonePct = Has("YellowZonePct") ? F("YellowZonePct") : 0f,
//                    BlueZonePct = Has("BlueZonePct") ? F("BlueZonePct") : 0f,
//                    GreenZonePct = Has("GreenZonePct") ? F("GreenZonePct") : 0f,
//                    BehaviorDays = Has("BehaviorDays") ? I("BehaviorDays") : 0
//                });
//            }

//            return list;
//        }

//        private static List<FIInput> PermuteColumn(List<FIInput> src, PropertyInfo pi, Random rnd)
//        {
//            // copy values
//            var vals = src.Select(row => pi.GetValue(row)).ToArray();

//            // shuffle indices
//            int n = vals.Length;
//            var idx = Enumerable.Range(0, n).ToArray();
//            for (int i = n - 1; i > 0; i--)
//            {
//                int j = rnd.Next(i + 1);
//                (idx[i], idx[j]) = (idx[j], idx[i]);
//            }

//            // build permuted copy
//            var dst = new List<FIInput>(n);
//            for (int i = 0; i < n; i++)
//            {
//                var r = src[i]; // shallow copy: create a new object and copy all props
//                var copy = new FIInput
//                {
//                    ExitReason = r.ExitReason,
//                    Grade = r.Grade,
//                    Age = r.Age,
//                    GenderNumeric = r.GenderNumeric,
//                    EthnicityNumeric = r.EthnicityNumeric,
//                    SpecialEd = r.SpecialEd,
//                    IEP = r.IEP,
//                    EntryReasonNumeric = r.EntryReasonNumeric,
//                    PriorIncidents = r.PriorIncidents,
//                    OfficeReferrals = r.OfficeReferrals,
//                    Suspensions = r.Suspensions,
//                    Expulsions = r.Expulsions,
//                    EntryAcademicLevelNumeric = r.EntryAcademicLevelNumeric,
//                    CheckInOut = r.CheckInOut,
//                    StructuredRecess = r.StructuredRecess,
//                    StructuredBreaks = r.StructuredBreaks,
//                    SmallGroups = r.SmallGroups,
//                    SocialWorkerVisits = r.SocialWorkerVisits,
//                    PsychologistVisits = r.PsychologistVisits,
//                    EntrySocialSkillsLevelNumeric = r.EntrySocialSkillsLevelNumeric,
//                    RiskScore = r.RiskScore,
//                    StudentStressLevelNormalized = r.StudentStressLevelNormalized,
//                    FamilySupportNormalized = r.FamilySupportNormalized,
//                    AcademicAbilityNormalized = r.AcademicAbilityNormalized,
//                    EmotionalRegulationNormalized = r.EmotionalRegulationNormalized,
//                    AvgVerbalAggression = r.AvgVerbalAggression,
//                    AvgPhysicalAggression = r.AvgPhysicalAggression,
//                    AvgAcademicEngagement = r.AvgAcademicEngagement,
//                    AvgSocialInteractions = r.AvgSocialInteractions,
//                    AvgEmotionalRegulation = r.AvgEmotionalRegulation,
//                    AvgAggressionRisk = r.AvgAggressionRisk,
//                    AvgEngagementLevel = r.AvgEngagementLevel,
//                    RedZonePct = r.RedZonePct,
//                    YellowZonePct = r.YellowZonePct,
//                    BlueZonePct = r.BlueZonePct,
//                    GreenZonePct = r.GreenZonePct,
//                    BehaviorDays = r.BehaviorDays
//                };

//                // set the permuted value for the chosen feature
//                pi.SetValue(copy, vals[idx[i]]);
//                dst.Add(copy);
//            }

//            return dst;
//        }

//        private static double EvaluateTop1Acc(MLContext ml, ITransformer model, List<FIInput> rows, out string[] labels)
//        {
//            var data = ml.Data.LoadFromEnumerable(rows);
//            var scored = model.Transform(data);

//            // Try to read class labels from Score.SlotNames
//            labels = GetClassLabelsFromScore(scored);

//            // Predict and compare with true label
//            var preds = ml.Data.CreateEnumerable<FIPredRow>(scored, reuseRowObject: false).ToArray();

//            int correct = 0;
//            int n = preds.Length;
//            for (int i = 0; i < n; i++)
//            {
//                var p = preds[i];
//                if (p.Score == null || p.Score.Length == 0) continue;

//                int topIdx = 0;
//                float max = p.Score[0];
//                for (int j = 1; j < p.Score.Length; j++)
//                    if (p.Score[j] > max) { max = p.Score[j]; topIdx = j; }

//                string predicted = (labels.Length == p.Score.Length) ? labels[topIdx] : p.PredictedLabel;
//                if (string.Equals(predicted, rows[i].ExitReason, StringComparison.OrdinalIgnoreCase))
//                    correct++;
//            }

//            return n > 0 ? (double)correct / n : 0.0;
//        }

//        private static string[] GetClassLabelsFromScore(IDataView scored)
//        {
//            if (!scored.Schema.TryGetColumnIndex("Score", out int scoreIdx))
//                return Array.Empty<string>();

//            var scoreCol = scored.Schema[scoreIdx];
//            var annSchema = scoreCol.Annotations.Schema;

//            // Find the annotation named "SlotNames"
//            int slotIdx = -1;
//            for (int i = 0; i < annSchema.Count; i++)
//            {
//                if (string.Equals(annSchema[i].Name, "SlotNames", StringComparison.Ordinal))
//                {
//                    slotIdx = i;
//                    break;
//                }
//            }

//            if (slotIdx < 0) return Array.Empty<string>();

//            VBuffer<ReadOnlyMemory<char>> slotNames = default;
//            scoreCol.Annotations.GetValue(slotIdx, ref slotNames);
//            return slotNames.DenseValues().Select(s => s.ToString()).ToArray();
//        }

//        private static void SaveCsv(string path, List<FeatureImportanceRow> rows)
//        {
//            using var sw = new StreamWriter(path);
//            sw.WriteLine("Feature,Importance,BaselineAccuracy,PermutedAccuracy");
//            foreach (var r in rows)
//                sw.WriteLine($"{r.Feature},{r.Importance:F6},{r.BaselineAccuracy:F6},{r.PermutedAccuracy:F6}");
//        }
//    }
//}
