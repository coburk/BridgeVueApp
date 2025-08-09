// SetupForm.cs

using BridgeVueApp.Database;
using BridgeVueApp.DataGeneration;
using BridgeVueApp.Models;
using CsvHelper;
using Microsoft.Data.SqlClient;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using BVDatabase = BridgeVueApp.Database.DatabaseLoader;





namespace BridgeVueApp
{
    public partial class SetupForm : Form
    {

        // In-memory Data
        private List<StudentProfile> generatedProfiles = new List<StudentProfile>();
        private List<IntakeData> generatedIntake = new List<IntakeData>();
        private List<DailyBehavior> generatedBehavior = new List<DailyBehavior>();
        private List<ExitData> generatedExitData = new List<ExitData>();

        public SetupForm()
        {
            InitializeComponent();
        }




        private void btnCreateDatabaseAndTables_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show(
                "⚠️ This will drop the existing database and delete all current data.\n\nDo you want to continue?",
                "Confirm Database Reset",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (confirm != DialogResult.Yes)
            {
                lblStatus.Text = "Database reset cancelled.";
                return;
            }

            try
            {
                progressBar.Value = 0;
                lblStatus.Text = "⏳ Dropping and recreating database...";

                // Full reset logic
                DatabaseInitializer.DropDatabaseIfExists();
                DatabaseInitializer.CreateDatabaseIfNotExists();
                DatabaseInitializer.CreateTablesIfNotExist();

                lblStatus.Text = "✅ Database and tables recreated successfully.";
                progressBar.Value = 100;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Reset Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "❌ Database reset failed.";
            }
        }






