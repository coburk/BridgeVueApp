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
                Grade = 1F,
                Age = 8F,
                GenderNum = 0F,
                EthnicityNum = 1F,
                SpecialEd = true,
                IEP = false,
                EntryReasonNum = 4F,
                PriorIncidents = 0F,
                OfficeReferrals = 0F,
                Suspensions = 0F,
                Expulsions = 0F,
                EntryAcademicLevelNum = 3F,
                EntrySocialSkillsLevelNum = 2F,
                AvgVerbalAggression = 0F,
                AvgPhysicalAggression = 0F,
                AvgAcademicEngagement = 3F,
                RedZonePct = 0.24632353F,
                LengthOfStay = 62F,
            };

            var result = ML_Class_Success.Predict(input);

            lblStaticPrediction.Text = $"Predicted Exit Outcome: {result.PredictedLabel}";
        }




        // Predict a random student from the database
        private void btnRandomStudentPredict_Click(object sender, EventArgs e)
        {
            PredictRandomStudent();
        }


        // Helper methods for conversion
        private int ConvertAcademicLevelToNum(string level)
        {
            return level switch
            {
                "Below Grade" => 0,
                "At Grade" => 1,
                "Above Grade" => 2,
                _ => -1 // Unknown
            };
        }

        private int ConvertSocialSkillsLevelToNum(string level)
        {
            return level switch
            {
                "Low" => 0,
                "Medium" => 1,
                "High" => 2,
                _ => -1 // Unknown
            };
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
                        string entryAcademicLevel = reader.GetString(reader.GetOrdinal("EntryAcademicLevel"));
                        string entrySocialSkillsLevel = reader.GetString(reader.GetOrdinal("EntrySocialSkillsLevel"));

                        var input = new ML_Class_Success.ModelInput()
                        {
                            Grade = reader.GetInt32(reader.GetOrdinal("Grade")),
                            Age = reader.GetInt32(reader.GetOrdinal("Age")),
                            GenderNum = reader.GetInt32(reader.GetOrdinal("Gender")),
                            EthnicityNum = reader.GetInt32(reader.GetOrdinal("Ethnicity")),
                            SpecialEd = reader.GetBoolean(reader.GetOrdinal("SpecialEd")),
                            IEP = reader.GetBoolean(reader.GetOrdinal("IEP")),
                            EntryReasonNum = reader.GetInt32(reader.GetOrdinal("EntryReason")),
                            PriorIncidents = reader.GetInt32(reader.GetOrdinal("PriorIncidents")),
                            OfficeReferrals = reader.GetInt32(reader.GetOrdinal("OfficeReferrals")),
                            Suspensions = reader.GetInt32(reader.GetOrdinal("Suspensions")),
                            Expulsions = reader.GetInt32(reader.GetOrdinal("Expulsions")),
                            EntryAcademicLevelNum = ConvertAcademicLevelToNum(entryAcademicLevel),
                            EntrySocialSkillsLevelNum = ConvertSocialSkillsLevelToNum(entrySocialSkillsLevel),
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
                            input.GenderNum.ToString(),
                            input.EthnicityNum.ToString(),
                            input.SpecialEd ? "Yes" : "No",
                            input.IEP ? "Yes" : "No",
                            input.EntryReasonNum.ToString(),
                            input.EntryAcademicLevelNum.ToString(),
                            input.EntrySocialSkillsLevelNum.ToString()
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

        // Manual What-If Prediction button click handler
        private void btnManualPredict_Click(object sender, EventArgs e)
        {
            try
            {
                var input = new ML_Class_Success.ModelInput()
                {
                    Grade = Convert.ToInt32(cmbGrade.Text),
                    Age = Convert.ToInt32(nudAge.Value),
                    GenderNum = cmbGender.value,
                    EthnicityNum = ConvertAcademicLevelToNum(cmbEthnicity.Text),
                    SpecialEd = chkSpecialEd.Checked,
                    IEP = chkIEP.Checked,
                    EntryReasonNum = cmbEntryReasonNum.Text,
                    PriorIncidents = Convert.ToInt32(nudPriorIncidents.Value),
                    OfficeReferrals = Convert.ToInt32(nudOfficeReferrals.Value),
                    Suspensions = Convert.ToInt32(nudSuspensions.Value),
                    Expulsions = Convert.ToInt32(nudExpulsions.Value),
                    EntryAcademicLevelNum = ConvertAcademicLevelToNum(cmbAcademicLevel.Text),
                    EntrySocialSkillsLevelNum = ConvertSocialSkillsLevelToNum(cmbSocialSkills.Text),
                    AvgVerbalAggression = (float)nudAvgVerbal.Value,
                    AvgPhysicalAggression = (float)nudAvgPhysical.Value,
                    AvgAcademicEngagement = (float)nudAvgEngagement.Value,
                    RedZonePct = (float)(nudRedZonePct.Value / 100.0m),  // Convert from percent to decimal
                };

                var result = ML_Class_Success.Predict(input);

                rtbManualPredictionOuput.Text = $"Predicted Outcome: {result.PredictedLabel}";
                rtbManualPredictionOuput.ForeColor = result.PredictedLabel == "Returned Successfully" ? Color.Green : Color.Red;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during prediction: {ex.Message}", "Prediction Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                        string entryAcademicLevel = reader.GetString(reader.GetOrdinal("EntryAcademicLevel"));
                        string entrySocialSkillsLevel = reader.GetString(reader.GetOrdinal("EntrySocialSkillsLevel"));

                        var input = new ML_Class_Success.ModelInput()
                        {
                            Grade = reader.GetInt32(reader.GetOrdinal("Grade")),
                            Age = reader.GetInt32(reader.GetOrdinal("Age")),
                            Gender = reader.GetString(reader.GetOrdinal("Gender")),
                            Ethnicity = reader.GetString(reader.GetOrdinal("Ethnicity")),
                            SpecialEd = reader.GetBoolean(reader.GetOrdinal("SpecialEd")),
                            IEP = reader.GetBoolean(reader.GetOrdinal("IEP")),
                            EntryReasonNum = reader.GetString(reader.GetOrdinal("EntryReasonNum")),
                            PriorIncidents = reader.GetInt32(reader.GetOrdinal("PriorIncidents")),
                            OfficeReferrals = reader.GetInt32(reader.GetOrdinal("OfficeReferrals")),
                            Suspensions = reader.GetInt32(reader.GetOrdinal("Suspensions")),
                            Expulsions = reader.GetInt32(reader.GetOrdinal("Expulsions")),
                            EntryAcademicLevelNum = ConvertAcademicLevelToNum(entryAcademicLevel),
                            EntrySocialSkillsLevelNum = ConvertSocialSkillsLevelToNum(entrySocialSkillsLevel),
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
                "The table below displays predicted outcomes for all current students in the BridgeVue program.\n" +
                "• Predicted Outcome: The model’s suggested reintegration outcome.\n" +
                "• Confidence: How certain the model is in its prediction.\n" +
                "• Behavior Metrics: Avg verbal/physical aggression, academic engagement, and red zone time.\n" +
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

    public class ModelInput
    {
        public int Grade { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string Ethnicity { get; set; }
        public bool SpecialEd { get; set; }
        public bool IEP { get; set; }
        public string EntryReason { get; set; }
        public int PriorIncidents { get; set; }
        public int OfficeReferrals { get; set; }
        public int Suspensions { get; set; }
        public int Expulsions { get; set; }


        // UI-friendly versions (not used by model)
        public string EntryAcademicLevel { get; set; }
        public string EntrySocialSkillsLevel { get; set; }


        // Model-used numeric versions
        public float EntryAcademicLevelNum { get; set; }
        public float EntrySocialSkillsLevelNum { get; set; }
        public float AvgVerbalAggression { get; set; }
        public float AvgPhysicalAggression { get; set; }
        public float AvgAcademicEngagement { get; set; }
        public float RedZonePct { get; set; }
    }
}
