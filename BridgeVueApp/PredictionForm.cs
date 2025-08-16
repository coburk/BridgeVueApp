using BridgeVueApp.Data;
using BridgeVueApp.Database;
using BridgeVueApp.MachineLearning;
using Microsoft.Data.SqlClient;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using static BridgeVueApp.MainForm;



namespace BridgeVueApp
{
    public partial class PredictionForm : Form
    {

        public PredictionForm()
        {
            InitializeComponent();
            SetupBatchSummary();


            pbTrain.Style = ProgressBarStyle.Continuous;
            pbTrain.MarqueeAnimationSpeed = 0; 
            pbTrain.Minimum = 0;
            pbTrain.Maximum = 100;
            pbTrain.Value = 0;
            pbTrain.Visible = false;

            lblTrainStatus.Visible = false;
            lblTrainStatus.Text = "";


        }

        private readonly BindingList<BatchProgressRow> _batchProgress = new BindingList<BatchProgressRow>();

        private sealed class BatchProgressRow
        {
            public DateTime Time { get; set; }
            public string Stage { get; set; } = "";
            public string Message { get; set; } = "";
        }

        private void btnPredictSatic_Click_1(object sender, EventArgs e)
        {
            var input = new ML_Class_Success.ModelInput()
            {
                Grade = 0F,
                Age = 5F,
                GenderNumeric = 1F,
                EthnicityNumeric = 5F,
                SpecialEd = false,
                IEP = false,
                EntryReasonNumeric = 3F,
                PriorIncidents = 4F,
                OfficeReferrals = 2F,
                Suspensions = 1F,
                Expulsions = 0F,
                EntryAcademicLevelNumeric = 2F,
                CheckInOut = true,
                StructuredRecess = true,
                StructuredBreaks = true,
                SmallGroups = 1F,
                SocialWorkerVisits = 2F,
                PsychologistVisits = 0F,
                EntrySocialSkillsLevelNumeric = 2F,
                RiskScore = 5F,
                StudentStressLevelNormalized = 0.72886014F,
                FamilySupportNormalized = 0.39298666F,
                AcademicAbilityNormalized = 0.3105175F,
                EmotionalRegulationNormalized = 0.32346544F,
                AvgVerbalAggression = 0F,
                AvgPhysicalAggression = 0F,
                AvgAcademicEngagement = 2F,
                AvgSocialInteractions = 1F,
                AvgEmotionalRegulation = 1F,
                AvgAggressionRisk = 0.56103545F,
                AvgEngagementLevel = 0.48540565F,
                RedZonePct = 0.33783785F,
                YellowZonePct = 0.4054054F,
                BlueZonePct = 0.25675675F,
                GreenZonePct = 0F,
                BehaviorDays = 148F,
            };


            var result = ML_Class_Success.Predict(input);

            rtbRandomPredictionOutput.Text = $"Predicted Exit Outcome: {MLLookups.Lookup(MLLookups.ExitReason, Convert.ToInt32(result.PredictedLabel))}";

            if (result == null)
            {
                rtbRandomPredictionOutput.Text = "No student data found.";
            }
        }







        // Predict a random student from the database
        private void btnRandomStudentPredict_Click(object sender, EventArgs e)
        {
            PredictRandomStudent();

        }


        // Method to predict a random student from the database
        private void PredictRandomStudent()
        {
            // Load the same model Batch uses
            var lm = BridgeVueApp.MachineLearning.BatchPredictor.LoadCurrentBest();

            // Use the same feature view Batch uses
            string connString = "Server=localhost;Database=BridgeVue;Integrated Security=True;TrustServerCertificate=True;";
            string sql = "SELECT TOP 1 * FROM vStudentMLDataRaw ORDER BY NEWID()";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        rtbRandomPredictionOutput.Text = "No student data found.";
                        return;
                    }

                    // Helpers for safe reads
                    int Ord(string name) => reader.GetOrdinal(name);
                    bool Has(string name) { try { return reader.GetOrdinal(name) >= 0; } catch { return false; } }
                    float F(string col) => reader.IsDBNull(Ord(col)) ? 0f : Convert.ToSingle(reader[col]);
                    int I(string col) => reader.IsDBNull(Ord(col)) ? 0 : reader.GetInt32(Ord(col));
                    bool B(string col) => !reader.IsDBNull(Ord(col)) && reader.GetBoolean(Ord(col));

