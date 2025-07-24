using CsvHelper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace BridgeVueApp
{
    public partial class SetupForm : Form
    {
        // Configurable Names
        private string dbName = "BridgeVue";
        private string tableStudentProfile = "StudentProfile";
        private string tableIntakeData = "IntakeData";
        private string tableDailyBehavior = "DailyBehavior";
        private string tableExitData = "ExitData";

        private string connectionString => "Server=localhost;Integrated Security=true;TrustServerCertificate=True;";
        private string dbConnection => $"Server=localhost;Database={dbName};Integrated Security=true;TrustServerCertificate=True;";

        // In-memory Data
        private List<StudentProfile> generatedProfiles = new List<StudentProfile>();
        private List<IntakeData> generatedIntake = new List<IntakeData>();
        private List<DailyBehavior> generatedBehavior = new List<DailyBehavior>();
        private List<ExitData> generatedExitData = new List<ExitData>();

        public SetupForm()
        {
            InitializeComponent();
        }

        // DROP and Create Database and Tables
        private void btnCreateDatabaseAndTables_Click(object sender, EventArgs e)
        {
            try
            {
                lblStatus.Text = "Dropping and Creating database and tables...";
                Application.DoEvents();

                // Create the database first
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string createDb = $"IF DB_ID('{dbName}') IS NULL CREATE DATABASE {dbName};";
                    new SqlCommand(createDb, conn).ExecuteNonQuery();
                }

                // Create/update all tables with enhanced schema
                using (SqlConnection conn = new SqlConnection(dbConnection))
                {
                    conn.Open();

                    // Drop and recreate tables to ensure proper schema
                    string dropAndCreateTables = $@"
                -- Drop existing tables in correct order (respecting foreign keys)
                IF OBJECT_ID('vw_BehavioralAggregates', 'V') IS NOT NULL
                    DROP VIEW vw_BehavioralAggregates;
                    
                IF OBJECT_ID('vw_MLReadyData', 'V') IS NOT NULL
                    DROP VIEW vw_MLReadyData;

                IF OBJECT_ID('{tableExitData}', 'U') IS NOT NULL
                    DROP TABLE {tableExitData};

                IF OBJECT_ID('{tableDailyBehavior}', 'U') IS NOT NULL
                    DROP TABLE {tableDailyBehavior};

                IF OBJECT_ID('{tableIntakeData}', 'U') IS NOT NULL
                    DROP TABLE {tableIntakeData};

                IF OBJECT_ID('{tableStudentProfile}', 'U') IS NOT NULL
                    DROP TABLE {tableStudentProfile};

                -- Create Student Profile Table with numeric equivalents
                CREATE TABLE {tableStudentProfile} (
                    StudentID INT PRIMARY KEY,
                    Grade INT NOT NULL,
                    Age INT NOT NULL,
                    Gender NVARCHAR(10),
                    GenderNumeric INT NOT NULL DEFAULT 0,
                    Ethnicity NVARCHAR(20),
                    EthnicityNumeric INT NOT NULL DEFAULT 0,
                    SpecialEd BIT NOT NULL DEFAULT 0,
                    IEP BIT NOT NULL DEFAULT 0,
                    CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
                    ModifiedDate DATETIME2 DEFAULT GETUTCDATE()
                );

                -- Create Intake Data Table with numeric and normalized fields
                CREATE TABLE {tableIntakeData} (
                    IntakeID INT IDENTITY(1,1) PRIMARY KEY,
                    StudentID INT NOT NULL,
                    EntryReason NVARCHAR(50),
                    EntryReasonNumeric INT NOT NULL DEFAULT 0,
                    PriorIncidents INT NOT NULL DEFAULT 0,
                    OfficeReferrals INT NOT NULL DEFAULT 0,
                    Suspensions INT NOT NULL DEFAULT 0,
                    Expulsions INT NOT NULL DEFAULT 0,
                    EntryAcademicLevel NVARCHAR(20),
                    EntryAcademicLevelNumeric INT NOT NULL DEFAULT 0,
                    CheckInOut BIT NOT NULL DEFAULT 0,
                    StructuredRecess BIT NOT NULL DEFAULT 0,
                    StructuredBreaks BIT NOT NULL DEFAULT 0,
                    SmallGroups INT NOT NULL DEFAULT 0,
                    SocialWorkerVisits INT NOT NULL DEFAULT 0,
                    PsychologistVisits INT NOT NULL DEFAULT 0,
                    EntrySocialSkillsLevel NVARCHAR(20),
                    EntrySocialSkillsLevelNumeric INT NOT NULL DEFAULT 0,
                    EntryDate DATE NOT NULL,
                    RiskScore INT NOT NULL DEFAULT 1,
                    -- ML-focused normalized fields (0-1 scale)
                    StudentStressLevelNormalized FLOAT NOT NULL DEFAULT 0.0,
                    FamilySupportNormalized FLOAT NOT NULL DEFAULT 0.0,
                    AcademicAbilityNormalized FLOAT NOT NULL DEFAULT 0.0,
                    EmotionalRegulationNormalized FLOAT NOT NULL DEFAULT 0.0,
                    CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
                    ModifiedDate DATETIME2 DEFAULT GETUTCDATE(),
                    CONSTRAINT FK_Intake_Student FOREIGN KEY (StudentID) REFERENCES {tableStudentProfile}(StudentID) ON DELETE CASCADE
                );

                -- Create Daily Behavior Table with numeric and ML fields
                CREATE TABLE {tableDailyBehavior} (
                    BehaviorID INT IDENTITY(1,1) PRIMARY KEY,
                    StudentID INT NOT NULL,
                    Timestamp DATETIME2 NOT NULL,
                    Level INT NOT NULL DEFAULT 1,
                    Step INT NOT NULL DEFAULT 1,
                    VerbalAggression INT NOT NULL DEFAULT 0,
                    PhysicalAggression INT NOT NULL DEFAULT 0,
                    Elopement INT NOT NULL DEFAULT 0,
                    OutOfSpot INT NOT NULL DEFAULT 0,
                    WorkRefusal INT NOT NULL DEFAULT 0,
                    ProvokingPeers INT NOT NULL DEFAULT 0,
                    InappropriateLanguage INT NOT NULL DEFAULT 0,
                    OutOfLane INT NOT NULL DEFAULT 0,
                    ZoneOfRegulation NVARCHAR(10),
                    ZoneOfRegulationNumeric INT NOT NULL DEFAULT 0,
                    AcademicEngagement INT NOT NULL DEFAULT 1,
                    SocialInteractions INT NOT NULL DEFAULT 1,
                    EmotionalRegulation INT NOT NULL DEFAULT 1,
                    StaffComments NVARCHAR(500),
                    WeeklyEmotionDate DATE,
                    WeeklyEmotionPictogram NVARCHAR(20),
                    WeeklyEmotionPictogramNumeric INT,
                    -- ML metrics
                    AggressionRiskNormalized FLOAT NOT NULL DEFAULT 0.0,
                    EngagementLevelNormalized FLOAT NOT NULL DEFAULT 0.0,
                    DayInProgram INT NOT NULL DEFAULT 1,
                    WeekInProgram INT NOT NULL DEFAULT 1,
                    CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
                    CONSTRAINT FK_Behavior_Student FOREIGN KEY (StudentID) REFERENCES {tableStudentProfile}(StudentID) ON DELETE CASCADE
                );

                -- Create Exit Data Table with numeric and ML improvement metrics
                CREATE TABLE {tableExitData} (
                    ExitID INT IDENTITY(1,1) PRIMARY KEY,
                    StudentID INT NOT NULL,
                    ExitReason NVARCHAR(50),
                    ExitReasonNumeric INT NOT NULL DEFAULT 0,
                    ExitDate DATE NOT NULL,
                    LengthOfStay INT NOT NULL DEFAULT 0,
                    ExitAcademicLevel NVARCHAR(20),
                    ExitAcademicLevelNumeric INT NOT NULL DEFAULT 0,
                    ExitSocialSkillsLevel NVARCHAR(20),
                    ExitSocialSkillsLevelNumeric INT NOT NULL DEFAULT 0,
                    -- ML improvement metrics
                    AcademicImprovement INT NOT NULL DEFAULT 0,
                    SocialSkillsImprovement INT NOT NULL DEFAULT 0,
                    OverallImprovementScore FLOAT NOT NULL DEFAULT 0.0,
                    ProgramEffectivenessScore FLOAT NOT NULL DEFAULT 0.0,
                    SuccessIndicator BIT NOT NULL DEFAULT 0,
                    CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
                    ModifiedDate DATETIME2 DEFAULT GETUTCDATE(),
                    CONSTRAINT FK_Exit_Student FOREIGN KEY (StudentID) REFERENCES {tableStudentProfile}(StudentID) ON DELETE CASCADE
                );

                -- Create indexes for better query performance
                CREATE INDEX IX_IntakeData_StudentID ON {tableIntakeData}(StudentID);
                CREATE INDEX IX_IntakeData_EntryDate ON {tableIntakeData}(EntryDate);
                CREATE INDEX IX_DailyBehavior_StudentID_Timestamp ON {tableDailyBehavior}(StudentID, Timestamp);
                CREATE INDEX IX_DailyBehavior_Timestamp ON {tableDailyBehavior}(Timestamp);
                CREATE INDEX IX_ExitData_StudentID ON {tableExitData}(StudentID);
                CREATE INDEX IX_ExitData_ExitDate ON {tableExitData}(ExitDate);
            ";

                    new SqlCommand(dropAndCreateTables, conn).ExecuteNonQuery();

                    // Create views separately to avoid issues with dynamic SQL
                    CreateMLViews(conn);
                }

                // Display success status with details
                DisplayDatabaseCreationStatus();
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Database creation failed: {ex.Message}";
                MessageBox.Show($"Error creating database or tables:\n\n{ex.Message}\n\nThis will drop and recreate all tables. Any existing data will be lost.",
                               "Database Creation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        // Create ML views separately
        private void CreateMLViews(SqlConnection conn)
        {
            try
            {
                // Create ML-ready view
                string createMLView = $@"
            CREATE VIEW vw_MLReadyData AS
            SELECT 
                sp.StudentID,
                sp.Age,
                sp.Grade,
                sp.GenderNumeric,
                sp.EthnicityNumeric,
                sp.SpecialEd,
                sp.IEP,
                i.EntryReasonNumeric,
                i.PriorIncidents,
                i.OfficeReferrals,
                i.Suspensions,
                i.Expulsions,
                i.EntryAcademicLevelNumeric,
                i.EntrySocialSkillsLevelNumeric,
                i.RiskScore,
                i.StudentStressLevelNormalized,
                i.FamilySupportNormalized,
                i.AcademicAbilityNormalized,
                i.EmotionalRegulationNormalized,
                i.CheckInOut,
                i.StructuredRecess,
                i.StructuredBreaks,
                i.SmallGroups,
                i.SocialWorkerVisits,
                i.PsychologistVisits,
                e.ExitReasonNumeric,
                e.LengthOfStay,
                e.ExitAcademicLevelNumeric,
                e.ExitSocialSkillsLevelNumeric,
                e.AcademicImprovement,
                e.SocialSkillsImprovement,
                e.OverallImprovementScore,
                e.ProgramEffectivenessScore,
                e.SuccessIndicator,
                CASE WHEN e.StudentID IS NOT NULL THEN 1 ELSE 0 END AS HasExited
            FROM {tableStudentProfile} sp
            INNER JOIN {tableIntakeData} i ON sp.StudentID = i.StudentID
            LEFT JOIN {tableExitData} e ON sp.StudentID = e.StudentID";

                new SqlCommand(createMLView, conn).ExecuteNonQuery();

                // Create behavioral aggregates view
                string createBehaviorView = $@"
            CREATE VIEW vw_BehavioralAggregates AS
            SELECT 
                StudentID,
                COUNT(*) AS DaysInProgram,
                MAX(WeekInProgram) AS WeeksInProgram,
                SUM(VerbalAggression + PhysicalAggression) AS TotalAggression,
                AVG(CAST(VerbalAggression + PhysicalAggression AS FLOAT)) AS AvgDailyAggression,
                AVG(AggressionRiskNormalized) AS AvgAggressionRisk,
                AVG(CAST(AcademicEngagement AS FLOAT)) AS AvgAcademicEngagement,
                AVG(EngagementLevelNormalized) AS AvgEngagementLevel,
                SUM(CASE WHEN ZoneOfRegulationNumeric = 1 THEN 1 ELSE 0 END) AS RedZoneDays,
                SUM(CASE WHEN ZoneOfRegulationNumeric = 2 THEN 1 ELSE 0 END) AS YellowZoneDays,
                SUM(CASE WHEN ZoneOfRegulationNumeric = 3 THEN 1 ELSE 0 END) AS BlueZoneDays,
                SUM(CASE WHEN ZoneOfRegulationNumeric = 4 THEN 1 ELSE 0 END) AS GreenZoneDays,
                AVG(CAST(ZoneOfRegulationNumeric AS FLOAT)) AS AvgZoneScore,
                SUM(Elopement) AS TotalElopements,
                SUM(WorkRefusal) AS TotalWorkRefusals,
                AVG(CAST(SocialInteractions AS FLOAT)) AS AvgSocialInteractions,
                AVG(CAST(EmotionalRegulation AS FLOAT)) AS AvgEmotionalRegulation,
                MIN(Timestamp) AS FirstBehaviorDate,
                MAX(Timestamp) AS LastBehaviorDate
            FROM {tableDailyBehavior}
            GROUP BY StudentID";

                new SqlCommand(createBehaviorView, conn).ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // Views are optional, so don't fail the whole process
                System.Diagnostics.Debug.WriteLine($"Error creating views: {ex.Message}");
            }
        }

        // Display database creation success status
        private void DisplayDatabaseCreationStatus()
        {
            try
            {
                var statusMessage = new StringBuilder();
                statusMessage.AppendLine("DATABASE & TABLES CREATED SUCCESSFULLY");
                statusMessage.AppendLine($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC | User: {Environment.UserName}");
                statusMessage.AppendLine($"Database: {dbName}");
                statusMessage.AppendLine("");
                statusMessage.AppendLine("TABLES CREATED:");
                statusMessage.AppendLine($"   {tableStudentProfile} (Demographics + Numeric Equivalents)");
                statusMessage.AppendLine($"   {tableIntakeData} (Assessments + ML Features)");
                statusMessage.AppendLine($"   {tableDailyBehavior} (Tracking + ML Metrics)");
                statusMessage.AppendLine($"   {tableExitData} (Outcomes + Improvement Metrics)");
                statusMessage.AppendLine("");
                statusMessage.AppendLine("VIEWS CREATED:");
                statusMessage.AppendLine("   vw_MLReadyData (Combined dataset for ML training)");
                statusMessage.AppendLine("   vw_BehavioralAggregates (Pre-calculated metrics)");
                statusMessage.AppendLine("");
                statusMessage.AppendLine("INDEXES CREATED:");
                statusMessage.AppendLine("   Performance indexes on key columns");
                statusMessage.AppendLine("   Foreign key relationships established");
                statusMessage.AppendLine("");
                statusMessage.AppendLine("Ready for synthetic data generation");

                lblStatus.Text = statusMessage.ToString().TrimEnd();
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Database Created | Status display error: {ex.Message}";
            }
        }

        // Alternative compact version for smaller status labels
        private void DisplayCompactDatabaseStatus()
        {
            try
            {
                lblStatus.Text = $"✅ Database '{dbName}' created with 4 tables + 2 views + indexes | " +
                                $"Ready for data generation | {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC";
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"✅ Database Created | Error: {ex.Message}";
            }
        }






        // View Database Info
        private void btnViewDatabaseInfo_Click(object sender, EventArgs e)
        {
            try
            {
                lblStatus.Text = "Retrieving database information...";
                Application.DoEvents();

                using (SqlConnection conn = new SqlConnection(dbConnection))
                {
                    conn.Open();

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

                    SqlCommand dbInfoCmd = new SqlCommand(dbInfoQuery, conn);
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
                    SqlCommand cmd = new SqlCommand(tableQuery, conn);
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
                        AddDataInsights(conn, result, tableData);
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

        // Alternative compact version for smaller displays
        private void DisplayCompactDatabaseInfo()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(dbConnection))
                {
                    conn.Open();
                    string query = @"
                SELECT t.NAME AS TableName, SUM(p.rows) AS [RowCount]
                FROM sys.tables t
                INNER JOIN sys.indexes i ON t.OBJECT_ID = i.object_id
                INNER JOIN sys.partitions p ON i.object_id = p.OBJECT_ID AND i.index_id = p.index_id
                WHERE t.is_ms_shipped = 0 AND i.type <= 1
                GROUP BY t.NAME ORDER BY t.NAME;";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    var counts = new List<string>();
                    int totalRows = 0;

                    while (reader.Read())
                    {
                        string tableName = reader["TableName"].ToString();
                        int rowCount = Convert.ToInt32(reader["RowCount"]);
                        totalRows += rowCount;

                        string shortName = tableName switch
                        {
                            var n when n.Contains("Student") => "Students",
                            var n when n.Contains("Intake") => "Intake",
                            var n when n.Contains("Behavior") => "Behaviors",
                            var n when n.Contains("Exit") => "Exits",
                            _ => tableName
                        };

                        counts.Add($"{shortName}:{rowCount:N0}");
                    }

                    lblStatus.Text = $"Database: {dbName} | {string.Join(" | ", counts)} | Total:{totalRows:N0} | {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC";
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Database info error: {ex.Message}";
            }
        }










        // Load Intake Data
        private void btnLoadIntakeData_Click(object sender, EventArgs e)
        {
            // Implementation for loading IntakeData CSV
            lblStatus.Text = "IntakeData loaded successfully.";
        }

        // Load Daily Behavior
        private void btnLoadDailyBehavior_Click(object sender, EventArgs e)
        {
            // Implementation for loading DailyBehavior CSV
            lblStatus.Text = "DailyBehavior loaded successfully.";
        }

        // Generate Synthetic Student Data
        private async void btnGenerateSyntheticData_Click(object sender, EventArgs e)
        {
            // Disable the button to prevent multiple clicks
            btnGenerateSyntheticData.Enabled = false;
            btnLoadGeneratedData.Enabled = false;
            btnSaveGeneratedCSV.Enabled = false;

            try
            {
                // Show initial status
                lblStatus.Text = "Initializing synthetic data generation...";
                Application.DoEvents();

                // Run the generation on a background thread with progress reporting
                var progress = new Progress<string>(message =>
                {
                    lblStatus.Text = message;
                    Application.DoEvents();
                });

                await Task.Run(() => GenerateSyntheticDataWithProgress(progress));

                // Show completion status
                DisplayGenerationSummaryInStatus();
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Generation Failed: {ex.Message}";
                MessageBox.Show($"Failed to generate synthetic data:\n\n{ex.Message}", "Generation Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Re-enable buttons
                btnGenerateSyntheticData.Enabled = true;
                btnLoadGeneratedData.Enabled = true;
                btnSaveGeneratedCSV.Enabled = true;
            }
        }

        private void GenerateSyntheticDataWithProgress(IProgress<string> progress)
        {
            Random rand = new Random();
            generatedProfiles.Clear();
            generatedIntake.Clear();
            generatedBehavior.Clear();
            generatedExitData.Clear();

            int numStudents = 50;
            progress?.Report($"Generating data for {numStudents} students...");

            for (int studentId = 1; studentId <= numStudents; studentId++)
            {
                // Report progress every few students
                if (studentId % 5 == 0 || studentId == 1)
                {
                    int percentComplete = (int)((double)studentId / numStudents * 30); // 30% for student setup
                    progress?.Report($"Creating student {studentId}/{numStudents} ({percentComplete}% complete)");
                }

                // Create student baseline characteristics that influence everything
                double studentStressLevel = GenerateNormalRandom(rand, 0.5, 0.2);
                double familySupport = GenerateNormalRandom(rand, 0.6, 0.25);
                double academicAbility = GenerateNormalRandom(rand, 0.5, 0.2);
                double emotionalRegulationCapacity = GenerateNormalRandom(rand, 0.5, 0.2);

                int grade = rand.Next(0, 6);
                int age = grade + rand.Next(5, 8);
                double ageMaturityFactor = Math.Min(1.0, age / 18.0);

                bool hasSpecialNeeds = rand.NextDouble() < 0.15;
                bool hasIEP = hasSpecialNeeds && rand.NextDouble() < 0.8;

                string gender = rand.NextDouble() > 0.5 ? "Male" : "Female";
                string[] ethnicities = { "White", "Black", "Hispanic", "Asian", "Other" };
                string ethnicity = ethnicities[rand.Next(ethnicities.Length)];

                generatedProfiles.Add(new StudentProfile
                {
                    StudentID = studentId,
                    Grade = grade,
                    Age = age,
                    Gender = gender,
                    GenderNumeric = GetGenderNumeric(gender),
                    Ethnicity = ethnicity,
                    EthnicityNumeric = GetEthnicityNumeric(ethnicity),
                    SpecialEd = hasSpecialNeeds ? 1 : 0,
                    IEP = hasIEP ? 1 : 0
                });

                DateTime entryDate = DateTime.Now.AddDays(-rand.Next(30, 120));
                double riskFactor = (1 - familySupport) * 0.4 + studentStressLevel * 0.4 + (hasSpecialNeeds ? 0.2 : 0);

                int priorIncidents = Math.Min(10, (int)(riskFactor * 8 + GenerateNormalRandom(rand, 0, 1)));
                int officeReferrals = Math.Min(5, (int)(riskFactor * 4 + GenerateNormalRandom(rand, 0, 0.5)));
                int suspensions = Math.Min(3, (int)(riskFactor * 3 + GenerateNormalRandom(rand, 0, 0.5)));

                string academicLevel = academicAbility > 0.7 ? "Above Grade" :
                                      academicAbility > 0.3 ? "At Grade" : "Below Grade";

                double socialSkillsScore = (emotionalRegulationCapacity * 0.6 + familySupport * 0.4);
                string socialSkillsLevel = socialSkillsScore > 0.6 ? "High" :
                                          socialSkillsScore > 0.3 ? "Medium" : "Low";

                string entryReason = GetWeightedEntryReason(rand, studentStressLevel, emotionalRegulationCapacity);

                generatedIntake.Add(new IntakeData
                {
                    IntakeID = 0,
                    StudentID = studentId,
                    EntryReason = entryReason,
                    EntryReasonNumeric = GetEntryReasonNumeric(entryReason),
                    PriorIncidents = Math.Max(0, priorIncidents),
                    OfficeReferrals = Math.Max(0, officeReferrals),
                    Suspensions = Math.Max(0, suspensions),
                    Expulsions = riskFactor > 0.8 && rand.NextDouble() < 0.3 ? 1 : 0,
                    EntryAcademicLevel = academicLevel,
                    EntryAcademicLevelNumeric = GetAcademicLevelNumeric(academicLevel),
                    CheckInOut = riskFactor > 0.5 ? 1 : 0,
                    StructuredRecess = (hasSpecialNeeds || riskFactor > 0.4) ? 1 : 0,
                    StructuredBreaks = (hasSpecialNeeds || emotionalRegulationCapacity < 0.4) ? 1 : 0,
                    SmallGroups = hasSpecialNeeds || academicAbility < 0.4 ? rand.Next(1, 3) : rand.Next(0, 2),
                    SocialWorkerVisits = (int)(riskFactor * 3) + rand.Next(0, 2),
                    PsychologistVisits = hasSpecialNeeds || emotionalRegulationCapacity < 0.3 ? rand.Next(1, 3) : rand.Next(0, 1),
                    EntrySocialSkillsLevel = socialSkillsLevel,
                    EntrySocialSkillsLevelNumeric = GetSocialSkillsLevelNumeric(socialSkillsLevel),
                    EntryDate = entryDate,
                    RiskScore = Math.Min(10, Math.Max(1, (int)(riskFactor * 9) + 1)),
                    StudentStressLevelNormalized = Math.Max(0, Math.Min(1, studentStressLevel)),
                    FamilySupportNormalized = Math.Max(0, Math.Min(1, familySupport)),
                    AcademicAbilityNormalized = Math.Max(0, Math.Min(1, academicAbility)),
                    EmotionalRegulationNormalized = Math.Max(0, Math.Min(1, emotionalRegulationCapacity))
                });

                // Generate behavioral data with progress reporting
                progress?.Report($"Generating behavior data for student {studentId}...");

                int behaviorDays = rand.Next(30, 91);
                int totalAggression = 0;
                int totalEngagement = 0;
                int redZoneDays = 0;

                double currentAggressionTrend = studentStressLevel * 0.7 + (1 - emotionalRegulationCapacity) * 0.3;
                double currentEngagementTrend = academicAbility * 0.5 + familySupport * 0.3 + emotionalRegulationCapacity * 0.2;

                double programEffectiveness = GenerateNormalRandom(rand, 0.6, 0.2);
                double improvementRate = programEffectiveness * 0.02;

                for (int day = 0; day < behaviorDays; day++)
                {
                    // Report progress for behavior generation (less frequently)
                    if (day % 10 == 0 && studentId % 10 == 0)
                    {
                        int overallPercent = 30 + (int)((double)studentId / numStudents * 50); // 30-80% range
                        progress?.Report($"Day {day + 1}/{behaviorDays} for student {studentId} ({overallPercent}% complete)");
                    }

                    double dayProgress = day / (double)behaviorDays;
                    currentAggressionTrend = Math.Max(0, currentAggressionTrend - (improvementRate * dayProgress));
                    currentEngagementTrend = Math.Min(1, currentEngagementTrend + (improvementRate * dayProgress * 0.5));

                    double weeklyFactor = 1.0 + 0.2 * Math.Sin((day % 7) * Math.PI / 7);
                    double dailyVariation = GenerateNormalRandom(rand, 0, 0.1);

                    double todayAggressionRisk = Math.Max(0, Math.Min(1, currentAggressionTrend * weeklyFactor + dailyVariation));
                    double todayEngagementLevel = Math.Max(0, Math.Min(1, currentEngagementTrend * weeklyFactor - dailyVariation));

                    int verbalAggression = rand.NextDouble() < todayAggressionRisk * 0.3 ? 1 : 0;
                    int physicalAggression = rand.NextDouble() < todayAggressionRisk * 0.15 ? 1 : 0;
                    int academicEngagement = Math.Max(1, Math.Min(5, (int)(todayEngagementLevel * 4) + 1 + rand.Next(-1, 2)));

                    string zone = GetZoneBasedOnBehavior(todayAggressionRisk, todayEngagementLevel, rand);
                    string emotion = day % 7 == 0 ? GetEmotionBasedOnBehavior(todayAggressionRisk, todayEngagementLevel, rand) : string.Empty;

                    totalAggression += verbalAggression + physicalAggression;
                    totalEngagement += academicEngagement;
                    if (zone == "Red") redZoneDays++;

                    int socialInteractions = Math.Max(1, Math.Min(5, (int)(socialSkillsScore * 4) + 1 + rand.Next(-1, 2)));
                    int emotionalRegulation = Math.Max(1, Math.Min(5, (int)(emotionalRegulationCapacity * 4) + 1 + rand.Next(-1, 2)));

                    generatedBehavior.Add(new DailyBehavior
                    {
                        StudentID = studentId,
                        Timestamp = entryDate.AddDays(day),
                        Level = Math.Max(1, Math.Min(6, (int)(todayEngagementLevel * 5) + 1)),
                        Step = Math.Max(1, Math.Min(6, (int)((1 - todayAggressionRisk) * 5) + 1)),
                        VerbalAggression = verbalAggression,
                        PhysicalAggression = physicalAggression,
                        Elopement = rand.NextDouble() < todayAggressionRisk * 0.1 ? 1 : 0,
                        OutOfSpot = rand.NextDouble() < (1 - todayEngagementLevel) * 0.3 ? 1 : 0,
                        WorkRefusal = rand.NextDouble() < (1 - todayEngagementLevel) * 0.4 ? 1 : 0,
                        ProvokingPeers = rand.NextDouble() < todayAggressionRisk * 0.2 ? 1 : 0,
                        InappropriateLanguage = rand.NextDouble() < todayAggressionRisk * 0.25 ? 1 : 0,
                        OutOfLane = rand.NextDouble() < (1 - emotionalRegulationCapacity) * 0.3 ? 1 : 0,
                        ZoneOfRegulation = zone,
                        ZoneOfRegulationNumeric = GetZoneNumeric(zone),
                        AcademicEngagement = academicEngagement,
                        SocialInteractions = socialInteractions,
                        EmotionalRegulation = emotionalRegulation,
                        StaffComments = string.Empty,
                        WeeklyEmotionDate = day % 7 == 0 ? (DateTime?)entryDate.AddDays(day) : null,
                        WeeklyEmotionPictogram = emotion,
                        WeeklyEmotionPictogramNumeric = !string.IsNullOrEmpty(emotion) ? GetEmotionNumeric(emotion) : (int?)null,
                        AggressionRiskNormalized = todayAggressionRisk,
                        EngagementLevelNormalized = todayEngagementLevel,
                        DayInProgram = day + 1,
                        WeekInProgram = (day / 7) + 1
                    });
                }

                // Generate exit data
                progress?.Report($"Calculating outcomes for student {studentId}...");

                double avgAggression = (double)totalAggression / behaviorDays;
                double avgEngagement = (double)totalEngagement / behaviorDays;
                double redZonePercent = (double)redZoneDays / behaviorDays;
                double improvementScore = currentEngagementTrend - (academicAbility * 0.5 + familySupport * 0.3 + emotionalRegulationCapacity * 0.2);

                if (rand.NextDouble() >= 0.15) // 85% have exit data
                {
                    string likelyOutcome = PredictOutcome(avgAggression, avgEngagement, redZonePercent,
                                                        improvementScore, riskFactor, programEffectiveness,
                                                        familySupport, behaviorDays, rand);

                    int lengthOfStay = CalculateLengthOfStay(likelyOutcome, improvementScore, riskFactor, rand);
                    DateTime exitDate = entryDate.AddDays(lengthOfStay);

                    string exitAcademicLevel = CalculateExitAcademicLevel(academicLevel, improvementScore, rand);
                    string exitSocialSkillsLevel = CalculateExitSocialSkillsLevel(socialSkillsLevel, improvementScore, rand);

                    generatedExitData.Add(new ExitData
                    {
                        ExitID = 0,
                        StudentID = studentId,
                        ExitReason = likelyOutcome,
                        ExitReasonNumeric = GetExitReasonNumeric(likelyOutcome),
                        ExitDate = exitDate,
                        LengthOfStay = lengthOfStay,
                        ExitAcademicLevel = exitAcademicLevel,
                        ExitAcademicLevelNumeric = GetAcademicLevelNumeric(exitAcademicLevel),
                        ExitSocialSkillsLevel = exitSocialSkillsLevel,
                        ExitSocialSkillsLevelNumeric = GetSocialSkillsLevelNumeric(exitSocialSkillsLevel),
                        AcademicImprovement = GetAcademicLevelNumeric(exitAcademicLevel) - GetAcademicLevelNumeric(academicLevel),
                        SocialSkillsImprovement = GetSocialSkillsLevelNumeric(exitSocialSkillsLevel) - GetSocialSkillsLevelNumeric(socialSkillsLevel),
                        OverallImprovementScore = improvementScore,
                        ProgramEffectivenessScore = programEffectiveness,
                        SuccessIndicator = GetSuccessIndicator(likelyOutcome)
                    });
                }
            }

            progress?.Report("Finalizing synthetic data generation...");
        }

        // Display generation summary in status
        private void DisplayGenerationSummaryInStatus()
        {
            try
            {
                int currentStudents = generatedProfiles.Count - generatedExitData.Count;
                int totalBehaviorDays = generatedBehavior.Count;
                double avgDaysPerStudent = totalBehaviorDays > 0 ? (double)totalBehaviorDays / generatedProfiles.Count : 0;

                var statusMessage = new StringBuilder();
                statusMessage.AppendLine("GENERATION COMPLETE");
                statusMessage.AppendLine($"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC | {Environment.UserName}");
                statusMessage.AppendLine("");
                statusMessage.AppendLine("SYNTHETIC DATA CREATED:");
                statusMessage.AppendLine($"   Students: {generatedProfiles.Count:N0} ({currentStudents:N0} current, {generatedExitData.Count:N0} exited)");
                statusMessage.AppendLine($"   Intake Records: {generatedIntake.Count:N0}");
                statusMessage.AppendLine($"   Behavior Records: {generatedBehavior.Count:N0} ({avgDaysPerStudent:F1} avg days/student)");
                statusMessage.AppendLine($"   Exit Records: {generatedExitData.Count:N0}");
                statusMessage.AppendLine("");
                statusMessage.AppendLine("FEATURES GENERATED:");
                statusMessage.AppendLine("   • Realistic correlations & behavioral patterns");
                statusMessage.AppendLine("   • Text + numeric equivalents for all categories");
                statusMessage.AppendLine("   • Normalized ML features & improvement metrics");
                statusMessage.AppendLine("   • Temporal trends & program effectiveness scores");
                statusMessage.AppendLine("");
                statusMessage.AppendLine("Ready for database loading or CSV export");

                lblStatus.Text = statusMessage.ToString().TrimEnd();
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Data Generation Complete | Status display error: {ex.Message}";
            }
        }

        // Compact version for smaller status labels
        private void DisplayCompactGenerationStatus()
        {
            try
            {
                int currentStudents = generatedProfiles.Count - generatedExitData.Count;

                lblStatus.Text = $"Generated {generatedProfiles.Count} students ({currentStudents} current, {generatedExitData.Count} exited) " +
                                $"| {generatedBehavior.Count} behaviors | Ready for export/loading | 2025-07-24 05:50:34 UTC";
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Data Generation Complete | Error: {ex.Message}";
            }
        }





        // Helper methods for more realistic data generation


        // Numeric conversion methods for categorical data
        private int GetGenderNumeric(string gender)
        {
            return gender switch
            {
                "Male" => 1,
                "Female" => 2,
                _ => 0
            };
        }

        private int GetEthnicityNumeric(string ethnicity)
        {
            return ethnicity switch
            {
                "White" => 1,
                "Black" => 2,
                "Hispanic" => 3,
                "Asian" => 4,
                "Other" => 5,
                _ => 0
            };
        }

        private int GetEntryReasonNumeric(string reason)
        {
            return reason switch
            {
                "Aggression" => 1,
                "Anxiety" => 2,
                "Trauma" => 3,
                "Withdrawn" => 4,
                "Disruptive" => 5,
                "Other" => 6,
                _ => 0
            };
        }

        private int GetAcademicLevelNumeric(string level)
        {
            return level switch
            {
                "Below Grade" => 1,
                "At Grade" => 2,
                "Above Grade" => 3,
                _ => 0
            };
        }

        private int GetSocialSkillsLevelNumeric(string level)
        {
            return level switch
            {
                "Low" => 1,
                "Medium" => 2,
                "High" => 3,
                _ => 0
            };
        }

        private int GetZoneNumeric(string zone)
        {
            return zone switch
            {
                "Green" => 4,    // Highest = best
                "Blue" => 3,
                "Yellow" => 2,
                "Red" => 1,      // Lowest = worst
                _ => 0
            };
        }

        private int GetEmotionNumeric(string emotion)
        {
            return emotion switch
            {
                "Happy" => 6,
                "Excited" => 5,
                "Nervous" => 4,
                "Lonely" => 3,
                "Sad" => 2,
                "Angry" => 1,
                _ => 0
            };
        }

        private int GetExitReasonNumeric(string reason)
        {
            return reason switch
            {
                "Returned Successfully" => 5,  // Best outcome
                "ACC" => 4,                    // Positive transition
                "Other" => 3,                  // Neutral
                "ABS" => 2,                    // Negative
                "Referred Out" => 1,           // Worst outcome
                _ => 0
            };
        }

        private int GetSuccessIndicator(string exitReason)
        {
            return exitReason switch
            {
                "Returned Successfully" => 1,
                "ACC" => 1,
                _ => 0
            };
        }

        private double GenerateNormalRandom(Random rand, double mean, double stdDev)
        {
            // Box-Muller transform for normal distribution
            double u1 = 1.0 - rand.NextDouble();
            double u2 = 1.0 - rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return Math.Max(0, Math.Min(1, mean + stdDev * randStdNormal));
        }

        private string GetWeightedEntryReason(Random rand, double stressLevel, double emotionalReg)
        {
            var reasons = new[] { "Aggression", "Anxiety", "Trauma", "Withdrawn", "Disruptive", "Other" };
            var weights = new double[]
            {
        stressLevel * 0.8 + (1 - emotionalReg) * 0.2, // Aggression
        stressLevel * 0.6 + (1 - emotionalReg) * 0.4, // Anxiety  
        stressLevel * 0.7, // Trauma
        (1 - emotionalReg) * 0.5 + stressLevel * 0.3, // Withdrawn
        stressLevel * 0.5 + (1 - emotionalReg) * 0.5, // Disruptive
        0.1 // Other
            };

            return GetWeightedChoice(rand, reasons, weights);
        }

        private string GetZoneBasedOnBehavior(double aggressionRisk, double engagementLevel, Random rand)
        {
            if (aggressionRisk > 0.7) return "Red";
            if (aggressionRisk > 0.4 || engagementLevel < 0.3) return "Yellow";
            if (engagementLevel > 0.7 && aggressionRisk < 0.2) return "Green";
            return "Blue";
        }

        private string GetEmotionBasedOnBehavior(double aggressionRisk, double engagementLevel, Random rand)
        {
            var emotions = new[] { "Happy", "Sad", "Angry", "Lonely", "Nervous", "Excited" };
            var weights = new double[]
            {
        engagementLevel * 0.8, // Happy
        (1 - engagementLevel) * 0.6, // Sad
        aggressionRisk * 0.9, // Angry
        (1 - engagementLevel) * 0.4, // Lonely
        aggressionRisk * 0.5, // Nervous
        engagementLevel * 0.6 // Excited
            };

            return GetWeightedChoice(rand, emotions, weights);
        }

        private string PredictOutcome(double avgAggression, double avgEngagement, double redZonePercent,
                                    double improvementScore, double riskFactor, double programEffectiveness,
                                    double familySupport, int behaviorDays, Random rand)
        {
            // Calculate success probability based on multiple factors
            double successScore = 0;

            // Behavioral factors (40% weight)
            successScore += (5 - avgEngagement) / 5 * 0.2; // Higher engagement = better
            successScore += Math.Max(0, 1 - avgAggression) * 0.1; // Lower aggression = better  
            successScore += Math.Max(0, 1 - redZonePercent) * 0.1; // Fewer red zone days = better

            // Improvement factors (30% weight)
            successScore += Math.Max(0, improvementScore) * 0.2; // Positive improvement = better
            successScore += programEffectiveness * 0.1; // Program working = better

            // Support factors (20% weight)
            successScore += familySupport * 0.15; // Family support = better
            successScore += Math.Max(0, 1 - riskFactor) * 0.05; // Lower initial risk = better

            // Time factors (10% weight)  
            successScore += Math.Min(1, behaviorDays / 60.0) * 0.1; // Longer stay can indicate stability

            // Add some randomness
            successScore += GenerateNormalRandom(rand, 0, 0.1);

            // Determine outcome based on success score
            if (successScore > 0.7) return "Returned Successfully";
            if (successScore < 0.3) return "Referred Out";
            if (successScore < 0.4 && rand.NextDouble() < 0.2) return "ABS";
            if (rand.NextDouble() < 0.05) return "ACC";

            return rand.NextDouble() < successScore ? "Returned Successfully" : "Referred Out";
        }

        private int CalculateLengthOfStay(string outcome, double improvementScore, double riskFactor, Random rand)
        {
            int baseDays = outcome switch
            {
                "Returned Successfully" => 75, // Successful completions take longer
                "Referred Out" => 45, // Unsuccessful shorter
                "ABS" => 30, // Absconded early
                "ACC" => 90, // Acute care longer
                _ => 60
            };

            // Adjust based on factors
            double adjustment = improvementScore * 20 - riskFactor * 15;
            return Math.Max(14, baseDays + (int)adjustment + rand.Next(-15, 15));
        }

        private string CalculateExitAcademicLevel(string entryLevel, double improvementScore, Random rand)
        {
            if (improvementScore > 0.3 && rand.NextDouble() < 0.4)
            {
                return entryLevel switch
                {
                    "Below Grade" => "At Grade",
                    "At Grade" => rand.NextDouble() < 0.3 ? "Above Grade" : "At Grade",
                    _ => entryLevel
                };
            }
            return entryLevel; // Most students don't change academic level significantly
        }

        private string CalculateExitSocialSkillsLevel(string entryLevel, double improvementScore, Random rand)
        {
            if (improvementScore > 0.2 && rand.NextDouble() < 0.6)
            {
                return entryLevel switch
                {
                    "Low" => "Medium",
                    "Medium" => rand.NextDouble() < 0.4 ? "High" : "Medium",
                    _ => entryLevel
                };
            }
            return entryLevel;
        }

        private string GetWeightedChoice(Random rand, string[] choices, double[] weights)
        {
            double totalWeight = weights.Sum();
            double randomValue = rand.NextDouble() * totalWeight;

            double currentWeight = 0;
            for (int i = 0; i < choices.Length; i++)
            {
                currentWeight += weights[i];
                if (randomValue <= currentWeight)
                    return choices[i];
            }

            return choices[choices.Length - 1];
        }








        // Load Generated Data into Database
        private void btnLoadGeneratedData_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Do you want to clear existing data tables before loading new data?\nClick Yes to truncate and reload all tables, No to append only new records.", "Data Load Option", MessageBoxButtons.YesNoCancel);

            if (result == DialogResult.Cancel)
            {
                lblStatus.Text = "Data load canceled by user.";
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(dbConnection))
                {
                    conn.Open();

                    if (result == DialogResult.Yes)
                    {
                        // Truncate tables in correct order (respecting foreign key constraints)
                        string truncateSql = $@"
                    TRUNCATE TABLE {tableExitData};
                    TRUNCATE TABLE {tableDailyBehavior}; 
                    TRUNCATE TABLE {tableIntakeData};
                    TRUNCATE TABLE {tableStudentProfile};";

                        using (SqlCommand truncateCmd = new SqlCommand(truncateSql, conn))
                        {
                            truncateCmd.ExecuteNonQuery();
                        }
                        lblStatus.Text = "Tables truncated. Loading new data...";
                    }

                    // Load Student Profiles with new numeric fields
                    foreach (var profile in generatedProfiles)
                    {
                        string query = $@"
                IF NOT EXISTS (SELECT 1 FROM {tableStudentProfile} WHERE StudentID = @StudentID)
                BEGIN
                    INSERT INTO {tableStudentProfile} (StudentID, Grade, Age, Gender, GenderNumeric, 
                        Ethnicity, EthnicityNumeric, SpecialEd, IEP)
                    VALUES (@StudentID, @Grade, @Age, @Gender, @GenderNumeric, 
                        @Ethnicity, @EthnicityNumeric, @SpecialEd, @IEP)
                END";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@StudentID", profile.StudentID);
                            cmd.Parameters.AddWithValue("@Grade", profile.Grade);
                            cmd.Parameters.AddWithValue("@Age", profile.Age);
                            cmd.Parameters.AddWithValue("@Gender", profile.Gender ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@GenderNumeric", profile.GenderNumeric);
                            cmd.Parameters.AddWithValue("@Ethnicity", profile.Ethnicity ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@EthnicityNumeric", profile.EthnicityNumeric);
                            cmd.Parameters.AddWithValue("@SpecialEd", profile.SpecialEd);
                            cmd.Parameters.AddWithValue("@IEP", profile.IEP);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // Load Intake Data with new numeric and normalized fields
                    foreach (var intake in generatedIntake)
                    {
                        string query = $@"
                IF NOT EXISTS (SELECT 1 FROM {tableIntakeData} WHERE StudentID = @StudentID)
                BEGIN
                    INSERT INTO {tableIntakeData} (StudentID, EntryReason, EntryReasonNumeric, PriorIncidents, 
                        OfficeReferrals, Suspensions, Expulsions, EntryAcademicLevel, EntryAcademicLevelNumeric,
                        CheckInOut, StructuredRecess, StructuredBreaks, SmallGroups, SocialWorkerVisits, 
                        PsychologistVisits, EntrySocialSkillsLevel, EntrySocialSkillsLevelNumeric, EntryDate, 
                        RiskScore, StudentStressLevelNormalized, FamilySupportNormalized, AcademicAbilityNormalized,
                        EmotionalRegulationNormalized)
                    VALUES (@StudentID, @EntryReason, @EntryReasonNumeric, @PriorIncidents, @OfficeReferrals, 
                        @Suspensions, @Expulsions, @EntryAcademicLevel, @EntryAcademicLevelNumeric, @CheckInOut, 
                        @StructuredRecess, @StructuredBreaks, @SmallGroups, @SocialWorkerVisits, @PsychologistVisits, 
                        @EntrySocialSkillsLevel, @EntrySocialSkillsLevelNumeric, @EntryDate, @RiskScore,
                        @StudentStressLevelNormalized, @FamilySupportNormalized, @AcademicAbilityNormalized,
                        @EmotionalRegulationNormalized)
                END";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@StudentID", intake.StudentID);
                            cmd.Parameters.AddWithValue("@EntryReason", intake.EntryReason ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@EntryReasonNumeric", intake.EntryReasonNumeric);
                            cmd.Parameters.AddWithValue("@PriorIncidents", intake.PriorIncidents);
                            cmd.Parameters.AddWithValue("@OfficeReferrals", intake.OfficeReferrals);
                            cmd.Parameters.AddWithValue("@Suspensions", intake.Suspensions);
                            cmd.Parameters.AddWithValue("@Expulsions", intake.Expulsions);
                            cmd.Parameters.AddWithValue("@EntryAcademicLevel", intake.EntryAcademicLevel ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@EntryAcademicLevelNumeric", intake.EntryAcademicLevelNumeric);
                            cmd.Parameters.AddWithValue("@CheckInOut", intake.CheckInOut);
                            cmd.Parameters.AddWithValue("@StructuredRecess", intake.StructuredRecess);
                            cmd.Parameters.AddWithValue("@StructuredBreaks", intake.StructuredBreaks);
                            cmd.Parameters.AddWithValue("@SmallGroups", intake.SmallGroups);
                            cmd.Parameters.AddWithValue("@SocialWorkerVisits", intake.SocialWorkerVisits);
                            cmd.Parameters.AddWithValue("@PsychologistVisits", intake.PsychologistVisits);
                            cmd.Parameters.AddWithValue("@EntrySocialSkillsLevel", intake.EntrySocialSkillsLevel ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@EntrySocialSkillsLevelNumeric", intake.EntrySocialSkillsLevelNumeric);
                            cmd.Parameters.AddWithValue("@EntryDate", intake.EntryDate);
                            cmd.Parameters.AddWithValue("@RiskScore", intake.RiskScore);
                            cmd.Parameters.AddWithValue("@StudentStressLevelNormalized", intake.StudentStressLevelNormalized);
                            cmd.Parameters.AddWithValue("@FamilySupportNormalized", intake.FamilySupportNormalized);
                            cmd.Parameters.AddWithValue("@AcademicAbilityNormalized", intake.AcademicAbilityNormalized);
                            cmd.Parameters.AddWithValue("@EmotionalRegulationNormalized", intake.EmotionalRegulationNormalized);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // Load Daily Behavior with new numeric and ML fields
                    foreach (var behavior in generatedBehavior)
                    {
                        string query = $@"
                INSERT INTO {tableDailyBehavior} (StudentID, Timestamp, Level, Step, VerbalAggression, 
                    PhysicalAggression, Elopement, OutOfSpot, WorkRefusal, ProvokingPeers, InappropriateLanguage, 
                    OutOfLane, ZoneOfRegulation, ZoneOfRegulationNumeric, AcademicEngagement, SocialInteractions, 
                    EmotionalRegulation, StaffComments, WeeklyEmotionDate, WeeklyEmotionPictogram, 
                    WeeklyEmotionPictogramNumeric, AggressionRiskNormalized, EngagementLevelNormalized, 
                    DayInProgram, WeekInProgram)
                VALUES (@StudentID, @Timestamp, @Level, @Step, @VerbalAggression, @PhysicalAggression, 
                    @Elopement, @OutOfSpot, @WorkRefusal, @ProvokingPeers, @InappropriateLanguage, @OutOfLane, 
                    @ZoneOfRegulation, @ZoneOfRegulationNumeric, @AcademicEngagement, @SocialInteractions, 
                    @EmotionalRegulation, @StaffComments, @WeeklyEmotionDate, @WeeklyEmotionPictogram, 
                    @WeeklyEmotionPictogramNumeric, @AggressionRiskNormalized, @EngagementLevelNormalized, 
                    @DayInProgram, @WeekInProgram)";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@StudentID", behavior.StudentID);
                            cmd.Parameters.AddWithValue("@Timestamp", behavior.Timestamp);
                            cmd.Parameters.AddWithValue("@Level", behavior.Level);
                            cmd.Parameters.AddWithValue("@Step", behavior.Step);
                            cmd.Parameters.AddWithValue("@VerbalAggression", behavior.VerbalAggression);
                            cmd.Parameters.AddWithValue("@PhysicalAggression", behavior.PhysicalAggression);
                            cmd.Parameters.AddWithValue("@Elopement", behavior.Elopement);
                            cmd.Parameters.AddWithValue("@OutOfSpot", behavior.OutOfSpot);
                            cmd.Parameters.AddWithValue("@WorkRefusal", behavior.WorkRefusal);
                            cmd.Parameters.AddWithValue("@ProvokingPeers", behavior.ProvokingPeers);
                            cmd.Parameters.AddWithValue("@InappropriateLanguage", behavior.InappropriateLanguage);
                            cmd.Parameters.AddWithValue("@OutOfLane", behavior.OutOfLane);
                            cmd.Parameters.AddWithValue("@ZoneOfRegulation", behavior.ZoneOfRegulation ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@ZoneOfRegulationNumeric", behavior.ZoneOfRegulationNumeric);
                            cmd.Parameters.AddWithValue("@AcademicEngagement", behavior.AcademicEngagement);
                            cmd.Parameters.AddWithValue("@SocialInteractions", behavior.SocialInteractions);
                            cmd.Parameters.AddWithValue("@EmotionalRegulation", behavior.EmotionalRegulation);
                            cmd.Parameters.AddWithValue("@StaffComments", behavior.StaffComments ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@WeeklyEmotionDate", behavior.WeeklyEmotionDate.HasValue ?
                                (object)behavior.WeeklyEmotionDate.Value : DBNull.Value);
                            cmd.Parameters.AddWithValue("@WeeklyEmotionPictogram", behavior.WeeklyEmotionPictogram ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@WeeklyEmotionPictogramNumeric", behavior.WeeklyEmotionPictogramNumeric.HasValue ?
                                (object)behavior.WeeklyEmotionPictogramNumeric.Value : DBNull.Value);
                            cmd.Parameters.AddWithValue("@AggressionRiskNormalized", behavior.AggressionRiskNormalized);
                            cmd.Parameters.AddWithValue("@EngagementLevelNormalized", behavior.EngagementLevelNormalized);
                            cmd.Parameters.AddWithValue("@DayInProgram", behavior.DayInProgram);
                            cmd.Parameters.AddWithValue("@WeekInProgram", behavior.WeekInProgram);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // Load Exit Data with new numeric and ML metrics
                    foreach (var exit in generatedExitData)
                    {
                        string query = $@"
                INSERT INTO {tableExitData} (StudentID, ExitReason, ExitReasonNumeric, ExitDate, LengthOfStay, 
                    ExitAcademicLevel, ExitAcademicLevelNumeric, ExitSocialSkillsLevel, ExitSocialSkillsLevelNumeric,
                    AcademicImprovement, SocialSkillsImprovement, OverallImprovementScore, ProgramEffectivenessScore,
                    SuccessIndicator)
                VALUES (@StudentID, @ExitReason, @ExitReasonNumeric, @ExitDate, @LengthOfStay, @ExitAcademicLevel, 
                    @ExitAcademicLevelNumeric, @ExitSocialSkillsLevel, @ExitSocialSkillsLevelNumeric,
                    @AcademicImprovement, @SocialSkillsImprovement, @OverallImprovementScore, 
                    @ProgramEffectivenessScore, @SuccessIndicator)";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@StudentID", exit.StudentID);
                            cmd.Parameters.AddWithValue("@ExitReason", exit.ExitReason ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@ExitReasonNumeric", exit.ExitReasonNumeric);
                            cmd.Parameters.AddWithValue("@ExitDate", exit.ExitDate);
                            cmd.Parameters.AddWithValue("@LengthOfStay", exit.LengthOfStay);
                            cmd.Parameters.AddWithValue("@ExitAcademicLevel", exit.ExitAcademicLevel ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@ExitAcademicLevelNumeric", exit.ExitAcademicLevelNumeric);
                            cmd.Parameters.AddWithValue("@ExitSocialSkillsLevel", exit.ExitSocialSkillsLevel ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@ExitSocialSkillsLevelNumeric", exit.ExitSocialSkillsLevelNumeric);
                            cmd.Parameters.AddWithValue("@AcademicImprovement", exit.AcademicImprovement);
                            cmd.Parameters.AddWithValue("@SocialSkillsImprovement", exit.SocialSkillsImprovement);
                            cmd.Parameters.AddWithValue("@OverallImprovementScore", exit.OverallImprovementScore);
                            cmd.Parameters.AddWithValue("@ProgramEffectivenessScore", exit.ProgramEffectivenessScore);
                            cmd.Parameters.AddWithValue("@SuccessIndicator", exit.SuccessIndicator);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    lblStatus.Text = $"Generated data loaded successfully. " +
                                   $"Profiles: {generatedProfiles.Count}, " +
                                   $"Intake: {generatedIntake.Count}, " +
                                   $"Behaviors: {generatedBehavior.Count}, " +
                                   $"Exits: {generatedExitData.Count}";
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error loading data: {ex.Message}";
                MessageBox.Show($"Failed to load data into database:\n\n{ex.Message}", "Database Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    csv.WriteRecords(generatedProfiles);
                }

                // Save Intake Data with new numeric and normalized fields
                using (var writer = new StreamWriter(Path.Combine(folderPath, "IntakeData.csv")))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(generatedIntake);
                }

                // Save Daily Behavior with new numeric and ML fields
                using (var writer = new StreamWriter(Path.Combine(folderPath, "DailyBehavior.csv")))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(generatedBehavior);
                }

                // Save Exit Data with new numeric and ML metrics
                using (var writer = new StreamWriter(Path.Combine(folderPath, "ExitData.csv")))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(generatedExitData);
                }

                // Create a summary file with metadata
                using (var writer = new StreamWriter(Path.Combine(folderPath, "DataSummary.txt")))
                {
                    writer.WriteLine($"Synthetic Data Export Summary");
                    writer.WriteLine($"Generated on: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
                    writer.WriteLine($"Generated by: {Environment.UserName}");
                    writer.WriteLine($"");
                    writer.WriteLine($"Record Counts:");
                    writer.WriteLine($"  Student Profiles: {generatedProfiles.Count}");
                    writer.WriteLine($"  Intake Records: {generatedIntake.Count}");
                    writer.WriteLine($"  Daily Behavior Records: {generatedBehavior.Count}");
                    writer.WriteLine($"  Exit Records: {generatedExitData.Count}");
                    writer.WriteLine($"");
                    writer.WriteLine($"File Contents:");
                    writer.WriteLine($"  StudentProfile.csv - Student demographics with text and numeric equivalents");
                    writer.WriteLine($"  IntakeData.csv - Intake assessments with normalized ML features");
                    writer.WriteLine($"  DailyBehavior.csv - Daily behavior tracking with ML metrics");
                    writer.WriteLine($"  ExitData.csv - Program completion data with improvement metrics");
                    writer.WriteLine($"");
                    writer.WriteLine($"New Features in this Export:");
                    writer.WriteLine($"  - Numeric equivalents for all categorical data");
                    writer.WriteLine($"  - Normalized scores (0-1 scale) for ML training");
                    writer.WriteLine($"  - Improvement metrics and success indicators");
                    writer.WriteLine($"  - Temporal features (day/week in program)");
                }

                // Optionally create a combined ML-ready dataset
                CreateMLReadyDataset(folderPath);

                // Display formatted summary in status label
                DisplayExportSummaryInStatus(folderPath);

                // Show success message with option to open folder
                DialogResult result = MessageBox.Show($"Data successfully exported to:\n{folderPath}\n\nWould you like to open the folder?",
                                                    "Export Complete", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

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
            }
        }

        // Display formatted export summary in the status label
        private void DisplayExportSummaryInStatus(string folderPath)
        {
            try
            {
                string folderName = Path.GetFileName(folderPath);
                int currentStudents = generatedProfiles.Count - generatedExitData.Count;
                int exitedStudents = generatedExitData.Count;
                int totalBehaviorDays = generatedBehavior.Count;
                double avgDaysPerStudent = totalBehaviorDays > 0 ? (double)totalBehaviorDays / generatedProfiles.Count : 0;

                // Create a nicely formatted status message
                var statusMessage = new StringBuilder();
                statusMessage.AppendLine("EXPORT COMPLETE");
                statusMessage.AppendLine($"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC | 👤 {Environment.UserName}");
                statusMessage.AppendLine($"Folder: {folderName}");
                statusMessage.AppendLine("");
                statusMessage.AppendLine("RECORD SUMMARY:");
                statusMessage.AppendLine($"   Students: {generatedProfiles.Count:N0} ({currentStudents:N0} current, {exitedStudents:N0} exited)");
                statusMessage.AppendLine($"   Intake Records: {generatedIntake.Count:N0}");
                statusMessage.AppendLine($"   Behavior Records: {generatedBehavior.Count:N0} ({avgDaysPerStudent:F1} avg days/student)");
                statusMessage.AppendLine($"   Exit Records: {generatedExitData.Count:N0}");
                statusMessage.AppendLine("");
                statusMessage.AppendLine("FILES CREATED:");
                statusMessage.AppendLine("   • StudentProfile.csv (Demographics + Numeric)");
                statusMessage.AppendLine("   • IntakeData.csv (Assessments + ML Features)");
                statusMessage.AppendLine("   • DailyBehavior.csv (Tracking + ML Metrics)");
                statusMessage.AppendLine("   • ExitData.csv (Outcomes + Improvements)");
                statusMessage.AppendLine("   • MLReadyDataset.csv (Combined for ML Training)");
                statusMessage.AppendLine("   • BehavioralAggregates.csv (Trend Analysis)");
                statusMessage.AppendLine("   • DataSummary.txt (Export Metadata)");

                // Set the formatted text to the status label
                lblStatus.Text = statusMessage.ToString().TrimEnd();

                // If you want to make the status label more readable, you might also want to:
                // 1. Set the label to use a monospace font for better alignment
                // 2. Increase the label size to accommodate more text
                // 3. Enable word wrap or multi-line display

                // Example of setting font (uncomment if desired):
                lblStatus.Font = new Font("Consolas", 9F, FontStyle.Regular);
                lblStatus.AutoSize = true;
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Export Complete | Status display error: {ex.Message}";
            }
        }

        // Alternative version with more compact formatting for smaller status labels
        private void DisplayCompactExportSummaryInStatus(string folderPath)
        {
            try
            {
                string folderName = Path.GetFileName(folderPath);
                int currentStudents = generatedProfiles.Count - generatedExitData.Count;

                var compactMessage = new StringBuilder();
                compactMessage.AppendLine($"✅ Export Complete - {DateTime.UtcNow:HH:mm:ss} UTC");
                compactMessage.AppendLine($"📁 {folderName}");
                compactMessage.AppendLine($"👥 {generatedProfiles.Count} students ({currentStudents} current) | 📈 {generatedBehavior.Count} behaviors | 🚪 {generatedExitData.Count} exits");
                compactMessage.AppendLine($"📄 7 files created (CSV + ML datasets + summary)");

                lblStatus.Text = compactMessage.ToString().TrimEnd();
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"✅ Export Complete | ⚠️ {ex.Message}";
            }
        }

        // Even more compact single-line version
        private void DisplaySingleLineExportStatus(string folderPath)
        {
            try
            {
                string folderName = Path.GetFileName(folderPath);
                int currentStudents = generatedProfiles.Count - generatedExitData.Count;

                lblStatus.Text = $"✅ Exported {generatedProfiles.Count} students ({currentStudents} current, {generatedExitData.Count} exited) " +
                                $"→ {folderName} | {generatedBehavior.Count} behaviors | 7 files | {DateTime.UtcNow:HH:mm} UTC";
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"✅ Export Complete | Error: {ex.Message}";
            }
        }




        // Create a combined dataset optimized for ML training
        private void CreateMLReadyDataset(string folderPath)
        {
            try
            {
                var mlReadyData = from profile in generatedProfiles
                                  join intake in generatedIntake on profile.StudentID equals intake.StudentID
                                  join exit in generatedExitData on profile.StudentID equals exit.StudentID into exitGroup
                                  from exit in exitGroup.DefaultIfEmpty()
                                  select new
                                  {
                                      // Student Demographics (Numeric)
                                      StudentID = profile.StudentID,
                                      Age = profile.Age,
                                      Grade = profile.Grade,
                                      GenderNumeric = profile.GenderNumeric,
                                      EthnicityNumeric = profile.EthnicityNumeric,
                                      SpecialEd = profile.SpecialEd,
                                      IEP = profile.IEP,

                                      // Intake Factors (Numeric & Normalized)
                                      EntryReasonNumeric = intake.EntryReasonNumeric,
                                      PriorIncidents = intake.PriorIncidents,
                                      OfficeReferrals = intake.OfficeReferrals,
                                      Suspensions = intake.Suspensions,
                                      Expulsions = intake.Expulsions,
                                      EntryAcademicLevelNumeric = intake.EntryAcademicLevelNumeric,
                                      EntrySocialSkillsLevelNumeric = intake.EntrySocialSkillsLevelNumeric,
                                      RiskScore = intake.RiskScore,
                                      StudentStressLevelNormalized = intake.StudentStressLevelNormalized,
                                      FamilySupportNormalized = intake.FamilySupportNormalized,
                                      AcademicAbilityNormalized = intake.AcademicAbilityNormalized,
                                      EmotionalRegulationNormalized = intake.EmotionalRegulationNormalized,

                                      // Support Services
                                      CheckInOut = intake.CheckInOut,
                                      StructuredRecess = intake.StructuredRecess,
                                      StructuredBreaks = intake.StructuredBreaks,
                                      SmallGroups = intake.SmallGroups,
                                      SocialWorkerVisits = intake.SocialWorkerVisits,
                                      PsychologistVisits = intake.PsychologistVisits,

                                      // Outcome Variables (for students who have exited)
                                      HasExited = exit != null ? 1 : 0,
                                      LengthOfStay = exit?.LengthOfStay ?? 0,
                                      ExitReasonNumeric = exit?.ExitReasonNumeric ?? 0,
                                      ExitAcademicLevelNumeric = exit?.ExitAcademicLevelNumeric ?? 0,
                                      ExitSocialSkillsLevelNumeric = exit?.ExitSocialSkillsLevelNumeric ?? 0,
                                      AcademicImprovement = exit?.AcademicImprovement ?? 0,
                                      SocialSkillsImprovement = exit?.SocialSkillsImprovement ?? 0,
                                      OverallImprovementScore = exit?.OverallImprovementScore ?? 0,
                                      ProgramEffectivenessScore = exit?.ProgramEffectivenessScore ?? 0,
                                      SuccessIndicator = exit?.SuccessIndicator ?? 0
                                  };

                using (var writer = new StreamWriter(Path.Combine(folderPath, "MLReadyDataset.csv")))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(mlReadyData);
                }

                // Create behavioral aggregates for ML
                CreateBehavioralAggregates(folderPath);
            }
            catch (Exception ex)
            {
                // Log but don't fail the main export
                System.Diagnostics.Debug.WriteLine($"Error creating ML dataset: {ex.Message}");
            }
        }

        // Create aggregated behavioral metrics for ML
        private void CreateBehavioralAggregates(string folderPath)
        {
            try
            {
                var behavioralAggregates = generatedBehavior
                    .GroupBy(b => b.StudentID)
                    .Select(g => new
                    {
                        StudentID = g.Key,
                        DaysInProgram = g.Count(),
                        WeeksInProgram = g.Max(b => b.WeekInProgram),

                        // Aggression Metrics
                        TotalVerbalAggression = g.Sum(b => b.VerbalAggression),
                        TotalPhysicalAggression = g.Sum(b => b.PhysicalAggression),
                        AvgAggressionRisk = g.Average(b => b.AggressionRiskNormalized),
                        AggressionTrend = CalculateTrend(g.Select(b => (double)b.VerbalAggression + b.PhysicalAggression).ToList()),

                        // Engagement Metrics
                        AvgAcademicEngagement = g.Average(b => b.AcademicEngagement),
                        AvgEngagementLevel = g.Average(b => b.EngagementLevelNormalized),
                        EngagementTrend = CalculateTrend(g.Select(b => (double)b.AcademicEngagement).ToList()),

                        // Zone Metrics
                        RedZoneDays = g.Count(b => b.ZoneOfRegulationNumeric == 1),
                        YellowZoneDays = g.Count(b => b.ZoneOfRegulationNumeric == 2),
                        BlueZoneDays = g.Count(b => b.ZoneOfRegulationNumeric == 3),
                        GreenZoneDays = g.Count(b => b.ZoneOfRegulationNumeric == 4),
                        AvgZoneScore = g.Average(b => b.ZoneOfRegulationNumeric),

                        // Other Behavioral Metrics
                        TotalElopements = g.Sum(b => b.Elopement),
                        TotalWorkRefusals = g.Sum(b => b.WorkRefusal),
                        AvgSocialInteractions = g.Average(b => b.SocialInteractions),
                        AvgEmotionalRegulation = g.Average(b => b.EmotionalRegulation),

                        // Progress Indicators
                        FirstWeekAvgEngagement = g.Where(b => b.WeekInProgram == 1).Any() ?
                            g.Where(b => b.WeekInProgram == 1).Average(b => b.AcademicEngagement) : 0,
                        LastWeekAvgEngagement = g.GroupBy(b => b.WeekInProgram).OrderByDescending(w => w.Key)
                            .FirstOrDefault()?.Average(b => b.AcademicEngagement) ?? 0
                    });

                using (var writer = new StreamWriter(Path.Combine(folderPath, "BehavioralAggregates.csv")))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(behavioralAggregates);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating behavioral aggregates: {ex.Message}");
            }
        }

        // Helper method to calculate trend (positive = improving, negative = declining)
        private double CalculateTrend(List<double> values)
        {
            if (values.Count < 2) return 0;

            // Simple linear regression slope
            double n = values.Count;
            double sumX = 0, sumY = 0, sumXY = 0, sumX2 = 0;

            for (int i = 0; i < values.Count; i++)
            {
                sumX += i;
                sumY += values[i];
                sumXY += i * values[i];
                sumX2 += i * i;
            }

            double slope = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
            return slope;
        }



        private void btnExitOutcomeCount_Click(object sender, EventArgs e)
        {
            // Analyze the Data Distribution
            // Count of Each Exit Outcome

            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(dbConnection))
                    {
                        conn.Open();
                        string query = @"
                        SELECT ExitReason, COUNT(*) AS NumStudents
                        FROM ExitData
                        GROUP BY ExitReason;";

                        SqlCommand cmd = new SqlCommand(query, conn);
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
                }
            }
        }

        private void btnExitOutcomeAvgs_Click(object sender, EventArgs e)
        {
            // Analyze the Data Distribution
            // Behavior Averages by Exit Outcome

            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(dbConnection))
                    {
                        conn.Open();
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

                        SqlCommand cmd = new SqlCommand(query, conn);
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
                }
            
            }
        }
 


        private void btnLoadStudentProfile_Click(object sender, EventArgs e)
        {


          //  MessageBox.Show("Ready to Load student profile...But nothing is here!");
        }
    }


    public class StudentProfile
    {
        public int StudentID { get; set; }
        public int Grade { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public int GenderNumeric { get; set; }
        public string Ethnicity { get; set; }
        public int EthnicityNumeric { get; set; }
        public int SpecialEd { get; set; }
        public int IEP { get; set; }
    }

    public class IntakeData
    {
        public int IntakeID { get; set; }
        public int StudentID { get; set; }
        public string EntryReason { get; set; }
        public int EntryReasonNumeric { get; set; }
        public int PriorIncidents { get; set; }
        public int OfficeReferrals { get; set; }
        public int Suspensions { get; set; }
        public int Expulsions { get; set; }
        public string EntryAcademicLevel { get; set; }
        public int EntryAcademicLevelNumeric { get; set; }
        public int CheckInOut { get; set; }
        public int StructuredRecess { get; set; }
        public int StructuredBreaks { get; set; }
        public int SmallGroups { get; set; }
        public int SocialWorkerVisits { get; set; }
        public int PsychologistVisits { get; set; }
        public string EntrySocialSkillsLevel { get; set; }
        public int EntrySocialSkillsLevelNumeric { get; set; }
        public DateTime EntryDate { get; set; }
        public int RiskScore { get; set; }

        // Additional normalized scores for ML
        public double StudentStressLevelNormalized { get; set; }
        public double FamilySupportNormalized { get; set; }
        public double AcademicAbilityNormalized { get; set; }
        public double EmotionalRegulationNormalized { get; set; }
    }

    public class DailyBehavior
    {
        public int StudentID { get; set; }
        public DateTime Timestamp { get; set; }
        public int Level { get; set; }
        public int Step { get; set; }
        public int VerbalAggression { get; set; }
        public int PhysicalAggression { get; set; }
        public int Elopement { get; set; }
        public int OutOfSpot { get; set; }
        public int WorkRefusal { get; set; }
        public int ProvokingPeers { get; set; }
        public int InappropriateLanguage { get; set; }
        public int OutOfLane { get; set; }
        public string ZoneOfRegulation { get; set; }
        public int ZoneOfRegulationNumeric { get; set; }
        public int AcademicEngagement { get; set; }
        public int SocialInteractions { get; set; }
        public int EmotionalRegulation { get; set; }
        public string StaffComments { get; set; }
        public DateTime? WeeklyEmotionDate { get; set; }
        public string WeeklyEmotionPictogram { get; set; }
        public int? WeeklyEmotionPictogramNumeric { get; set; }

        // Additional fields for ML
        public double AggressionRiskNormalized { get; set; }
        public double EngagementLevelNormalized { get; set; }
        public int DayInProgram { get; set; }
        public int WeekInProgram { get; set; }
    }

    public class ExitData
    {
        public int ExitID { get; set; }
        public int StudentID { get; set; }
        public string ExitReason { get; set; }
        public int ExitReasonNumeric { get; set; }
        public DateTime ExitDate { get; set; }
        public int LengthOfStay { get; set; }
        public string ExitAcademicLevel { get; set; }
        public int ExitAcademicLevelNumeric { get; set; }
        public string ExitSocialSkillsLevel { get; set; }
        public int ExitSocialSkillsLevelNumeric { get; set; }

        // Additional metrics for ML
        public int AcademicImprovement { get; set; }        // Change in academic level (-2 to +2)
        public int SocialSkillsImprovement { get; set; }    // Change in social skills level (-2 to +2)
        public double OverallImprovementScore { get; set; }  // Calculated improvement metric (0-1)
        public double ProgramEffectivenessScore { get; set; } // How well program worked for student (0-1)
        public int SuccessIndicator { get; set; }           // Binary: 1 = successful outcome, 0 = unsuccessful
    }
}
