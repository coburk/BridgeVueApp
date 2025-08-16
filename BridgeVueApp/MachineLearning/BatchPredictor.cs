using BridgeVueApp.Database;
using Microsoft.Data.SqlClient;
using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace BridgeVueApp.MachineLearning
{
    // Input for scoring: mirrors ModelInput minus the ExitReason label
    public sealed class PredictionInput
    {
        public int StudentID { get; set; }             // keep int (not a feature)

        public float Grade { get; set; }
        public float Age { get; set; }
        public float GenderNumeric { get; set; }
        public float EthnicityNumeric { get; set; }
        public bool SpecialEd { get; set; }
        public bool IEP { get; set; }

        public float EntryReasonNumeric { get; set; }
        public float PriorIncidents { get; set; }
        public float OfficeReferrals { get; set; }
        public float Suspensions { get; set; }
        public float Expulsions { get; set; }

        public float EntryAcademicLevelNumeric { get; set; }
        public bool CheckInOut { get; set; }
        public bool StructuredRecess { get; set; }
        public bool StructuredBreaks { get; set; }
        public float SmallGroups { get; set; }
        public float SocialWorkerVisits { get; set; }
        public float PsychologistVisits { get; set; }
        public float EntrySocialSkillsLevelNumeric { get; set; }
        public float RiskScore { get; set; }

        public float StudentStressLevelNormalized { get; set; }
        public float FamilySupportNormalized { get; set; }
        public float AcademicAbilityNormalized { get; set; }
        public float EmotionalRegulationNormalized { get; set; }

        public float AvgVerbalAggression { get; set; }
        public float AvgPhysicalAggression { get; set; }
        public float AvgAcademicEngagement { get; set; }
        public float AvgSocialInteractions { get; set; }
        public float AvgEmotionalRegulation { get; set; }
        public float AvgAggressionRisk { get; set; }
        public float AvgEngagementLevel { get; set; }

        public float RedZonePct { get; set; }
        public float YellowZonePct { get; set; }
        public float BlueZonePct { get; set; }
        public float GreenZonePct { get; set; }

        public float BehaviorDays { get; set; }

        public string ExitReason { get; set; } = ""; //Placeholder

        public float SuccessProbability { get; set; }
        public string Top3 { get; set; }
    }


    // Matches ML.NET multiclass scorer output
    public sealed class ExitPredictionRow
    {
        public string PredictedLabel { get; set; } = "";
        public float[] Score { get; set; } = Array.Empty<float>();
    }

    public static class BatchPredictor
    {
        private static readonly string ConnStr = DatabaseConfig.FullConnection;
        private static readonly string[] SuccessLabels = new[] { "Returned Successfully", "ACC" };

        public static int Run(IProgress<string>? progress = null, int? overrideModelId = null, float successThreshold = 0.50f)
        {
            var ml = new MLContext(seed: 0);

            progress?.Report("Loading current best model…");
            var (modelId, modelPath) = GetModelToUse(overrideModelId);
            if (modelId == 0 || string.IsNullOrWhiteSpace(modelPath) || !File.Exists(modelPath))
                throw new InvalidOperationException("No model available. Train a model first.");

            ITransformer model;
            DataViewSchema modelSchema;
            using (var fs = File.OpenRead(modelPath))
                model = ml.Model.Load(fs, out modelSchema);

            progress?.Report("Loading prediction inputs from vStudentMLDataRaw…");
            var rows = LoadPredictionInputs();

            if (rows.Count == 0)
            {
                progress?.Report("No rows to score.");
                return 0;
            }

            var data = ml.Data.LoadFromEnumerable(rows);
            var scored = model.Transform(data);

            // === Discover class labels from the Score column ===

            var scoreCol = scored.Schema["Score"];

            VBuffer<ReadOnlyMemory<char>> slotNames = default;
            string[] labels = Array.Empty<string>();

            // 1) Preferred: Score.SlotNames (present for most AutoML multiclass models)
            try
            {
                scoreCol.Annotations.GetValue("SlotNames", ref slotNames);
                labels = slotNames.DenseValues().Select(s => s.ToString()).ToArray();
            }
            catch
            {
                // ignore; we’ll try fallbacks
            }

            // 2) Fallback: PredictedLabel.KeyValues (category names attached to the label column)
            if (labels.Length == 0)
            {
                int predIdx = -1;
                for (int i = 0; i < scored.Schema.Count; i++)
                {
                    if (string.Equals(scored.Schema[i].Name, "PredictedLabel", StringComparison.Ordinal))
                    {
                        predIdx = i;
                        break;
                    }
                }

                if (predIdx >= 0)
                {
                    var predCol = scored.Schema[predIdx];
                    VBuffer<ReadOnlyMemory<char>> keyVals = default;
                    try
                    {
                        predCol.Annotations.GetValue("KeyValues", ref keyVals);
                        labels = keyVals.DenseValues().Select(s => s.ToString()).ToArray();
                    }
                    catch
                    {
                        // still nothing; keep falling back
                    }
                }
            }

            // 3) Last resort: infer count from first row's Score length
            if (labels.Length == 0)
            {
                var first = ml.Data.CreateEnumerable<ExitPredictionRow>(scored, reuseRowObject: false).FirstOrDefault();
                int n = first?.Score?.Length ?? 0;
                if (n <= 0)
                    throw new InvalidOperationException("Cannot determine class labels: no SlotNames/KeyValues and Score has no length.");
                labels = Enumerable.Range(0, n).Select(i => i.ToString()).ToArray(); // "0","1",...
            }

            // Flexible label handling—no hard assert on class count
            var canonical = new[] { "Returned Successfully", "ACC", "Transferred", "ABS", "Referred Out" };

            // Inform if any canonical classes are missing (but don't fail)
            var missing = canonical.Where(c => !labels.Any(l => l.Equals(c, StringComparison.OrdinalIgnoreCase))).ToArray();
            if (missing.Length > 0)
                Debug.WriteLine("Note: model was trained without these classes: " + string.Join(", ", missing));

            // Inform if any unexpected names show up
            var nonCanonical = labels.Where(l => !canonical.Any(c => c.Equals(l, StringComparison.OrdinalIgnoreCase))).ToArray();
            if (nonCanonical.Length > 0)
                Debug.WriteLine("Warning: non-canonical class names detected: " + string.Join(", ", nonCanonical));

            // Success probability sums ONLY over success labels that are actually present
            var successSet = new HashSet<string>(
                labels.Where(l =>
                    l.Equals("Returned Successfully", StringComparison.OrdinalIgnoreCase) ||
                    l.Equals("ACC", StringComparison.OrdinalIgnoreCase)),
                StringComparer.OrdinalIgnoreCase
            );





            // Update the progress
            progress?.Report($"Scoring {rows.Count} rows…");
            var preds = ml.Data.CreateEnumerable<ExitPredictionRow>(scored, reuseRowObject: false).ToList();

            // Derive success prob and Top-K (using softmax for probabilities)
            float[] Softmax(float[] z)
            {
                var max = z.Max();
                var exps = new float[z.Length];
                double sum = 0;
                for (int i = 0; i < z.Length; i++)
                {
                    var e = MathF.Exp(z[i] - max);
                    exps[i] = e;
                    sum += e;
                }
                if (sum <= 0) return Enumerable.Repeat(1f / z.Length, z.Length).ToArray();
                for (int i = 0; i < exps.Length; i++) exps[i] = (float)(exps[i] / sum);
                return exps;
            }

            //var successSet = new HashSet<string>(SuccessLabels, StringComparer.OrdinalIgnoreCase);

            var toInsert = new List<(int StudentID, string PredLabel, decimal? Top1Prob, decimal SuccessProb, string TopKJson)>(preds.Count);
            for (int i = 0; i < preds.Count; i++)
            {
                var p = preds[i];
                var probs = Softmax(p.Score);
                // map to labels
                var labeled = probs.Select((v, j) => new { Label = labels[j], Prob = v })
                                   .OrderByDescending(x => x.Prob)
                                   .ToList();

                var top1 = labeled[0];

                // Calculate success probability
                var successProb = labeled.Where(x => successSet.Contains(x.Label)).Sum(x => x.Prob);

                // Store Top-3 for transparency
                var topK = labeled.Take(3).Select(x => new { x.Label, Prob = Math.Round(x.Prob, 4) }).ToList();
                var topKJson = JsonSerializer.Serialize(topK);

                toInsert.Add((
                    StudentID: rows[i].StudentID,
                    PredLabel: top1.Label,
                    Top1Prob: (decimal?)Math.Round(top1.Prob, 5),
                    SuccessProb: (decimal)Math.Round(successProb, 5),
                    TopKJson: topKJson
                ));
            }

            progress?.Report("Writing predictions to InferenceLog…");
            BulkInsertInferenceLog(modelId, toInsert);

            progress?.Report("Batch scoring complete.");
            return toInsert.Count;
        }

        private static (int ModelID, string ModelPath) GetModelToUse(int? overrideModelId)
        {
            using var conn = new SqlConnection(ConnStr);
            conn.Open();

            if (overrideModelId.HasValue)
            {
                using var cmd = new SqlCommand($@"
                    SELECT ModelID, ModelFilePath 
                    FROM {DatabaseConfig.TableModelPerformance}
                    WHERE ModelID = @id;", conn);
                cmd.Parameters.AddWithValue("@id", overrideModelId.Value);
                using var r = cmd.ExecuteReader();
                if (r.Read()) return (r.GetInt32(0), r.IsDBNull(1) ? "" : r.GetString(1));
                return (0, "");
            }

            using (var cmd = new SqlCommand($@"
                SELECT TOP(1) ModelID, ModelFilePath
                FROM {DatabaseConfig.TableModelPerformance}
                WHERE IsCurrentBest = 1
                ORDER BY TrainingDate DESC;", conn))
            using (var r = cmd.ExecuteReader())
            {
                if (r.Read()) return (r.GetInt32(0), r.IsDBNull(1) ? "" : r.GetString(1));
            }

            // fallback: latest by date
            using (var cmd = new SqlCommand($@"
                SELECT TOP(1) ModelID, ModelFilePath
                FROM {DatabaseConfig.TableModelPerformance}
                ORDER BY TrainingDate DESC;", conn))
            using (var r = cmd.ExecuteReader())
            {
                if (r.Read()) return (r.GetInt32(0), r.IsDBNull(1) ? "" : r.GetString(1));
            }

            return (0, "");
        }

        private static List<PredictionInput> LoadPredictionInputs()
        {
            var list = new List<PredictionInput>();
            var sql = $"SELECT * FROM {DatabaseConfig.vStudentMLDataRaw} ORDER BY StudentID ASC";

            using var conn = new SqlConnection(ConnStr);
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
                list.Add(new PredictionInput
                {
                    StudentID = I("StudentID"),
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
                    ExitReason = "" // Placeholder
                });
            }

            return list;
        }

        private static void BulkInsertInferenceLog(int modelId,
            List<(int StudentID, string PredLabel, decimal? Top1Prob, decimal SuccessProb, string TopKJson)> rows)
        {
            using var conn = new SqlConnection(ConnStr);
            conn.Open();

            // Simple fast insert using a TVP pattern or individual INSERTs; here: batched INSERTs.
            using var tx = conn.BeginTransaction();
            try
            {
                const string sql = $@"
INSERT INTO {DatabaseConfig.TableInferenceLog}
(ModelID, PredictedLabel, PredictedScore, SuccessProbability, TopKJson, ActualLabel, EntityKey)
VALUES
(@m, @lbl, @score, @succ, @topk, NULL, @ek);";

                using var cmd = new SqlCommand(sql, conn, tx);
                cmd.Parameters.Add("@m", SqlDbType.Int);
                cmd.Parameters.Add("@lbl", SqlDbType.NVarChar, 50);
                cmd.Parameters.Add("@score", SqlDbType.Decimal).Precision = 6; cmd.Parameters["@score"].Scale = 5;
                cmd.Parameters.Add("@succ", SqlDbType.Decimal).Precision = 6; cmd.Parameters["@succ"].Scale = 5;
                cmd.Parameters.Add("@topk", SqlDbType.NVarChar, -1);
                cmd.Parameters.Add("@ek", SqlDbType.NVarChar, 100);

                foreach (var r in rows)
                {
                    cmd.Parameters["@m"].Value = modelId;
                    cmd.Parameters["@lbl"].Value = r.PredLabel ?? (object)DBNull.Value;
                    cmd.Parameters["@score"].Value = (object?)r.Top1Prob ?? DBNull.Value;
                    cmd.Parameters["@succ"].Value = r.SuccessProb;
                    cmd.Parameters["@topk"].Value = (object?)r.TopKJson ?? DBNull.Value;
                    cmd.Parameters["@ek"].Value = r.StudentID.ToString();
                    cmd.ExecuteNonQuery();
                }

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public sealed class LoadedModel
        {
            public MLContext Ml { get; init; }
            public ITransformer Model { get; init; }
            public string[] Labels { get; init; }
            public PredictionEngine<PredictionInput, ExitPredictionRow> Engine { get; init; }
        }

        public static LoadedModel LoadCurrentBest(IProgress<string>? progress = null, int? overrideModelId = null)
        {
            var ml = new MLContext(seed: 0);
            var (modelId, modelPath) = GetModelToUse(overrideModelId);
            if (modelId == 0 || string.IsNullOrWhiteSpace(modelPath) || !File.Exists(modelPath))
                throw new InvalidOperationException("No model available. Train a model first.");

            ITransformer model;
            using (var fs = File.OpenRead(modelPath))
                model = ml.Model.Load(fs, out _);

            // extract SlotNames once from the model by running a 1-row dummy
            var dummy = new[] { new PredictionInput { ExitReason = "" } };
            var scored = model.Transform(ml.Data.LoadFromEnumerable(dummy));

            var scoreCol = scored.Schema["Score"];
            VBuffer<ReadOnlyMemory<char>> slot = default;
            string[] labels;

            try
            {
                // Preferred: get slot names by annotation KIND name
                scoreCol.Annotations.GetValue("SlotNames", ref slot);
                labels = slot.DenseValues().Select(s => s.ToString()).ToArray();
            }
            catch
            {
                // Fallback: infer label count from a scored row
                var one = ml.Data.CreateEnumerable<ExitPredictionRow>(scored, reuseRowObject: false)
                                 .FirstOrDefault();
                var n = one?.Score?.Length ?? 0;
                labels = n > 0
                    ? Enumerable.Range(0, n).Select(i => i.ToString()).ToArray()
                    : Array.Empty<string>();
            }

        Notes:

            var engine = ml.Model.CreatePredictionEngine<PredictionInput, ExitPredictionRow>(model);
            return new LoadedModel { Ml = ml, Model = model, Labels = labels, Engine = engine };
        }

        public static (string Label, float Prob, List<(string Label, float Prob)> TopK)
            PredictOne(LoadedModel lm, PredictionInput input)
        {
            var p = lm.Engine.Predict(input);
            // softmax the Score vector to get per-class probs
            var z = p.Score ?? Array.Empty<float>();
            var max = z.Length > 0 ? z.Max() : 0f;
            var exps = z.Select(v => MathF.Exp(v - max)).ToArray();
            var sum = exps.Sum(); if (sum == 0) sum = 1;
            var probs = exps.Select(v => v / sum).ToArray();

            var labeled = probs.Select((v, i) =>
                new { Label = i < lm.Labels.Length ? lm.Labels[i] : i.ToString(), Prob = v })
                .OrderByDescending(x => x.Prob).ToList();

            var top = labeled[0];
            var topK = labeled.Take(3).Select(x => (x.Label, x.Prob)).ToList();
            return (top.Label, top.Prob, topK);
        }


    }
}