                    // Build the SAME PredictionInput schema Batch uses (floats/bools + ExitReason placeholder)
                    var input = new BridgeVueApp.MachineLearning.PredictionInput // if your class is named PredictionInput, use that
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

                        // required by the saved pipeline even for prediction
                        ExitReason = ""
                    };

                    // Predict using the SAME engine + labels as Batch
                    var pred = BridgeVueApp.MachineLearning.BatchPredictor.PredictOne(lm, input);
                    string predictedExitReason = pred.Label;
                    float confidence = pred.Prob;

                    // Log to inference table (use current-best ID)
                    int modelId = GetCurrentBestModelId();
                    PredictionLogger.LogPrediction(
                        modelId,
                        input.StudentID.ToString(),
                        predictedExitReason,
                        confidence
                    );

                    // ---------- UI rendering (unchanged except: use predictedExitReason/confidence directly) ----------
                    rtbRandomPredictionOutput.Clear();
                    rtbRandomPredictionOutput.Font = new Font("Consolas", 10);

                    rtbRandomPredictionOutput.SelectionAlignment = HorizontalAlignment.Center;
                    rtbRandomPredictionOutput.SelectionFont = new Font("Consolas", 10, FontStyle.Bold);
                    rtbRandomPredictionOutput.SelectionColor = Color.Blue;
                    rtbRandomPredictionOutput.AppendText("EXIT REASON PREDICTION\n");
                    rtbRandomPredictionOutput.AppendText("============================\n\n");

                    rtbRandomPredictionOutput.SelectionFont = new Font("Consolas", 12, FontStyle.Bold);
                    rtbRandomPredictionOutput.SelectionColor =
                        predictedExitReason == "Returned Successfully" || predictedExitReason == "ACC"
                        ? Color.Green : Color.Orange;
                    rtbRandomPredictionOutput.AppendText($"{predictedExitReason}\n[Confidence: {confidence:P1}]\n\n");

                    rtbRandomPredictionOutput.SelectionFont = new Font("Consolas", 10, FontStyle.Bold);
                    rtbRandomPredictionOutput.SelectionColor = Color.Blue;
                    rtbRandomPredictionOutput.AppendText("------------------------------\n");
                    rtbRandomPredictionOutput.AppendText("   STUDENT SUMMARY\n");
                    rtbRandomPredictionOutput.AppendText("------------------------------\n\n");

                    rtbRandomPredictionOutput.SelectionAlignment = HorizontalAlignment.Left;
                    string leftPadding = new string(' ', 6);

                    string[] leftLabels = {
                "Grade:", "Age:", "Gender:", "Ethnicity:", "Special Ed:", "IEP:",
                "Entry Reason:", "Academic Level:", "Social Skills:", "Risk Score:"
            };

                    string[] leftValues = {
                input.Grade.ToString("0"),
                input.Age.ToString("0"),
                MLLookups.Lookup(MLLookups.Gender, Convert.ToInt32(input.GenderNumeric)),
                MLLookups.Lookup(MLLookups.Ethnicity, Convert.ToInt32(input.EthnicityNumeric)),
                input.SpecialEd ? "Yes" : "No",
                input.IEP ? "Yes" : "No",
                MLLookups.Lookup(MLLookups.EntryReason, Convert.ToInt32(input.EntryReasonNumeric)),
                MLLookups.Lookup(MLLookups.EntryAcademicLevel, Convert.ToInt32(input.EntryAcademicLevelNumeric)),
                MLLookups.Lookup(MLLookups.EntrySocialSkillsLevel, Convert.ToInt32(input.EntrySocialSkillsLevelNumeric)),
                input.RiskScore.ToString("F2")
            };

                    string[] rightLabels = {
                "Prior Incidents:", "Office Referrals:", "Suspensions:", "Expulsions:",
                "Avg Verbal Agg:", "Avg Physical Agg:", "Avg Engagement:", "Red Zone %:",
                "Green Zone %:", "Behavior Days:"
            };

                    string[] rightValues = {
                input.PriorIncidents.ToString("0"),
                input.OfficeReferrals.ToString("0"),
                input.Suspensions.ToString("0"),
                input.Expulsions.ToString("0"),
                input.AvgVerbalAggression.ToString("F2"),
                input.AvgPhysicalAggression.ToString("F2"),
                input.AvgAcademicEngagement.ToString("F2"),
                input.RedZonePct.ToString("P1"),
                input.GreenZonePct.ToString("P1"),
                input.BehaviorDays.ToString("0")
            };

