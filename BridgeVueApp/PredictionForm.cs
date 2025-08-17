using BridgeVueApp.Data;
using BridgeVueApp.Database;
using BridgeVueApp.MachineLearning;
using Microsoft.Data.SqlClient;
using Microsoft.ML;
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using static BridgeVueApp.MainForm;
using ScottPlot.WinForms;





namespace BridgeVueApp
{
    public partial class PredictionForm : Form
    {
        private readonly BindingList<BatchProgressRow> _batchProgress = new BindingList<BatchProgressRow>();

        private sealed class BatchProgressRow
        {
            public DateTime Time { get; set; }
            public string Stage { get; set; } = "";
            public string Message { get; set; } = "";
        }


        // host panel and plot for the history chart
        private Panel panelMetrics;
        private ScottPlot.WinForms.FormsPlot metricsPlot;



        public PredictionForm()
        {
            // Initialize the form components
            InitializeComponent();

            // Don't run DB/chart code in the Designer
            if (IsInDesignMode) return;

            // Set up the DataGridView for Model Summary
            this.Shown += (_, __) =>
            {
                try { LoadModelSummaryIntoGrid(dgvModelSummary); }
                catch { }
                EnsureBatchControls();
            };


            // Initialize the form
            SetupBatchSummary();

            // Fix grid selection/sorting compatibility
            ConfigureGridForRows(dgvBatchPrediction);
            ConfigureGridForRows(dgvModelSummary);

            // Set up the chart for model metrics
            LoadModelHistoryIntoGrid(dgvModelHistory);
            EnsureMetricsPanelCreated();
            EnsureMetricsPlotCreated();
            RenderModelMetricsChart();



            // Set up the progress bar and labels
            pbTrain.Style = ProgressBarStyle.Continuous;
            pbTrain.MarqueeAnimationSpeed = 0;
            pbTrain.Minimum = 0;
            pbTrain.Maximum = 100;
            pbTrain.Value = 0;
            pbTrain.Visible = false;

            // Set up the training status label
            lblTrainStatus.Visible = false;
            lblTrainStatus.Text = "";
        }


        
        // Handle the Predict Static button click
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
            rtbRandomPredictionOutput.Text =
                $"Predicted Exit Outcome: {MLLookups.Lookup(MLLookups.ExitReason, Convert.ToInt32(result.PredictedLabel))}";

            if (result == null)
                rtbRandomPredictionOutput.Text = "No student data found.";
        }




        private void btnRandomStudentPredict_Click(object sender, EventArgs e)
        {
            // Predict a random student from the database
            if (!ModelAvailable())
            {
                rtbRandomPredictionOutput.Text =
                    "No trained model found. Go to the Train tab and train a model first.";
                return;
            }

            PredictRandomStudent();

        }




        // Predict a random student from the database using the same pipeline as Batch
        private void PredictRandomStudent()
        {
            // Load current model (guard if missing)
            var lm = BatchPredictor.TryLoadCurrentBest();
            if (lm is null)
            {
                rtbRandomPredictionOutput.Text =
                    "No trained model found or the model file is missing. Please train a model first.";
                return;
            }


            // Pull a random student row
            const string connString = "Server=localhost;Database=BridgeVue;Integrated Security=True;TrustServerCertificate=True;";
            const string sql = "SELECT TOP 1 * FROM vStudentMLDataRaw ORDER BY NEWID()";

            using var conn = new SqlConnection(connString);
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            if (!reader.Read())
            {
                rtbRandomPredictionOutput.Text = "No student data found.";
                return;
            }

            // Helpers
            int Ord(string name) => reader.GetOrdinal(name);
            bool Has(string name) { try { return reader.GetOrdinal(name) >= 0; } catch { return false; } }
            float F(string col) => reader.IsDBNull(Ord(col)) ? 0f : Convert.ToSingle(reader[col]);
            int I(string col) => reader.IsDBNull(Ord(col)) ? 0 : reader.GetInt32(Ord(col));
            bool B(string col) => !reader.IsDBNull(Ord(col)) && reader.GetBoolean(Ord(col));

            // Build SAME schema as Batch
            var input = new PredictionInput
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

                // required by pipeline even for prediction
                ExitReason = ""
            };

