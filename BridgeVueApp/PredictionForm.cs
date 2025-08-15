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
            string connString = "Server=localhost;Database=BridgeVue;Integrated Security=True;TrustServerCertificate=True;";
            string sql = "SELECT TOP 1 * FROM vStudentMLTrainingData ORDER BY NEWID()";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Pull StudentID from DB row
                        int studentId = reader.GetInt32(reader.GetOrdinal("StudentID"));

                        // Build input and predict
                        var input = ModelInputFactory.FromReader(reader);
                        var result = ML_Class_Success.Predict(input);

                        // Top-1 class from score vector
                        var (topIdx, confidence) = Top1(result.Score);

                        // Map class index -> human label (guard if out of range)
                        string predictedExitReason = topIdx >= 0
                            ? MLLookups.Lookup(MLLookups.ExitReason, topIdx)
                            : "Unknown";

                        // Log the prediction to InferenceLog
                        int modelId = GetCurrentBestModelId();
                        PredictionLogger.LogPrediction(
                            modelId,
                            studentId.ToString(),
                            predictedExitReason,
                            confidence
                        );

                        

                        // UI output (kept your formatting)
                        rtbRandomPredictionOutput.Clear();
                        rtbRandomPredictionOutput.Font = new Font("Consolas", 10);

                        rtbRandomPredictionOutput.SelectionAlignment = HorizontalAlignment.Center;
                        rtbRandomPredictionOutput.SelectionFont = new Font("Consolas", 10, FontStyle.Bold);
                        rtbRandomPredictionOutput.SelectionColor = Color.Blue;
                        rtbRandomPredictionOutput.AppendText("EXIT REASON PREDICTION\n");
                        rtbRandomPredictionOutput.AppendText("============================\n\n");

                        rtbRandomPredictionOutput.SelectionFont = new Font("Consolas", 12, FontStyle.Bold);
                        rtbRandomPredictionOutput.SelectionColor = predictedExitReason == "Graduated" ? Color.Green : Color.Orange;
                        rtbRandomPredictionOutput.AppendText($"{predictedExitReason}\n[Confidence: {confidence:P1}]\n\n");


                        //float modelAccuracy = 0.94f;

                        // Student Summary Header
                        rtbRandomPredictionOutput.SelectionFont = new Font("Consolas", 10, FontStyle.Bold);
                        rtbRandomPredictionOutput.SelectionColor = Color.Blue;
                        rtbRandomPredictionOutput.AppendText("------------------------------\n");
                        rtbRandomPredictionOutput.AppendText("   STUDENT SUMMARY\n");
                        rtbRandomPredictionOutput.AppendText("------------------------------\n\n");

                        // Left-align data with left padding
                        rtbRandomPredictionOutput.SelectionAlignment = HorizontalAlignment.Left;
                        string leftPadding = new string(' ', 6);

                        // Define columns with lookup conversions
                        string[] leftLabels = {
                        "Grade:", "Age:", "Gender:", "Ethnicity:", "Special Ed:", "IEP:",
                        "Entry Reason:", "Academic Level:", "Social Skills:", "Risk Score:"
                };

                        string[] leftValues = {
                        input.Grade.ToString(),
                        input.Age.ToString(),
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
                        input.PriorIncidents.ToString(),
                        input.OfficeReferrals.ToString(),
                        input.Suspensions.ToString(),
                        input.Expulsions.ToString(),
                        input.AvgVerbalAggression.ToString("F2"),
                        input.AvgPhysicalAggression.ToString("F2"),
                        input.AvgAcademicEngagement.ToString("F2"),
                        input.RedZonePct.ToString("P1"),
                        input.GreenZonePct.ToString("P1"),
                        input.BehaviorDays.ToString()
                };

                        int maxRows = Math.Max(leftLabels.Length, rightLabels.Length);

                        for (int i = 0; i < maxRows; i++)
                        {
                            string leftLabel = i < leftLabels.Length ? leftLabels[i].PadRight(18) : "".PadRight(18);
                            string leftValue = i < leftValues.Length ? leftValues[i] : "";

                            string rightLabel = i < rightLabels.Length ? rightLabels[i].PadRight(18) : "".PadRight(18);
                            string rightValue = i < rightValues.Length ? rightValues[i] : "";

                            // Print left label in black
                            rtbRandomPredictionOutput.SelectionColor = Color.Black;
                            rtbRandomPredictionOutput.SelectionFont = new Font("Consolas", 10, FontStyle.Regular);
                            rtbRandomPredictionOutput.AppendText(leftPadding + leftLabel);

                            // Print left value in black
                            rtbRandomPredictionOutput.SelectionColor = Color.Black;
                            rtbRandomPredictionOutput.AppendText(leftValue.PadRight(15));

                            // Print right label in black
                            rtbRandomPredictionOutput.SelectionColor = Color.Black;
                            rtbRandomPredictionOutput.AppendText(rightLabel);

                            // Apply coloring to right value
                            if (i >= 4) // Behavior data
                            {
                                if (i == 4) // Verbal Aggression
                                    rtbRandomPredictionOutput.SelectionColor = input.AvgVerbalAggression < 0.5f ? Color.Green : Color.Red;
                                else if (i == 5) // Physical Aggression
                                    rtbRandomPredictionOutput.SelectionColor = input.AvgPhysicalAggression < 0.5f ? Color.Green : Color.Red;
                                else if (i == 6) // Engagement
                                    rtbRandomPredictionOutput.SelectionColor = input.AvgAcademicEngagement >= 3.0f ? Color.Green : Color.Red;
                                else if (i == 7) // Red Zone %
                                    rtbRandomPredictionOutput.SelectionColor = input.RedZonePct < 0.25f ? Color.Orange : Color.Red;
                                else if (i == 8) // Green Zone %
                                    rtbRandomPredictionOutput.SelectionColor = input.GreenZonePct > 0.5f ? Color.Green : Color.Orange;
                                else
                                    rtbRandomPredictionOutput.SelectionColor = Color.Black;

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
                    else
                    {
                        rtbRandomPredictionOutput.Text = "No student data found.";
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
        private void btnBatchPredict_Click(object sender, EventArgs e)
        {
            // Log the button click for debugging
            Debug.WriteLine($"Button clicked at {DateTime.Now:HH:mm:ss.fff}");

            string connString = "Server=localhost;Database=BridgeVue;Integrated Security=True;TrustServerCertificate=True;";
            string sql = "SELECT * FROM vStudentMLTrainingData ORDER BY StudentID ASC";

            List<BatchPredictionResult> results = new List<BatchPredictionResult>();

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);


                using (var reader = cmd.ExecuteReader())
                {
                    var ords = new OrdinalCache(reader);  // build once
                    while (reader.Read())
                    {
                        // 1) Pull StudentID from DB row
                        int studentId = reader.GetInt32(reader.GetOrdinal("StudentID"));

                        // 2) Predict
                        var input = ModelInputFactory.FromReader(reader, ords);
                        var prediction = ML_Class_Success.Predict(input);

                        // 3) Label + score (SAFE)
                        var (topIdx, confidence) = Top1(prediction.Score ?? Array.Empty<float>());
                        string predictedExitReason = topIdx >= 0
                            ? MLLookups.Lookup(MLLookups.ExitReason, topIdx)
                            : "Unknown";

                        // 4) Log (don’t let logging break UI)
                        try
                        {
                            int modelId = GetCurrentBestModelId();
                            PredictionLogger.LogPrediction(
                                modelId,
                                studentId.ToString(),
                                predictedExitReason,
                                confidence
                            );
                        }
                        catch (Exception logEx)
                        {
                            Debug.WriteLine($"Inference logging failed: {logEx.Message}");
                        }

                        // 5) Add to results grid
                        results.Add(new BatchPredictionResult
                        {
                            StudentID = studentId,
                            PredictedOutcome = predictedExitReason,
                            Confidence = confidence.ToString("P1"),
                            Gender = MLLookups.Lookup(MLLookups.Gender, Convert.ToInt32(input.GenderNumeric)),
                            Ethnicity = MLLookups.Lookup(MLLookups.Ethnicity, Convert.ToInt32(input.EthnicityNumeric)),
                            Grade = Convert.ToInt32(input.Grade),
                            RiskScore = input.RiskScore,
                            AvgVerbalAggression = input.AvgVerbalAggression,
                            AvgPhysicalAggression = input.AvgPhysicalAggression,
                            AvgAcademicEngagement = input.AvgAcademicEngagement,
                            RedZonePct = input.RedZonePct,
                            GreenZonePct = input.GreenZonePct
                        });

                    }
                }
            }

            dgvBatchPrediction.DataSource = results;
            dgvBatchPrediction.Invalidate();
            dgvBatchPrediction.Refresh();

        }

        // Handle row pre-paint to color rows based on predicted exit reason
        private void DgvBatchPrediction_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            var row = dgvBatchPrediction.Rows[e.RowIndex];
            string outcome = row.Cells["PredictedOutcome"].Value?.ToString();

            switch (outcome)
            {
                case "Returned Successfully":
                    row.DefaultCellStyle.BackColor = Color.FromArgb(220, 255, 220);  // Soft Green
                    row.DefaultCellStyle.ForeColor = Color.DarkGreen;
                    break;
                case "ACC":
                    row.DefaultCellStyle.BackColor = Color.FromArgb(220, 240, 255);  // Soft Blue
                    row.DefaultCellStyle.ForeColor = Color.DarkBlue;
                    break;
                case "Transferred":
                    row.DefaultCellStyle.BackColor = Color.FromArgb(240, 248, 255);  // Alice Blue
                    row.DefaultCellStyle.ForeColor = Color.DarkSlateBlue;
                    break;
                case "Referred Out":
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 240, 220);  // Soft Orange
                    row.DefaultCellStyle.ForeColor = Color.DarkOrange;
                    break;
                case "Dropped Out":
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 220, 220);  // Soft Red
                    row.DefaultCellStyle.ForeColor = Color.DarkRed;
                    break;
                case "Unknown":
                case "Other":
                    row.DefaultCellStyle.BackColor = Color.FromArgb(250, 250, 220);  // Soft Yellow
                    row.DefaultCellStyle.ForeColor = Color.DarkGoldenrod;
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
            using var cmd = new SqlCommand(
                $"SELECT TOP 1 ModelID FROM {DatabaseConfig.TableModelPerformance} WHERE IsCurrentBest = 1 ORDER BY ModelID DESC;",
                conn);
            var obj = cmd.ExecuteScalar();
            if (obj == null) throw new InvalidOperationException("No current best model found. Train a model first.");
            return (int)obj;
        }

    }
}