                    int maxRows = Math.Max(leftLabels.Length, rightLabels.Length);
                    for (int i = 0; i < maxRows; i++)
                    {
                        string leftLabel = i < leftLabels.Length ? leftLabels[i].PadRight(18) : "".PadRight(18);
                        string leftValue = i < leftValues.Length ? leftValues[i] : "";
                        string rightLabel = i < rightLabels.Length ? rightLabels[i].PadRight(18) : "".PadRight(18);
                        string rightValue = i < rightValues.Length ? rightValues[i] : "";

                        rtbRandomPredictionOutput.SelectionColor = Color.Black;
                        rtbRandomPredictionOutput.SelectionFont = new Font("Consolas", 10, FontStyle.Regular);
                        rtbRandomPredictionOutput.AppendText(leftPadding + leftLabel);
                        rtbRandomPredictionOutput.SelectionColor = Color.Black;
                        rtbRandomPredictionOutput.AppendText(leftValue.PadRight(15));
                        rtbRandomPredictionOutput.SelectionColor = Color.Black;
                        rtbRandomPredictionOutput.AppendText(rightLabel);

                        if (i >= 4)
                        {
                            if (i == 4) rtbRandomPredictionOutput.SelectionColor = input.AvgVerbalAggression < 0.5f ? Color.Green : Color.Red;
                            else if (i == 5) rtbRandomPredictionOutput.SelectionColor = input.AvgPhysicalAggression < 0.5f ? Color.Green : Color.Red;
                            else if (i == 6) rtbRandomPredictionOutput.SelectionColor = input.AvgAcademicEngagement >= 3.0f ? Color.Green : Color.Red;
                            else if (i == 7) rtbRandomPredictionOutput.SelectionColor = input.RedZonePct < 0.25f ? Color.Orange : Color.Red;
                            else if (i == 8) rtbRandomPredictionOutput.SelectionColor = input.GreenZonePct > 0.5f ? Color.Green : Color.Orange;
                            else rtbRandomPredictionOutput.SelectionColor = Color.Black;

                            rtbRandomPredictionOutput.SelectionFont = new Font("Consolas", 10, FontStyle.Bold);
                        }
                        else
                        {
                            rtbRandomPredictionOutput.SelectionColor = Color.Black;
                            rtbRandomPredictionOutput.SelectionFont = new Font("Consolas", 10, FontStyle.Regular);
                        }

                        rtbRandomPredictionOutput.AppendText(rightValue + "\n");
                    }
                }
            }
        }







        private void tabBatch_Click(object sender, EventArgs e)
        {

        }

        private void PredictionForm_Load(object sender, EventArgs e)
        {

        }





        // Handle batch prediction button click
        private async void btnBatchPredict_Click(object sender, EventArgs e)
        {
            try
            {
                btnBatchPredict.Enabled = false;
                Cursor = Cursors.WaitCursor;

                // show progress in the grid as we go
                _batchProgress.Clear();
                dgvBatchPrediction.DataSource = _batchProgress;
                dgvBatchPrediction.AutoGenerateColumns = true;

                AppendBatchProgress("Batch", "Starting batch predictions…");

                var prog = new Progress<string>(msg => AppendBatchProgress("Batch", msg));
                int wrote = await Task.Run(() => BatchPredictor.Run(prog));

                AppendBatchProgress("Batch", $"Scoring finished. {wrote} inference rows written.");
                AppendBatchProgress("Batch", "Loading latest predictions per student…");

                int modelId = GetCurrentBestModelId();
                var results = LoadLatestBatchResults(modelId);

                AppendBatchProgress("Batch", $"Loaded {results.Count} latest predictions. Rendering…");

                dgvBatchPrediction.DataSource = new BindingList<BatchPredictionResult>(results);
                dgvBatchPrediction.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                dgvBatchPrediction.Refresh();

                AppendBatchProgress("Batch", "Done.");
            }
            catch (Exception ex)
            {
                // keep in-grid reporting; no message boxes
                AppendBatchProgress("Error", ex.Message);
            }
            finally
            {
                Cursor = Cursors.Default;
                btnBatchPredict.Enabled = true;
            }
        }


        private List<BatchPredictionResult> LoadLatestBatchResults(int modelId)
        {
            string sql = $@"
            WITH Latest AS (
                SELECT il.*, 
                        TRY_CONVERT(int, il.EntityKey) AS StudentIDInt,
                        ROW_NUMBER() OVER (
                            PARTITION BY il.EntityKey 
                            ORDER BY il.CreatedAtUtc DESC, il.InferenceID DESC
                        ) rn
                FROM {DatabaseConfig.TableInferenceLog} il
                WHERE il.ModelID = @mid
            )
            SELECT 
                L.StudentIDInt AS StudentID,
                L.PredictedLabel,
                L.PredictedScore,
                L.SuccessProbability,
                L.TopKJson,
                sp.GenderNumeric,
                sp.EthnicityNumeric,
                sp.Grade,
                r.RiskScore,
                r.AvgVerbalAggression,
                r.AvgPhysicalAggression,
                r.AvgAcademicEngagement,
                r.RedZonePct,
                r.GreenZonePct,
                r.BehaviorDays
            FROM Latest L
            LEFT JOIN {DatabaseConfig.TableStudentProfile} sp ON sp.StudentID = L.StudentIDInt
            LEFT JOIN {DatabaseConfig.vStudentMLDataRaw} r    ON r.StudentID  = L.StudentIDInt
            WHERE L.rn = 1
            ORDER BY L.StudentIDInt ASC;";

            var list = new List<BatchPredictionResult>();
            using var conn = new SqlConnection(DatabaseConfig.FullConnection);
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@mid", modelId);
            using var rd = cmd.ExecuteReader();

            int Ord(string n) => rd.GetOrdinal(n);
            float F(string c) => rd.IsDBNull(Ord(c)) ? 0f : Convert.ToSingle(rd[c]);
            int I(string c) => rd.IsDBNull(Ord(c)) ? 0 : rd.GetInt32(Ord(c));
            string S(string c) => rd.IsDBNull(Ord(c)) ? "" : rd.GetString(Ord(c));
            decimal? DN(string c) => rd.IsDBNull(Ord(c)) ? (decimal?)null : rd.GetDecimal(Ord(c));

            while (rd.Read())
            {
                var studentId = I("StudentID");
                var label = S("PredictedLabel");
                var top1 = DN("PredictedScore");
                var succ = DN("SuccessProbability") ?? 0m;

                list.Add(new BatchPredictionResult
                {
                    StudentID = studentId,
                    PredictedOutcome = string.IsNullOrWhiteSpace(label) ? "Unknown" : label,
                    Confidence = (top1.HasValue ? ((float)top1.Value).ToString("P1") : ""),
                    // If you added these properties to BatchPredictionResult, uncomment:
                    // SuccessProbability = (float)succ,
                    // Top3 = FormatTop3(S("TopKJson")),

                    Gender = MLLookups.Lookup(MLLookups.Gender, I("GenderNumeric")),
                    Ethnicity = MLLookups.Lookup(MLLookups.Ethnicity, I("EthnicityNumeric")),
                    Grade = I("Grade"),
                    RiskScore = F("RiskScore"),
                    AvgVerbalAggression = F("AvgVerbalAggression"),
                    AvgPhysicalAggression = F("AvgPhysicalAggression"),
                    AvgAcademicEngagement = F("AvgAcademicEngagement"),
                    RedZonePct = F("RedZonePct"),
                    GreenZonePct = F("GreenZonePct")
                });
            }

            return list;
        }

        private static string FormatTop3(string topKJson)
        {
            if (string.IsNullOrWhiteSpace(topKJson)) return "";
            try
            {
                var items = JsonSerializer.Deserialize<List<TopKItem>>(topKJson);
                if (items == null || items.Count == 0) return "";
                return string.Join("  |  ", items.Select(i => $"{i.Label} {i.Prob:P0}"));
            }
            catch { return ""; }
        }

        private sealed class TopKItem
        {
            public string Label { get; set; } = "";
            public double Prob { get; set; }
        }



        // Aend a progress message to the batch progress DataGridView
        private void AppendBatchProgress(string stage, string message)
        {
            _batchProgress.Add(new BatchProgressRow
            {
                Time = DateTime.UtcNow,
                Stage = stage,
                Message = message
            });

            try
            {
                var last = _batchProgress.Count - 1;
                if (last >= 0)
                {
                    dgvBatchPrediction.FirstDisplayedScrollingRowIndex = last;
                    dgvBatchPrediction.InvalidateRow(last);
                }
            }
            catch { /* ignore scroll errors */ }
        }



        // Handle row pre-paint to color rows based on predicted exit reason
        private void DgvBatchPrediction_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex < 0) return; // header
            var grid = (DataGridView)sender;
            var row = grid.Rows[e.RowIndex];
            if (row.IsNewRow) return;

            // Try the expected column name, then sensible fallbacks
            string outcome = null;
            if (grid.Columns.Contains("PredictedOutcome"))
                outcome = row.Cells["PredictedOutcome"].Value?.ToString();
            else if (grid.Columns.Contains("PredictedLabel"))
                outcome = row.Cells["PredictedLabel"].Value?.ToString();
            else if (grid.Columns.Contains("ExitReason"))
                outcome = row.Cells["ExitReason"].Value?.ToString();

            if (string.IsNullOrWhiteSpace(outcome)) return;

            switch (outcome)
            {
                case "Returned Successfully":
                    row.DefaultCellStyle.BackColor = Color.FromArgb(220, 255, 220);
                    row.DefaultCellStyle.ForeColor = Color.DarkGreen;
                    break;
                case "ACC":
                    row.DefaultCellStyle.BackColor = Color.FromArgb(220, 240, 255);
                    row.DefaultCellStyle.ForeColor = Color.DarkBlue;
                    break;
                case "Transferred":
                    row.DefaultCellStyle.BackColor = Color.FromArgb(240, 248, 255);
                    row.DefaultCellStyle.ForeColor = Color.DarkSlateBlue;
                    break;
                case "Referred Out":
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 240, 220);
                    row.DefaultCellStyle.ForeColor = Color.DarkOrange;
                    break;
                case "ABS":
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 220, 220);
                    row.DefaultCellStyle.ForeColor = Color.DarkRed;
                    break;
                default:
                    row.DefaultCellStyle.BackColor = Color.White;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                    break;
            }
        }


        // Setup the batch summary text and DataGridView styles
        private void SetupBatchSummary()
        {
            lblBatchSummary.Text =
                "The table below displays predicted exit reasons for all current students in the BridgeVue program.\n\n" +
                "• Predicted Outcome: The model's suggested exit reason (Graduated, Transferred, etc.).\n" +
                "• Confidence: How certain the model is in its prediction.\n" +
                "• Behavior Metrics: Risk scores, aggression levels, engagement, and zone percentages.\n\n" +
                "These predictions are supportive tools for staff and should be combined with professional judgment.";

            dgvBatchPrediction.RowPrePaint += DgvBatchPrediction_RowPrePaint;
        }


        private static (int idx, float val) Top1(float[] scores)
        {
            if (scores == null || scores.Length == 0) return (-1, 0f);
            int idx = 0;
            float max = scores[0];
            for (int i = 1; i < scores.Length; i++)
                if (scores[i] > max) { max = scores[i]; idx = i; }
            return (idx, max);
        }



        public class BatchPredictionResult
        {
            public int StudentID { get; set; }
            public string PredictedOutcome { get; set; } // Now contains text like "Graduated"
            public string Confidence { get; set; }
            public string Gender { get; set; } // Added - converted from numeric
            public string Ethnicity { get; set; } // Added - converted from numeric
            public int Grade { get; set; } // Added
            public float RiskScore { get; set; } // Added
            public float AvgVerbalAggression { get; set; }
            public float AvgPhysicalAggression { get; set; }
            public float AvgAcademicEngagement { get; set; }
            public float RedZonePct { get; set; }
            public float GreenZonePct { get; set; } // Added
        }

        private async void btnTrain_Click(object sender, EventArgs e)
        {
            // Disable the button to prevent multiple clicks
            btnTrain.Enabled = false;

            // Show progress bar and status label
            pbTrain.Style = ProgressBarStyle.Continuous;
            pbTrain.MarqueeAnimationSpeed = 0;
            pbTrain.Minimum = 0;
            pbTrain.Maximum = 100;
            pbTrain.Value = 0;
            pbTrain.Visible = true;

            // Clear previous summary
            rtbTrainSummary.Clear();
            rtbTrainSummary.BringToFront();

            // Show training status label
            lblTrainStatus.Visible = true;
            lblTrainStatus.Text = "0%";

            rtbTrainSummary.Clear();
            AppendSectionHeader("Training Log");

            try
            {
                var ml = new MLContext(seed: 0);

                // Load from DB 
                var trainingData = ModelTrainer.LoadTrainingDataFromDatabase();
                var dataView = ml.Data.LoadFromEnumerable(trainingData);

                // Deterministic split for lineage reproducibility
                var split = ml.Data.TrainTestSplit(dataView, testFraction: 0.2, seed: 0);

                var prog = new Progress<TrainingStatus>(s =>
                {
                    // Estimate % by time (AutoML doesn’t expose true %)
                    double pct = (s.TotalSeconds > 0) ? Math.Min(100.0, (s.ElapsedSeconds / s.TotalSeconds) * 100.0) : 0.0;
                    int p = (int)Math.Round(pct);
                    p = Math.Max(pbTrain.Minimum, Math.Min(pbTrain.Maximum, p));
                    pbTrain.Value = p;
                    lblTrainStatus.Text = $"{p}%";

                    // Compact, readable log line into the RTB
                    var bestPart = (s.BestValAccuracy.HasValue && !string.IsNullOrEmpty(s.BestTrainer))
                        ? $"  (best: {s.BestTrainer} {s.BestValAccuracy.Value:P2})"
                        : string.Empty;

                    // Compact message with trainer
                    var trainerPart = !string.IsNullOrEmpty(s.CurrentTrainer) ? $"  • {s.CurrentTrainer}" : "";
                    

                    lblTrainStatus.Text = $"{pct:0}% — {s.Message}{trainerPart}{bestPart}";

                    if (s.Done)
                    {
                        pbTrain.Value = 100;
                        lblTrainStatus.Text = s.Message;
                    }
                });

                var result = await Task.Run(() => ModelTrainer.TrainAndLogModel(split.TrainSet, split.TestSet, prog, maxExperimentSeconds: 60));

                // Final summary in the same RTB (clear visual break)
                AppendSectionHeader("Training Summary");
                AppendLog(result.Report);

            }
            catch (Exception ex)
            {
                AppendSectionHeader("Training Failed");
                AppendLog(ex.Message);
                lblTrainStatus.Text = "Error";
            }
            finally
            {
                btnTrain.Enabled = true;
                // keep pb + label visible so the user can read the final message
                // (hide them later if you want)
            }
        }



        private const int MaxLogChars = 80_000; // cap to avoid huge control

        private void AppendLog(string line)
        {
            if (rtbTrainSummary.InvokeRequired)
            {
                rtbTrainSummary.BeginInvoke(new Action<string>(AppendLog), line);
                return;
            }

            // Trim old text to keep control snappy
            if (rtbTrainSummary.TextLength > MaxLogChars)
                rtbTrainSummary.Select(0, rtbTrainSummary.TextLength - MaxLogChars / 2); // drop first half
            else
                rtbTrainSummary.Select(rtbTrainSummary.TextLength, 0);

            // Optional: subtle color for running updates
            rtbTrainSummary.SelectionColor = Color.DimGray;
            rtbTrainSummary.AppendText((rtbTrainSummary.TextLength > 0 ? Environment.NewLine : string.Empty) + line);

            // Autoscroll
            rtbTrainSummary.SelectionStart = rtbTrainSummary.TextLength;
            rtbTrainSummary.ScrollToCaret();
        }

        private void AppendSectionHeader(string title)
        {
            if (rtbTrainSummary.InvokeRequired)
            {
                rtbTrainSummary.BeginInvoke(new Action<string>(AppendSectionHeader), title);
                return;
            }
            rtbTrainSummary.AppendText(Environment.NewLine + Environment.NewLine);
            rtbTrainSummary.SelectionFont = new Font("Consolas", 10f, FontStyle.Bold);
            rtbTrainSummary.SelectionColor = Color.Black;
            rtbTrainSummary.AppendText(title + Environment.NewLine);
            rtbTrainSummary.SelectionFont = new Font("Consolas", 9f, FontStyle.Regular);
        }



        // Determines the current best model ID from the database
        private static int GetCurrentBestModelId()
        {
            using var conn = new SqlConnection(DatabaseConfig.FullConnection);
            conn.Open();

            // Try current best
            using (var cmd = new SqlCommand(
                $"SELECT TOP 1 ModelID FROM {DatabaseConfig.TableModelPerformance} WHERE IsCurrentBest = 1 ORDER BY TrainingDate DESC;", conn))
            {
                var obj = cmd.ExecuteScalar();
                if (obj != null) return (int)obj;
            }

            // Fallback to latest by date
            using (var cmd2 = new SqlCommand(
                $"SELECT TOP 1 ModelID FROM {DatabaseConfig.TableModelPerformance} ORDER BY TrainingDate DESC;", conn))
            {
                var obj2 = cmd2.ExecuteScalar();
                if (obj2 != null) return (int)obj2;
            }

            // Nothing trained yet
            throw new InvalidOperationException("No models exist yet. Train a model first on the Train tab.");
        }


    }
}