            // Predict using your Batch wrapper API
            var pred = BatchPredictor.PredictOne(lm, input);
            string predictedExitReason = pred.Label;
            float confidence = pred.Prob;

            // Log to inference table
            if (BatchPredictor.TryGetCurrentModel(out var modelId, out _))
            {
                PredictionLogger.LogPrediction(modelId, input.StudentID.ToString(), predictedExitReason, confidence);
            }




            // ---------- UI rendering ----------
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



        private async void btnBatchPredict_Click(object sender, EventArgs e)
        {
            try
            {
                btnBatchPredict.Enabled = false;
                Cursor = Cursors.WaitCursor;

                _batchProgress.Clear();
                dgvBatchPrediction.DataSource = _batchProgress;
                dgvBatchPrediction.AutoGenerateColumns = true;

                // Ensure a model exists and capture its ID
                if (!BridgeVueApp.MachineLearning.BatchPredictor.TryGetCurrentModel(out var modelId, out _))
                {
                    AppendBatchProgress("Batch", "No trained model found or model file missing. Please train a model first.");
                    return;
                }

                AppendBatchProgress("Batch", "Starting batch predictions…");

                var prog = new Progress<string>(msg => AppendBatchProgress("Batch", msg));

                // Pass the same modelId to Run so we score with that exact model
                int wrote = await Task.Run(() => BridgeVueApp.MachineLearning.BatchPredictor.Run(
                    prog,
                    overrideModelId: modelId
                ));

                AppendBatchProgress("Batch", $"Scoring finished. {wrote} inference rows written.");
                AppendBatchProgress("Batch", "Loading latest predictions per student…");

                // Reuse the same ID here (this replaces 'currentModelId')
                var results = LoadLatestBatchResults(modelId);

                AppendBatchProgress("Batch", $"Loaded {results.Count} latest predictions. Rendering…");

                dgvBatchPrediction.DataSource = new BindingList<BatchPredictionResult>(results);
                dgvBatchPrediction.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                dgvBatchPrediction.Refresh();

                AppendBatchProgress("Batch", "Done.");
            }
            catch (Exception ex)
            {
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
                // var succ = DN("SuccessProbability") ?? 0m; // if you add to UI

                list.Add(new BatchPredictionResult
                {
                    StudentID = studentId,
                    PredictedOutcome = string.IsNullOrWhiteSpace(label) ? "Unknown" : label,
                    Confidence = (top1.HasValue ? ((float)top1.Value).ToString("P1") : ""),
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

        // Append a progress message to the batch progress DataGridView
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
                "• Predicted Outcome: The model's suggested exit reason (Returned Successfully, ACC, etc.).\n" +
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
            public string PredictedOutcome { get; set; }
            public string Confidence { get; set; }
            public string Gender { get; set; }
            public string Ethnicity { get; set; }
            public int Grade { get; set; }
            public float RiskScore { get; set; }
            public float AvgVerbalAggression { get; set; }
            public float AvgPhysicalAggression { get; set; }
            public float AvgAcademicEngagement { get; set; }
            public float RedZonePct { get; set; }
            public float GreenZonePct { get; set; }
        }





        // Handle the Train button click event
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

                    var bestPart = (s.BestValAccuracy.HasValue && !string.IsNullOrEmpty(s.BestTrainer))
                        ? $"  (best: {s.BestTrainer} {s.BestValAccuracy.Value:P2})" : string.Empty;

                    var trainerPart = !string.IsNullOrEmpty(s.CurrentTrainer) ? $"  • {s.CurrentTrainer}" : "";

                    lblTrainStatus.Text = $"{pct:0}% — {s.Message}{trainerPart}{bestPart}";

                    if (s.Done)
                    {
                        pbTrain.Value = 100;
                        lblTrainStatus.Text = s.Message;
                    }
                });

                var result = await Task.Run(() => ModelTrainer.TrainAndLogModel(
                    split.TrainSet, split.TestSet, prog, maxExperimentSeconds: 60));


                // Final summary
                AppendSectionHeader("Training Summary");
                AppendLog(result.Report);



                // refresh history grid + chart after a new run
                LoadModelHistoryIntoGrid(dgvModelHistory);
                RenderModelMetricsChart();


                // refresh current-best summary
                LoadModelSummaryIntoGrid(dgvModelSummary);




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

            if (rtbTrainSummary.TextLength > MaxLogChars)
                rtbTrainSummary.Select(0, rtbTrainSummary.TextLength - MaxLogChars / 2);
            else
                rtbTrainSummary.Select(rtbTrainSummary.TextLength, 0);

            rtbTrainSummary.SelectionColor = Color.DimGray;
            rtbTrainSummary.AppendText((rtbTrainSummary.TextLength > 0 ? Environment.NewLine : string.Empty) + line);
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

            throw new InvalidOperationException("No models exist yet. Train a model first on the Train tab.");
        }