        // View Database Info
        private void btnViewDatabaseInfo_Click(object sender, EventArgs e)
        {
            try
            {
                lblStatus.Text = "Retrieving database information...";
                Application.DoEvents();

                using (SqlConnection dbConnection = new SqlConnection(DatabaseConfig.FullConnection))
                {
                    dbConnection.Open();

                    // Get table row counts
                    string tableQuery = @"
                SELECT t.NAME AS TableName, SUM(p.rows) AS [RowCount]
                FROM sys.tables t
                INNER JOIN sys.indexes i 
                    ON t.OBJECT_ID = i.object_id
                INNER JOIN sys.partitions p 
                    ON i.object_id = p.OBJECT_ID AND i.index_id = p.index_id
                WHERE t.is_ms_shipped = 0 
                    AND i.type <= 1
                GROUP BY t.NAME
                ORDER BY t.NAME;";

                    // Get database metadata
                    string dbInfoQuery = @"
                SELECT 
                    DB_NAME() AS DatabaseName,
                    (SELECT COUNT(*) FROM sys.tables WHERE is_ms_shipped = 0) AS TableCount,
                    (SELECT COUNT(*) FROM sys.views WHERE is_ms_shipped = 0) AS ViewCount,
                    (SELECT COUNT(*) FROM sys.indexes WHERE object_id IN (SELECT object_id FROM sys.tables WHERE is_ms_shipped = 0)) AS IndexCount";

                    SqlCommand dbInfoCmd = new SqlCommand(dbInfoQuery, dbConnection);
                    SqlDataReader dbInfoReader = dbInfoCmd.ExecuteReader();

                    string databaseName = "";
                    int tableCount = 0, viewCount = 0, indexCount = 0;

                    if (dbInfoReader.Read())
                    {
                        databaseName = dbInfoReader["DatabaseName"].ToString();
                        tableCount = Convert.ToInt32(dbInfoReader["TableCount"]);
                        viewCount = Convert.ToInt32(dbInfoReader["ViewCount"]);
                        indexCount = Convert.ToInt32(dbInfoReader["IndexCount"]);
                    }
                    dbInfoReader.Close();

                    // Get table row counts
                    SqlCommand cmd = new SqlCommand(tableQuery, dbConnection);
                    SqlDataReader reader = cmd.ExecuteReader();

                    StringBuilder result = new StringBuilder();
                    result.AppendLine("DATABASE INFORMATION");
                    result.AppendLine($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC | User: {Environment.UserName}");
                    result.AppendLine($"Database: {databaseName}");
                    result.AppendLine("");
                    result.AppendLine("SUMMARY:");
                    result.AppendLine($"   Tables: {tableCount}");
                    result.AppendLine($"   Views: {viewCount}");
                    result.AppendLine($"   Indexes: {indexCount}");
                    result.AppendLine("");
                    result.AppendLine("TABLE ROW COUNTS:");
                    result.AppendLine("   ┌─────────────────────────┬────────────┐");
                    result.AppendLine("   │ Table Name              │ Row Count  │");
                    result.AppendLine("   ├─────────────────────────┼────────────┤");

                    int totalRows = 0;
                    var tableData = new List<(string name, int count)>();

                    while (reader.Read())
                    {
                        string tableName = reader["TableName"].ToString();
                        int rowCount = Convert.ToInt32(reader["RowCount"]);
                        tableData.Add((tableName, rowCount));
                        totalRows += rowCount;
                    }

                    // Add table rows with nice formatting
                    foreach (var (name, count) in tableData.OrderBy(x => x.name))
                    {
                        result.AppendLine($"   │ {name,-23} │ {count,10:N0} │");
                    }

                    result.AppendLine("   ├─────────────────────────┼────────────┤");
                    result.AppendLine($"   │ TOTAL RECORDS           │ {totalRows,10:N0} │");
                    result.AppendLine("   └─────────────────────────┴────────────┘");

                    reader.Close();

                    // Get additional insights if we have data
                    if (totalRows > 0)
                    {
                        result.AppendLine("");
                        result.AppendLine("DATA INSIGHTS:");
                        AddDataInsights(dbConnection, result, tableData); // Use dbConnection for data insights
                    }

                    result.AppendLine("");
                    result.AppendLine("Database information retrieved successfully");

                    // Set monospace font for better alignment
                    lblStatus.Font = new Font("Consolas", 9F, FontStyle.Regular);
                    lblStatus.Text = result.ToString().TrimEnd();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Failed to retrieve database info: {ex.Message}";
                MessageBox.Show($"Error retrieving database information:\n\n{ex.Message}", "Database Info Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }





        // Add data insights to the database info display
        private void AddDataInsights(SqlConnection conn, StringBuilder result, List<(string name, int count)> tableData)
        {
            try
            {
                // Get current vs exited students
                string insightsQuery = @"
            SELECT 
                (SELECT COUNT(*) FROM StudentProfile) as TotalStudents,
                (SELECT COUNT(*) FROM ExitData) as ExitedStudents,
                (SELECT COUNT(DISTINCT StudentID) FROM DailyBehavior) as StudentsWithBehaviorData,
                (SELECT AVG(CAST(LengthOfStay AS FLOAT)) FROM ExitData) as AvgLengthOfStay,
                (SELECT COUNT(*) FROM DailyBehavior WHERE ZoneOfRegulation = 'Green') as GreenZoneDays,
                (SELECT COUNT(*) FROM DailyBehavior WHERE ZoneOfRegulation = 'Red') as RedZoneDays,
                (SELECT MIN(EntryDate) FROM IntakeData) as EarliestEntry,
                (SELECT MAX(CASE WHEN ExitDate IS NOT NULL THEN ExitDate ELSE GETDATE() END) FROM IntakeData i LEFT JOIN ExitData e ON i.StudentID = e.StudentID) as LatestActivity";

                SqlCommand insightsCmd = new SqlCommand(insightsQuery, conn);
                SqlDataReader insightsReader = insightsCmd.ExecuteReader();

                if (insightsReader.Read())
                {
                    int totalStudents = Convert.ToInt32(insightsReader["TotalStudents"] ?? 0);
                    int exitedStudents = Convert.ToInt32(insightsReader["ExitedStudents"] ?? 0);
                    int currentStudents = totalStudents - exitedStudents;
                    int studentsWithBehavior = Convert.ToInt32(insightsReader["StudentsWithBehaviorData"] ?? 0);
                    double avgLengthOfStay = Convert.ToDouble(insightsReader["AvgLengthOfStay"] ?? 0);
                    int greenZoneDays = Convert.ToInt32(insightsReader["GreenZoneDays"] ?? 0);
                    int redZoneDays = Convert.ToInt32(insightsReader["RedZoneDays"] ?? 0);

                    DateTime? earliestEntry = insightsReader["EarliestEntry"] as DateTime?;
                    DateTime? latestActivity = insightsReader["LatestActivity"] as DateTime?;

                    result.AppendLine($"   Students: {totalStudents:N0} total ({currentStudents:N0} current, {exitedStudents:N0} exited)");
                    result.AppendLine($"   Behavior Tracking: {studentsWithBehavior:N0} students");
                    result.AppendLine($"   Avg Length of Stay: {avgLengthOfStay:F1} days");

                    if (greenZoneDays + redZoneDays > 0)
                    {
                        double greenPercentage = (double)greenZoneDays / (greenZoneDays + redZoneDays) * 100;
                        result.AppendLine($"   Green Zone: {greenZoneDays:N0} days ({greenPercentage:F1}%)");
                        result.AppendLine($"   Red Zone: {redZoneDays:N0} days ({100 - greenPercentage:F1}%)");
                    }

                    if (earliestEntry.HasValue && latestActivity.HasValue)
                    {
                        TimeSpan dataSpan = latestActivity.Value - earliestEntry.Value;
                        result.AppendLine($"   Data Range: {dataSpan.Days} days ({earliestEntry.Value:yyyy-MM-dd} to {latestActivity.Value:yyyy-MM-dd})");
                    }
                }
                insightsReader.Close();
            }
            catch (Exception ex)
            {
                result.AppendLine($"   Insights calculation error: {ex.Message}");
            }
        }







        // Generate Synthetic Student Data
        private async void btnGenerateSyntheticData_Click(object sender, EventArgs e)
        {
            progressBar.Value = 0;
            lblStatus.Text = "Generating synthetic data...";

            var progress = new Progress<string>(status => lblStatus.Text = status);
            var generator = new DataGenerationManager(progress);

            try
            {
                // Generate synthetic data with weekly emotions but Do Not Save
                await generator.GenerateSyntheticOnlyAsync(includeWeeklyEmotions: true);  
                lblStatus.Text = $"Synthetic data generation complete. {DataGenerationUtils.LastExitSummary.Count} exit records generated.";
                progressBar.Value = 100;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Synthetic data generation failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "❌ Synthetic data generation failed.";
            }
        }








        // Load Generated Data into Database
        private void btnLoadGeneratedData_Click(object sender, EventArgs e)
        {
            try
            {
                // Separate loader logic - assumes StudentProfiles already inserted and IDs assigned
                var savedProfiles = BVDatabase.BulkInsertStudentProfiles(DataGenerationUtils.GeneratedProfiles);
                progressBar.Value = 20;
                BVDatabase.BulkInsertIntakeData(DataGenerationUtils.GeneratedIntake);
                progressBar.Value = 40;
                BVDatabase.BulkInsertDailyBehavior(DataGenerationUtils.GeneratedBehavior);
                progressBar.Value = 60;
                BVDatabase.BulkInsertExitData(DataGenerationUtils.GeneratedExitData);
                progressBar.Value = 80;

                lblStatus.Text = $"✅ Saved {DataGenerationUtils.GeneratedProfiles.Count} students to DB.";
                progressBar.Value = 100;
            }


            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save to database: {ex.Message}");
                lblStatus.Text = "❌ Save failed.";
                progressBar.Value = 0;
            }
        }




        // Save Generated Data as CSVs
        private void btnSaveGeneratedCSV_Click(object sender, EventArgs e)
        {


            try
            {
                string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
                string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                               "BridgeVueData", $"SyntheticData_{timestamp}");
                Directory.CreateDirectory(folderPath);

                // Save Student Profiles with new numeric fields
                using (var writer = new StreamWriter(Path.Combine(folderPath, "StudentProfile.csv")))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(DataGenerationUtils.GeneratedProfiles);
                    progressBar.Value = 25;
                }

                // Save Intake Data with new numeric and normalized fields
                using (var writer = new StreamWriter(Path.Combine(folderPath, "IntakeData.csv")))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(DataGenerationUtils.GeneratedIntake);
                    progressBar.Value = 50;
                }

                // Save Daily Behavior with new numeric and ML fields
                using (var writer = new StreamWriter(Path.Combine(folderPath, "DailyBehavior.csv")))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(DataGenerationUtils.GeneratedBehavior);
                    progressBar.Value = 70;
                }

                // Save Exit Data with new numeric and ML metrics
                using (var writer = new StreamWriter(Path.Combine(folderPath, "ExitData.csv")))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(DataGenerationUtils.GeneratedExitData);
                    progressBar.Value = 80;
                }

                // Create a summary file with metadata
                using (var writer = new StreamWriter(Path.Combine(folderPath, "DataSummary.txt")))
                {
                    writer.WriteLine($"Synthetic Data Export Summary");
                    writer.WriteLine($"Generated on: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
                    writer.WriteLine($"Generated by: {Environment.UserName}");
                    writer.WriteLine($"Export Folder: {folderPath}");
                    writer.WriteLine($"");
                    writer.WriteLine($"Record Counts:");
                    writer.WriteLine($"  Student Profiles: {DataGenerationUtils.GeneratedProfiles.Count}");
                    writer.WriteLine($"  Intake Records: {DataGenerationUtils.GeneratedIntake.Count}");
                    writer.WriteLine($"  Daily Behavior Records: {DataGenerationUtils.GeneratedBehavior.Count}");
                    writer.WriteLine($"  Exit Records: {DataGenerationUtils.GeneratedExitData.Count}");
                    writer.WriteLine($"");
                    writer.WriteLine($"File Contents:");
                    writer.WriteLine($"  StudentProfile.csv    - Student demographics with text and numeric equivalents");
                    writer.WriteLine($"  IntakeData.csv        - Intake assessments with normalized ML features");
                    writer.WriteLine($"  DailyBehavior.csv     - Daily behavior tracking with ML metrics");
                    writer.WriteLine($"  ExitData.csv          - Program completion data with improvement metrics");
                    writer.WriteLine($"  DataSummary.txt       - Summary of the generated data");

                }
                progressBar.Value = 90;


                // Show success message with option to open folder
                DialogResult result = MessageBox.Show($"Data successfully exported to:\n{folderPath}\n\nWould you like to open the folder?",
                                                    "Export Complete", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                progressBar.Value = 100;

                if (result == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start("explorer.exe", folderPath);
                }

                btnLoadStudentProfile_Click(sender, e);
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"❌ Export Failed: {ex.Message}";
                MessageBox.Show($"Failed to export CSV files:\n\n{ex.Message}", "Export Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                progressBar.Value = 0;
            }

        }




        // Analyze Exit Outcomes
        private void btnExitOutcomeCount_Click(object sender, EventArgs e)
        {
            // Analyze the Data Distribution
            // Count of Each Exit Outcome

            {
                try
                {
                    using (SqlConnection dbConnection = new SqlConnection(DatabaseConfig.FullConnection))
                    {
                        dbConnection.Open();
                        string query = @"
                        SELECT ExitReason, COUNT(*) AS NumStudents
                        FROM ExitData
                        GROUP BY ExitReason;";

                        SqlCommand cmd = new SqlCommand(query, dbConnection);
                        SqlDataReader reader = cmd.ExecuteReader();

                        // Use a StringBuilder for performance
                        StringBuilder result = new StringBuilder();
                        result.AppendLine("Count of Each Exit Outcome:");
                        result.AppendLine("------------------------------");
                        result.AppendLine($"{"Outcome",-25}{"Count",5}");
                        result.AppendLine("------------------------------");

                        while (reader.Read())
                        {
                            string outcome = reader["ExitReason"].ToString();
                            int count = Convert.ToInt32(reader["NumStudents"]);

                            result.AppendLine($"{outcome,-25}{count,5}");
                        }

                        lblStatus.Font = new Font("Consolas", 10);  // Use monospaced font for perfect alignment
                        lblStatus.Text = result.ToString();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                    lblStatus.Text = "Failed to retrieve database info.";
                    progressBar.Value = 0;
                }
            }
        }





        // Analyze Behavior Averages by Exit Outcome
        private void btnExitOutcomeAvgs_Click(object sender, EventArgs e)
        {
            // Analyze the Data Distribution
            // Behavior Averages by Exit Outcome

            {
                try
                {
                    using (SqlConnection dbConnection = new SqlConnection(DatabaseConfig.FullConnection))
                    {
                        dbConnection.Open();
                        string query = @"
                        SELECT ExitReason,
                            AVG(AvgVerbalAggression) AS AvgVerbal,
                            AVG(AvgPhysicalAggression) AS AvgPhysical,
                            AVG(AvgAcademicEngagement) AS AvgEngagement,
                            AVG(RedZonePct) AS AvgRedZone
                        FROM vStudentPredictionData v
                        JOIN ExitData e 
                            ON v.StudentID = e.StudentID
                        GROUP BY ExitReason;";

                        SqlCommand cmd = new SqlCommand(query, dbConnection);
                        SqlDataReader reader = cmd.ExecuteReader();

                        // Use a StringBuilder for performance
                        StringBuilder result = new StringBuilder();
                        result.AppendLine("Behavior Averages by Exit Outcome:");
                        result.AppendLine("---------------------------------------------------------------------");
                        result.AppendLine($"{"Outcome",-25}{"Verbal",8}{"Physical",9}{"Engagement",12}{"Red Zone",10}");
                        result.AppendLine("---------------------------------------------------------------------");

                        while (reader.Read())
                        {
                            string outcome = reader["ExitReason"].ToString();
                            float avgVerbal = Convert.ToSingle(reader["AvgVerbal"]);
                            float avgPhysical = Convert.ToSingle(reader["AvgPhysical"]);
                            float avgEngagement = Convert.ToSingle(reader["AvgEngagement"]);
                            float avgRedZone = Convert.ToSingle(reader["AvgRedZone"]);

                            result.AppendLine($"{outcome,-25}{avgVerbal,8:F2}{avgPhysical,9:F2}{avgEngagement,12:F2}{avgRedZone,10:P1}");
                        }

                        lblStatus.Font = new Font("Consolas", 10);  // Use monospace for alignment
                        lblStatus.Text = result.ToString();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                    lblStatus.Text = "Failed to retrieve database info.";
                    progressBar.Value = 0;
                }

            }
        }





        // Load Student Profile Data
        private void btnLoadStudentProfile_Click(object sender, EventArgs e)
        {
            // Implementation for loading StudentProfile CSV
            lblStatus.Text = "Ready to Load Student Profile Data...But nothing is here!";
            //lblStatus.Text = "Student Profile Data loaded successfully.";
        }




        // Load Intake Data
        private void btnLoadIntakeData_Click(object sender, EventArgs e)
        {
            // Implementation for loading IntakeData CSV
            lblStatus.Text = "Ready to Load Intake Data...But nothing is here!";
            //lblStatus.Text = "Intake Data loaded successfully.";
        }



        // Load Daily Behavior
        private void btnLoadDailyBehavior_Click(object sender, EventArgs e)
        {
            // Implementation for loading DailyBehavior CSV
            lblStatus.Text = "Ready to Load Daily Behavior Data...But nothing is here!";
            //lblStatus.Text = "DailyBehavior loaded successfully.";
        }





        // Helper method to extract percentage from status message
        private int ExtractPercentage(string status)
        {
            var match = System.Text.RegularExpressions.Regex.Match(status, @"(\d+)%");
            if (match.Success && int.TryParse(match.Groups[1].Value, out int percent))
                return percent;
            return 0;
        }




        public class ModelPerformance
        {
            public string ModelName { get; set; }
            public DateTime TrainingDate { get; set; }
            public float Accuracy { get; set; }
            public float F1Score { get; set; }
            public float AUC { get; set; }
            public float Precision { get; set; }
            public float Recall { get; set; }
            public int TrainingDataSize { get; set; }
            public int TestDataSize { get; set; }
            public bool IsCurrentBest { get; set; }
            public string ModelFilePath { get; set; }
        }
    }
}
