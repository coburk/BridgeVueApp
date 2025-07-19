using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace BridgeVueApp
{
    public partial class PredictionForm : Form
    {
        public PredictionForm()
        {
            InitializeComponent();
            SetupBatchSummary();
        }




        private void btnPredictSatic_Click(object sender, EventArgs e)
        {
            var input = new ML_Class_Success.ModelInput()
            {
                Grade = 3,
                Age = 9,
                Gender = "Male",
                Ethnicity = "Hispanic",
                SpecialEd = true,
                IEP = false,
                EntryReason = "Aggression",
                PriorIncidents = 4,
                OfficeReferrals = 1,
                Suspensions = 1,
                Expulsions = 0,
                EntryAcademicLevel = "Below Grade",
                EntrySocialSkillsLevel = "Low",
                AvgVerbalAggression = 0.8f,
                AvgPhysicalAggression = 0.5f,
                AvgAcademicEngagement = 3.5f,
                RedZonePct = 0.25f
            };

            var result = ML_Class_Success.Predict(input);

            lblStaticPrediction.Text = $"Predicted Exit Outcome: {result.PredictedLabel}";
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
            string sql = "SELECT TOP 1 * FROM vStudentPredictionData ORDER BY NEWID()";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var input = new ML_Class_Success.ModelInput()
                        {
                            Grade = reader.GetInt32(reader.GetOrdinal("Grade")),
                            Age = reader.GetInt32(reader.GetOrdinal("Age")),
                            Gender = reader.GetString(reader.GetOrdinal("Gender")),
                            Ethnicity = reader.GetString(reader.GetOrdinal("Ethnicity")),
                            SpecialEd = reader.GetBoolean(reader.GetOrdinal("SpecialEd")),
                            IEP = reader.GetBoolean(reader.GetOrdinal("IEP")),
                            EntryReason = reader.GetString(reader.GetOrdinal("EntryReason")),
                            PriorIncidents = reader.GetInt32(reader.GetOrdinal("PriorIncidents")),
                            OfficeReferrals = reader.GetInt32(reader.GetOrdinal("OfficeReferrals")),
                            Suspensions = reader.GetInt32(reader.GetOrdinal("Suspensions")),
                            Expulsions = reader.GetInt32(reader.GetOrdinal("Expulsions")),
                            EntryAcademicLevel = reader.GetString(reader.GetOrdinal("EntryAcademicLevel")),
                            EntrySocialSkillsLevel = reader.GetString(reader.GetOrdinal("EntrySocialSkillsLevel")),
                            AvgVerbalAggression = Convert.ToSingle(reader.GetValue(reader.GetOrdinal("AvgVerbalAggression"))),
                            AvgPhysicalAggression = Convert.ToSingle(reader.GetValue(reader.GetOrdinal("AvgPhysicalAggression"))),
                            AvgAcademicEngagement = Convert.ToSingle(reader.GetValue(reader.GetOrdinal("AvgAcademicEngagement"))),
                            RedZonePct = Convert.ToSingle(reader.GetValue(reader.GetOrdinal("RedZonePct")))

                        };

                        var result = ML_Class_Success.Predict(input);

                        // Assume accuracy is stored or hardcoded for now
                        float modelAccuracy = 0.94f;

                        // Build output string for the RichTextBox

                        // Clear and set font
                        rtbRandomPredictionOutput.Clear();
                        rtbRandomPredictionOutput.Font = new Font("Consolas", 10);

                        // Centered Header
                        rtbRandomPredictionOutput.SelectionAlignment = HorizontalAlignment.Center;
                        rtbRandomPredictionOutput.SelectionFont = new Font("Consolas", 10, FontStyle.Bold);
                        rtbRandomPredictionOutput.SelectionColor = Color.Blue;
                        rtbRandomPredictionOutput.AppendText("PREDICTION RESULT\n");
                        rtbRandomPredictionOutput.AppendText("============================\n\n");

                        rtbRandomPredictionOutput.SelectionFont = new Font("Consolas", 10, FontStyle.Bold);
                        rtbRandomPredictionOutput.SelectionColor = result.PredictedLabel == "Returned Successfully" ? Color.Green : Color.Red;
                        rtbRandomPredictionOutput.AppendText($"{result.PredictedLabel}   [Accuracy: {modelAccuracy:P0}]\n\n");

                        // Student Summary Header
                        rtbRandomPredictionOutput.SelectionFont = new Font("Consolas", 10, FontStyle.Bold);
                        rtbRandomPredictionOutput.SelectionColor = Color.Blue;
                        rtbRandomPredictionOutput.AppendText("------------------------------\n");
                        rtbRandomPredictionOutput.AppendText("   STUDENT SUMMARY\n");
                        rtbRandomPredictionOutput.AppendText("------------------------------\n\n");

                        // Left-align data with left padding
                        rtbRandomPredictionOutput.SelectionAlignment = HorizontalAlignment.Left;

                        string leftPadding = new string(' ', 6);

                        // Define columns
                        string[] leftLabels = {
                            "Grade:", "Age:", "Gender:", "Ethnicity:", "Special Ed:", "IEP:",
                            "Entry Reason:", "Academic Level:", "Social Skills:"
                };

                        string[] leftValues = {
                            input.Grade.ToString(),
                            input.Age.ToString(),
                            input.Gender,
                            input.Ethnicity,
                            input.SpecialEd ? "Yes" : "No",
                            input.IEP ? "Yes" : "No",
                            input.EntryReason,
                            input.EntryAcademicLevel,
                            input.EntrySocialSkillsLevel
                };

                        string[] rightLabels = {
                            "Prior Incidents:", "Office Referrals:", "Suspensions:", "Expulsions:",
                            "Avg Verbal Agg:", "Avg Physical Agg:", "Avg Engagement:", "Red Zone %:"
                };

                        string[] rightValues = {
                            input.PriorIncidents.ToString(),
                            input.OfficeReferrals.ToString(),
                            input.Suspensions.ToString(),
                            input.Expulsions.ToString(),
                            input.AvgVerbalAggression.ToString("F2"),
                            input.AvgPhysicalAggression.ToString("F2"),
                            input.AvgAcademicEngagement.ToString("F2"),
                            input.RedZonePct.ToString("P1")
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
                                    rtbRandomPredictionOutput.SelectionColor = input.RedZonePct < 0.25f ? Color.Green : Color.Red;
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
            string connString = "Server=localhost;Database=BridgeVue;Integrated Security=True;TrustServerCertificate=True;";
            string sql = "SELECT * FROM vStudentPredictionData ORDER BY StudentID ASC\r\n";

            List<BatchPredictionResult> results = new List<BatchPredictionResult>();

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var input = new ML_Class_Success.ModelInput()
                        {
                            Grade = reader.GetInt32(reader.GetOrdinal("Grade")),
                            Age = reader.GetInt32(reader.GetOrdinal("Age")),
                            Gender = reader.GetString(reader.GetOrdinal("Gender")),
                            Ethnicity = reader.GetString(reader.GetOrdinal("Ethnicity")),
                            SpecialEd = reader.GetBoolean(reader.GetOrdinal("SpecialEd")),
                            IEP = reader.GetBoolean(reader.GetOrdinal("IEP")),
                            EntryReason = reader.GetString(reader.GetOrdinal("EntryReason")),
                            PriorIncidents = reader.GetInt32(reader.GetOrdinal("PriorIncidents")),
                            OfficeReferrals = reader.GetInt32(reader.GetOrdinal("OfficeReferrals")),
                            Suspensions = reader.GetInt32(reader.GetOrdinal("Suspensions")),
                            Expulsions = reader.GetInt32(reader.GetOrdinal("Expulsions")),
                            EntryAcademicLevel = reader.GetString(reader.GetOrdinal("EntryAcademicLevel")),
                            EntrySocialSkillsLevel = reader.GetString(reader.GetOrdinal("EntrySocialSkillsLevel")),
                            AvgVerbalAggression = Convert.ToSingle(reader.GetValue(reader.GetOrdinal("AvgVerbalAggression"))),
                            AvgPhysicalAggression = Convert.ToSingle(reader.GetValue(reader.GetOrdinal("AvgPhysicalAggression"))),
                            AvgAcademicEngagement = Convert.ToSingle(reader.GetValue(reader.GetOrdinal("AvgAcademicEngagement"))),
                            RedZonePct = Convert.ToSingle(reader.GetValue(reader.GetOrdinal("RedZonePct")))
                        };

                        var prediction = ML_Class_Success.Predict(input);

                        results.Add(new BatchPredictionResult
                        {
                            StudentID = reader.GetInt32(reader.GetOrdinal("StudentID")),
                            PredictedOutcome = prediction.PredictedLabel,
                            Confidence = prediction.Score.Max().ToString("P1"),
                            AvgVerbalAggression = input.AvgVerbalAggression,
                            AvgPhysicalAggression = input.AvgPhysicalAggression,
                            AvgAcademicEngagement = input.AvgAcademicEngagement,
                            RedZonePct = input.RedZonePct
                        });
                    }
                }
            }

            dgvBatchPrediction.DataSource = results;
        }

        // Handle row pre-paint to color rows based on predicted outcome
        private void DgvBatchPrediction_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            var row = dgvBatchPrediction.Rows[e.RowIndex];
            string outcome = row.Cells["PredictedOutcome"].Value.ToString();

            if (outcome == "Returned Successfully")
            {
                row.DefaultCellStyle.BackColor = Color.LightGreen;
            }
            else if (outcome == "Referred Out")
            {
                row.DefaultCellStyle.BackColor = Color.LightCoral;
            }
            else
            {
                row.DefaultCellStyle.BackColor = Color.LightYellow;
            }
        }

        // Setup the batch summary text and DataGridView styles
        private void SetupBatchSummary()
        {
            lblBatchSummary.Text =
                "The table below displays predicted outcomes for all current students in the BridgeVue program.\n\n" +
                "• Predicted Outcome: The model’s suggested reintegration outcome.\n" +
                "• Confidence: How certain the model is in its prediction.\n" +
                "• Behavior Metrics: Avg verbal/physical aggression, academic engagement, and red zone time.\n\n" +
                "These predictions are supportive tools for staff and should be combined with professional judgment.";

            dgvBatchPrediction.RowPrePaint += DgvBatchPrediction_RowPrePaint;
        }

    }


    public class BatchPredictionResult
    {
        public int StudentID { get; set; }
        public string PredictedOutcome { get; set; }
        public string Confidence { get; set; }
        public float AvgVerbalAggression { get; set; }
        public float AvgPhysicalAggression { get; set; }
        public float AvgAcademicEngagement { get; set; }
        public float RedZonePct { get; set; }
    }
}