        // ----- Model summary grid -----
        private void LoadModelSummaryIntoGrid(DataGridView grid)
        {
            using var conn = new SqlConnection(DatabaseConfig.FullConnection);
            conn.Open();

            // 1) Current-best model
            int modelId = -1;
            var model = new DataTable();
            using (var cmd = new SqlCommand($@"
                SELECT TOP (1)
                    mp.ModelID, mp.TrainingDate, mp.ModelName, mp.ModelType,
                    mp.Accuracy, mp.F1Score, mp.TrainingDataSize, mp.TestDataSize, mp.ModelFilePath
                FROM {DatabaseConfig.TableModelPerformance} mp
                WHERE mp.IsCurrentBest = 1
                ORDER BY mp.TrainingDate DESC;", conn))
            using (var da = new SqlDataAdapter(cmd)) { da.Fill(model); }

            if (model.Rows.Count == 0)
            {
                grid.DataSource = null;
                return;
            }

            modelId = Convert.ToInt32(model.Rows[0]["ModelID"]);

            // 2) Snapshots linked to this model
            var snaps = new DataTable();
            using (var cmd = new SqlCommand($@"
                SELECT
                    mdu.Role,
                    ds.SnapshotID,
                    ds.CreatedAtUtc,
                    ds.SourceView,
                    ds.SelectionQuery,
                    ds.[RowCount],
                    ds.FeatureSchemaHash,
                    ds.DataContentHash
                FROM {DatabaseConfig.TableModelDataUsage} mdu
                JOIN {DatabaseConfig.TableDatasetSnapshot} ds
                  ON ds.SnapshotID = mdu.SnapshotID
                WHERE mdu.ModelID = @m
                ORDER BY CASE mdu.Role WHEN 'TRAIN' THEN 0 WHEN 'TEST' THEN 1 ELSE 2 END, ds.CreatedAtUtc;", conn))
            {
                cmd.Parameters.AddWithValue("@m", modelId);
                using var da = new SqlDataAdapter(cmd);
                da.Fill(snaps);
            }

            // 3) Compose a friendly summary table for the grid
            var summary = new DataTable();
            summary.Columns.Add("ModelID", typeof(int));
            summary.Columns.Add("TrainingDate", typeof(DateTime));
            summary.Columns.Add("Trainer", typeof(string));
            summary.Columns.Add("Type", typeof(string));
            summary.Columns.Add("Accuracy", typeof(double));
            summary.Columns.Add("F1", typeof(double));
            summary.Columns.Add("TrainRows", typeof(int));
            summary.Columns.Add("TestRows", typeof(int));
            summary.Columns.Add("TrainSnapshotID", typeof(int));
            summary.Columns.Add("TestSnapshotID", typeof(int));

            var row = model.Rows[0];
            int trainSnap = snaps.AsEnumerable().FirstOrDefault(r => string.Equals((string)r["Role"], "TRAIN", StringComparison.OrdinalIgnoreCase))?.Field<int>("SnapshotID") ?? 0;
            int testSnap = snaps.AsEnumerable().FirstOrDefault(r => string.Equals((string)r["Role"], "TEST", StringComparison.OrdinalIgnoreCase))?.Field<int>("SnapshotID") ?? 0;

            summary.Rows.Add(
                (int)row["ModelID"],
                (DateTime)row["TrainingDate"],
                (string)row["ModelName"],
                (string)row["ModelType"],
                row["Accuracy"] == DBNull.Value ? 0.0 : Convert.ToDouble(row["Accuracy"]),
                row["F1Score"] == DBNull.Value ? 0.0 : Convert.ToDouble(row["F1Score"]),
                row["TrainingDataSize"] == DBNull.Value ? 0 : Convert.ToInt32(row["TrainingDataSize"]),
                row["TestDataSize"] == DBNull.Value ? 0 : Convert.ToInt32(row["TestDataSize"]),
                trainSnap,
                testSnap
            );

            grid.AutoGenerateColumns = true;
            grid.DataSource = summary;
        }


        // Handle the Model Summary Refresh button click
        private void btnModelRefresh_Click(object sender, EventArgs e)
        {
            LoadModelSummaryIntoGrid(dgvModelSummary);
        }


        // Configure the DataGridView to display rows properly
        private void ConfigureGridForRows(DataGridView g)
        {
            g.ReadOnly = true;
            g.MultiSelect = false;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // <-- key change
            g.AutoGenerateColumns = true;

            // Tidy columns after data binding
            g.DataBindingComplete += (s, e) =>
            {
                foreach (DataGridViewColumn c in g.Columns)
                {
                    c.SortMode = DataGridViewColumnSortMode.Automatic;   // safe with FullRowSelect
                    c.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }

            };
        }


        


            // Plot Accuracy & F1 over time
            

        /// <summary>
        /// Hides any listed columns if *every* row is NULL/blank for that column.
        /// This prevents showing confusing zero-like placeholders for unused metrics.
        /// </summary>
        private static void HideEmptyMetricColumns(DataGridView grid, params string[] columnNames)
        {
            foreach (var name in columnNames)
            {
                if (!grid.Columns.Contains(name)) continue;

                bool allBlank = true;
                foreach (DataGridViewRow r in grid.Rows)
                {
                    if (r.IsNewRow) continue;
                    var v = r.Cells[name].Value;
                    // treat DBNull/ null / empty string as blank
                    if (v != null && v != DBNull.Value && !string.IsNullOrWhiteSpace(Convert.ToString(v)))
                    {
                        allBlank = false; break;
                    }
                }
                if (allBlank)
                    grid.Columns[name].Visible = false;
            }
        }



        // ----- Model history grid -----
        private void LoadModelHistoryIntoGrid(DataGridView grid)
        {
            using var conn = new SqlConnection(DatabaseConfig.FullConnection);
            conn.Open();

            var dt = new DataTable();
            using (var cmd = new SqlCommand($@"
        SELECT
            mp.ModelID,
            mp.TrainingDate,
            mp.ModelName,
            mp.ModelType,
            mp.Accuracy,          -- MicroAccuracy (mapped to Accuracy)
            mp.F1Score,           -- MacroAccuracy (mapped to F1Score)
            mp.TrainingDataSize,
            mp.TestDataSize,
            mp.IsCurrentBest
        FROM {DatabaseConfig.TableModelPerformance} mp
        ORDER BY mp.TrainingDate DESC;", conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }

            grid.AutoGenerateColumns = true;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.DataSource = dt;
            grid.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

            if (grid.Columns.Contains("IsCurrentBest"))
            {
                grid.Columns["IsCurrentBest"].HeaderText = "Best?";
                grid.Columns["IsCurrentBest"].Width = 55;
            }
            if (grid.Columns.Contains("ModelName")) grid.Columns["ModelName"].HeaderText = "Trainer";
            if (grid.Columns.Contains("Accuracy")) grid.Columns["Accuracy"].DefaultCellStyle.Format = "P2";
            if (grid.Columns.Contains("F1Score"))
            {
                grid.Columns["F1Score"].HeaderText = "F1 (Macro)";
                grid.Columns["F1Score"].DefaultCellStyle.Format = "P2";
            }

            foreach (DataGridViewRow r in grid.Rows)
            {
                if (r.IsNewRow) continue;
                if (grid.Columns.Contains("IsCurrentBest") && r.Cells["IsCurrentBest"].Value is bool b && b)
                    r.DefaultCellStyle.BackColor = Color.FromArgb(235, 255, 235);
            }
        }




        // Ensure the metrics plot is created and added to the host control
        private void EnsureMetricsPanelCreated()
        {
            if (panelMetrics != null && !panelMetrics.IsDisposed) return;

            panelMetrics = new Panel
            {
                Name = "panelMetrics",
                Dock = DockStyle.Bottom,
                Height = 240,
                BackColor = SystemColors.Control
            };

            // Put it in the same container as the history grid
            var host = dgvModelHistory?.Parent ?? this;
            host.Controls.Add(panelMetrics);
            panelMetrics.BringToFront();
        }



        // Render the model metrics chart (Accuracy and F1 over time)
        // Create (once) the ScottPlot control inside panelMetrics
        private void EnsureMetricsPlotCreated()
        {
            if (metricsPlot != null && !metricsPlot.IsDisposed) return;

            EnsureMetricsPanelCreated();

            metricsPlot = new ScottPlot.WinForms.FormsPlot
            {
                Dock = DockStyle.Fill
            };

            panelMetrics.Controls.Add(metricsPlot);
        }


        // Render Accuracy & F1 over time into metricsPlot
        private void RenderModelMetricsChart()
        {
            if (IsInDesignMode) return;
            EnsureMetricsPlotCreated();

            var tAcc = new List<DateTime>();
            var acc = new List<double>();
            var tF1 = new List<DateTime>();
            var f1 = new List<double>();

            using var conn = new SqlConnection(DatabaseConfig.FullConnection);
            conn.Open();
            using var cmd = new SqlCommand($@"
        SELECT [Timestamp], Accuracy, F1Score
        FROM {DatabaseConfig.TableModelMetricsHistory}
        ORDER BY [Timestamp];", conn);
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                var ts = rd.GetDateTime(0);
                if (!rd.IsDBNull(1)) { tAcc.Add(ts); acc.Add(Convert.ToDouble(rd.GetDecimal(1))); }
                if (!rd.IsDBNull(2)) { tF1.Add(ts); f1.Add(Convert.ToDouble(rd.GetDecimal(2))); }
            }

            double[] xAcc = tAcc.Select(d => d.ToOADate()).ToArray();
            double[] xF1 = tF1.Select(d => d.ToOADate()).ToArray();

            metricsPlot.Plot.Clear();

            if (acc.Count > 0)
            {
                var sAcc = metricsPlot.Plot.Add.Scatter(xAcc, acc.ToArray());
                sAcc.LegendText = "Accuracy";
            }

            if (f1.Count > 0)
            {
                var sF1 = metricsPlot.Plot.Add.Scatter(xF1, f1.ToArray());
                sF1.LegendText = "F1";
            }

            // v5 axis/legend API
            metricsPlot.Plot.Axes.DateTimeTicksBottom();
            metricsPlot.Plot.Axes.Left.Label.Text = "Score";
            metricsPlot.Plot.Axes.Title.Label.Text = "Model Metrics Over Time";
            metricsPlot.Plot.Legend.IsVisible = true;

            metricsPlot.Refresh();
        }



        private static bool ModelAvailable()
        {
            using var conn = new SqlConnection(DatabaseConfig.FullConnection);
            conn.Open();
            using var cmd = new SqlCommand($@"
        SELECT TOP (1) ModelFilePath
        FROM {DatabaseConfig.TableModelPerformance}
        WHERE IsCurrentBest = 1
        ORDER BY TrainingDate DESC;", conn);

            var pathObj = cmd.ExecuteScalar();
            if (pathObj == null) return false;

            var path = Convert.ToString(pathObj);
            if (string.IsNullOrWhiteSpace(path)) return false;

            try { return File.Exists(path); } catch { return false; }
        }





        // Design-time helper (works in ctor and at runtime)
        private static bool IsInDesignMode =>
            LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
            string.Equals(Process.GetCurrentProcess().ProcessName, "devenv", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(Process.GetCurrentProcess().ProcessName, "DesignToolsServer", StringComparison.OrdinalIgnoreCase);



        private void rtbTrainSummary_TextChanged(object sender, EventArgs e)
        {

        }



        // Return true and set modelId if a trained model exists; false otherwise.
        private static bool TryGetCurrentBestModelId(out int modelId)
        {
            modelId = 0;
            try
            {
                using var conn = new SqlConnection(DatabaseConfig.FullConnection);
                conn.Open();

                // Prefer explicit "current best"
                using (var cmd = new SqlCommand(
                    $"SELECT TOP 1 ModelID FROM {DatabaseConfig.TableModelPerformance} WHERE IsCurrentBest = 1 ORDER BY TrainingDate DESC;", conn))
                {
                    var obj = cmd.ExecuteScalar();
                    if (obj != null) { modelId = Convert.ToInt32(obj); return true; }
                }

                // Fallback to latest by date
                using (var cmd2 = new SqlCommand(
                    $"SELECT TOP 1 ModelID FROM {DatabaseConfig.TableModelPerformance} ORDER BY TrainingDate DESC;", conn))
                {
                    var obj2 = cmd2.ExecuteScalar();
                    if (obj2 != null) { modelId = Convert.ToInt32(obj2); return true; }
                }
            }
            catch { /* ignore and return false */ }

            return false;
        }

        // Enable/disable Batch controls and show a friendly notice if needed
        private void EnsureBatchControls()
        {
            if (IsInDesignMode) return;

            bool hasModel = TryGetCurrentBestModelId(out _);

            btnBatchPredict.Enabled = hasModel;

            // If no model yet, bind progress grid (if not already) and show guidance once
            if (!hasModel)
            {
                if (dgvBatchPrediction.DataSource == null)
                {
                    dgvBatchPrediction.DataSource = _batchProgress;
                    dgvBatchPrediction.AutoGenerateColumns = true;
                }

                // Clear any old progress noise and show a single helpful line
                _batchProgress.Clear();
                AppendBatchProgress(
                    "Batch",
                    "No trained model found. Go to the Train tab, train a model, then return here to score students."
                );
            }
        }


            // Returns null instead of throwing when no model is available
            private static BridgeVueApp.MachineLearning.BatchPredictor.LoadedModel? TryLoadCurrentModel()
            {
                try { return BridgeVueApp.MachineLearning.BatchPredictor.LoadCurrentBest(); }
                catch { return null; }
            }

            // Returns 0 if there is no current best / any model at all
            private static int TryGetCurrentBestModelId()
            {
                using var conn = new Microsoft.Data.SqlClient.SqlConnection(DatabaseConfig.FullConnection);
                conn.Open();

                // current best first
                using (var cmd = new Microsoft.Data.SqlClient.SqlCommand(
                    $"SELECT TOP 1 ModelID FROM {DatabaseConfig.TableModelPerformance} WHERE IsCurrentBest = 1 ORDER BY TrainingDate DESC;", conn))
                {
                    var obj = cmd.ExecuteScalar();
                    if (obj != null) return (int)obj;
                }

                // else newest by date
                using (var cmd2 = new Microsoft.Data.SqlClient.SqlCommand(
                    $"SELECT TOP 1 ModelID FROM {DatabaseConfig.TableModelPerformance} ORDER BY TrainingDate DESC;", conn))
                {
                    var obj2 = cmd2.ExecuteScalar();
                    if (obj2 != null) return (int)obj2;
                }

                return 0;
            }
    }
}
