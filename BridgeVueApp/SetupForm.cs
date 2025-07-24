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

        // Create Database and Tables
        private void btnCreateDatabaseAndTables_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string createDb = $"IF DB_ID('{dbName}') IS NULL CREATE DATABASE {dbName};";
                    new SqlCommand(createDb, conn).ExecuteNonQuery();
                }

                using (SqlConnection conn = new SqlConnection(dbConnection))
                {
                    conn.Open();
                    string createTables = $@"
                        IF OBJECT_ID('{tableStudentProfile}', 'U') IS NULL
                        CREATE TABLE {tableStudentProfile} (
                            StudentID INT PRIMARY KEY,
                            Grade INT,
                            Age INT,
                            Gender NVARCHAR(10),
                            Ethnicity NVARCHAR(20),
                            SpecialEd BIT,
                            IEP BIT
                        );

                        IF OBJECT_ID('{tableIntakeData}', 'U') IS NULL
                        CREATE TABLE IntakeData (
                            IntakeID INT IDENTITY(1,1) PRIMARY KEY,
                            StudentID INT,
                            EntryReason NVARCHAR(50),
                            PriorIncidents INT,
                            OfficeReferrals INT,
                            Suspensions INT,
                            Expulsions INT,
                            EntryAcademicLevel NVARCHAR(20),
                            CheckInOut BIT,
                            StructuredRecess BIT,
                            StructuredBreaks BIT,
                            SmallGroups BIT,
                            SocialWorkerVisits INT,
                            PsychologistVisits INT,
                            EntrySocialSkillsLevel NVARCHAR(20),
                            EntryDate DATE,
                            RiskScore INT,
                            CONSTRAINT FK_Intake_Student FOREIGN KEY (StudentID) REFERENCES StudentProfile(StudentID)
                        );


                        IF OBJECT_ID('{tableDailyBehavior}', 'U') IS NULL
                        CREATE TABLE {tableDailyBehavior} (
                            BehaviorID INT IDENTITY(1,1) PRIMARY KEY,
                            StudentID INT,
                            Timestamp DATE,
                            Level INT,
                            Step INT,
                            VerbalAggression INT,
                            PhysicalAggression INT,
                            Elopement INT,
                            OutOfSpot INT,
                            WorkRefusal INT,
                            ProvokingPeers INT,
                            InappropriateLanguage INT,
                            OutOfLane INT,
                            ZoneOfRegulation NVARCHAR(10),
                            AcademicEngagement INT,
                            SocialInteractions INT,
                            EmotionalRegulation INT,
                            StaffComments NVARCHAR(255),
                            WeeklyEmotionDate DATE,
                            WeeklyEmotionPictogram NVARCHAR(20)
                            CONSTRAINT FK_Behavior_Student FOREIGN KEY (StudentID) REFERENCES StudentProfile(StudentID)
                        );

                        IF OBJECT_ID('{tableExitData}', 'U') IS NULL
                        CREATE TABLE ExitData (
                            ExitID INT IDENTITY(1,1) PRIMARY KEY,
                            StudentID INT,
                            ExitReason NVARCHAR(50),
                            ExitDate DATE,
                            LengthOfStay INT,
                            ExitAcademicLevel NVARCHAR(20),
                            ExitSocialSkillsLevel NVARCHAR(20),
                            CONSTRAINT FK_Exit_Student FOREIGN KEY (StudentID) REFERENCES StudentProfile(StudentID)
                        );
                    ";
                    new SqlCommand(createTables, conn).ExecuteNonQuery();
                }

                lblStatus.Text = "Database and tables created successfully.";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                lblStatus.Text = "Failed to create database or tables.";
            }
        }

        // View Database Info
        private void btnViewDatabaseInfo_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(dbConnection))
                {
                    conn.Open();
                    string query = @"
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
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    StringBuilder result = new StringBuilder();
                    result.AppendLine("Table Row Counts in BridgeVue Database");
                    result.AppendLine("----------------------------------------");
                    result.AppendLine($"{"Table Name",-25}{"Rows",10}");
                    result.AppendLine("----------------------------------------");

                    while (reader.Read())
                    {
                        string tableName = reader["TableName"].ToString();
                        int rowCount = Convert.ToInt32(reader["RowCount"]);

                        result.AppendLine($"{tableName,-25}{rowCount,10:N0}");  // Format with commas
                    }

                    lblStatus.Font = new Font("Consolas", 10);  // Monospaced for alignment
                    lblStatus.Text = result.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                lblStatus.Text = "Failed to retrieve database info.";
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

        // Generate Data
        private void btnGenerateData_Click(object sender, EventArgs e)
        {
            GenerateSyntheticData();
            lblStatus.Text = "Synthetic data generated successfully.";
        }

        private void GenerateSyntheticData()
        {
            Random rand = new Random();
            generatedProfiles.Clear();
            generatedIntake.Clear();
            generatedBehavior.Clear();
            generatedExitData.Clear();

            int numStudents = 50;

            for (int studentId = 1; studentId <= numStudents; studentId++)
            {
                int grade = rand.Next(0, 6);
                int age = grade + rand.Next(5, 8);

                generatedProfiles.Add(new StudentProfile
                {
                    StudentID = studentId,
                    Grade = grade,
                    Age = age,
                    Gender = rand.NextDouble() > 0.5 ? "Male" : "Female",
                    Ethnicity = new[] { "White", "Black", "Hispanic", "Asian", "Other" }[rand.Next(5)],
                    SpecialEd = rand.NextDouble() > 0.7 ? 1 : 0,
                    IEP = rand.NextDouble() > 0.75 ? 1 : 0
                });

                DateTime entryDate = DateTime.Now.AddDays(-rand.Next(30, 120));

                generatedIntake.Add(new IntakeData
                {
                    IntakeID = 0,
                    StudentID = studentId,
                    EntryReason = new[] { "Aggression", "Anxiety", "Trauma", "Withdrawn", "Disruptive", "Other" }[rand.Next(6)],
                    PriorIncidents = rand.Next(0, 5),
                    OfficeReferrals = rand.Next(0, 3),
                    Suspensions = rand.Next(0, 2),
                    Expulsions = rand.Next(0, 1),
                    EntryAcademicLevel = new[] { "Below Grade", "At Grade", "Above Grade" }[rand.Next(3)],
                    CheckInOut = rand.Next(0, 2),
                    StructuredRecess = rand.Next(0, 2),
                    StructuredBreaks = rand.Next(0, 2),
                    SmallGroups = rand.Next(0, 2),
                    SocialWorkerVisits = rand.Next(0, 3),
                    PsychologistVisits = rand.Next(0, 2),
                    EntrySocialSkillsLevel = new[] { "Low", "Medium", "High" }[rand.Next(3)],
                    EntryDate = entryDate,
                    RiskScore = rand.Next(1, 10)
                });

                int behaviorDays = rand.Next(30, 61);
                int totalAggression = 0;
                int totalEngagement = 0;
                int redZoneDays = 0;

                for (int day = 0; day < behaviorDays; day++)
                {
                    int verbalAggression = rand.Next(0, 2);
                    int physicalAggression = rand.Next(0, 2);
                    int academicEngagement = rand.Next(1, 6);
                    string zone = new[] { "Green", "Blue", "Yellow", "Red" }[rand.Next(4)];

                    totalAggression += verbalAggression + physicalAggression;
                    totalEngagement += academicEngagement;
                    if (zone == "Red") redZoneDays++;

                    generatedBehavior.Add(new DailyBehavior
                    {
                        StudentID = studentId,
                        Timestamp = entryDate.AddDays(day),
                        Level = rand.Next(1, 7),
                        Step = rand.Next(1, 7),
                        VerbalAggression = verbalAggression,
                        PhysicalAggression = physicalAggression,
                        Elopement = rand.Next(0, 2),
                        OutOfSpot = rand.Next(0, 2),
                        WorkRefusal = rand.Next(0, 2),
                        ProvokingPeers = rand.Next(0, 2),
                        InappropriateLanguage = rand.Next(0, 2),
                        OutOfLane = rand.Next(0, 2),
                        ZoneOfRegulation = zone,
                        AcademicEngagement = academicEngagement,
                        SocialInteractions = rand.Next(1, 6),
                        EmotionalRegulation = rand.Next(1, 6),
                        StaffComments = string.Empty,
                        WeeklyEmotionDate = day % 7 == 0 ? (DateTime?)entryDate.AddDays(day) : null,
                        WeeklyEmotionPictogram = day % 7 == 0 ? new[] { "Happy", "Sad", "Angry", "Lonely", "Nervous", "Excited" }[rand.Next(6)] : string.Empty
                    });
                }

                double avgAggression = (double)totalAggression / behaviorDays;
                double avgEngagement = (double)totalEngagement / behaviorDays;
                double redZonePercent = (double)redZoneDays / behaviorDays;

                // Simulate missing exit data for ~20% of students
                if (rand.NextDouble() < 0.2) continue;

                string likelyOutcome;

                if (avgAggression < 0.5 && avgEngagement > 3)
                    likelyOutcome = "Returned Successfully";
                else if (avgAggression > 1.5 && redZonePercent > 0.3)
                    likelyOutcome = "Referred Out";
                else if (rand.NextDouble() < 0.1)
                    likelyOutcome = "ACC";
                else
                    likelyOutcome = new[] { "Returned Successfully", "Referred Out", "ABS", "ACC", "Other" }[rand.Next(5)];

                DateTime exitDate = entryDate.AddDays(rand.Next(30, 120));

                generatedExitData.Add(new ExitData
                {
                    ExitID = 0,
                    StudentID = studentId,
                    ExitReason = likelyOutcome,
                    ExitDate = exitDate,
                    LengthOfStay = (exitDate - entryDate).Days,
                    ExitAcademicLevel = new[] { "Below Grade", "At Grade", "Above Grade" }[rand.Next(3)],
                    ExitSocialSkillsLevel = new[] { "Low", "Medium", "High" }[rand.Next(3)]
                });
            }
        }

        // Load Generated Data into Database
        private void btnLoadGeneratedData_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Do you want to clear existing StudentProfile and IntakeData tables before loading new data?\nClick Yes to truncate and reload, No to append only new DailyBehavior records.", "Data Load Option", MessageBoxButtons.YesNoCancel);

            if (result == DialogResult.Cancel)
            {
                lblStatus.Text = "Data load canceled by user.";
                return;
            }

            using (SqlConnection conn = new SqlConnection(dbConnection))
            {
                conn.Open();

                if (result == DialogResult.Yes)
                {
                    // Truncate the tables to allow full reload
                    string truncateSql = $"TRUNCATE TABLE {tableStudentProfile}; TRUNCATE TABLE {tableIntakeData};";
                    using (SqlCommand truncateCmd = new SqlCommand(truncateSql, conn))
                    {
                        truncateCmd.ExecuteNonQuery();
                    }
                }

                foreach (var profile in generatedProfiles)
                {
                    string query = $@"
                    IF NOT EXISTS (SELECT 1 FROM {tableStudentProfile} WHERE StudentID = @StudentID)
                    BEGIN
                        INSERT INTO {tableStudentProfile} (StudentID, Grade, Age, Gender, Ethnicity, SpecialEd, IEP)
                        VALUES (@StudentID, @Grade, @Age, @Gender, @Ethnicity, @SpecialEd, @IEP)
                    END";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@StudentID", profile.StudentID);
                        cmd.Parameters.AddWithValue("@Grade", profile.Grade);
                        cmd.Parameters.AddWithValue("@Age", profile.Age);
                        cmd.Parameters.AddWithValue("@Gender", profile.Gender);
                        cmd.Parameters.AddWithValue("@Ethnicity", profile.Ethnicity);
                        cmd.Parameters.AddWithValue("@SpecialEd", profile.SpecialEd);
                        cmd.Parameters.AddWithValue("@IEP", profile.IEP);
                        cmd.ExecuteNonQuery();
                    }
                }

                foreach (var intake in generatedIntake)
                {
                    string query = $@"
                    IF NOT EXISTS (SELECT 1 FROM {tableIntakeData} WHERE StudentID = @StudentID)
                    BEGIN
                        INSERT INTO {tableIntakeData} (StudentID, EntryReason, PriorIncidents, OfficeReferrals, Suspensions, Expulsions, 
                            EntryAcademicLevel, CheckInOut, StructuredRecess, StructuredBreaks, SmallGroups, SocialWorkerVisits, PsychologistVisits, 
                            EntrySocialSkillsLevel, EntryDate, RiskScore)
                        VALUES (@StudentID, @EntryReason, @PriorIncidents, @OfficeReferrals, @Suspensions, @Expulsions, @EntryAcademicLevel, 
                            @CheckInOut, @StructuredRecess, @StructuredBreaks, @SmallGroups, @SocialWorkerVisits, @PsychologistVisits, 
                            @EntrySocialSkillsLevel, @EntryDate, @RiskScore)
                    END";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@StudentID", intake.StudentID);
                        cmd.Parameters.AddWithValue("@EntryReason", intake.EntryReason);
                        cmd.Parameters.AddWithValue("@PriorIncidents", intake.PriorIncidents);
                        cmd.Parameters.AddWithValue("@OfficeReferrals", intake.OfficeReferrals);
                        cmd.Parameters.AddWithValue("@Suspensions", intake.Suspensions);
                        cmd.Parameters.AddWithValue("@Expulsions", intake.Expulsions);
                        cmd.Parameters.AddWithValue("@EntryAcademicLevel", intake.EntryAcademicLevel);
                        cmd.Parameters.AddWithValue("@CheckInOut", intake.CheckInOut);
                        cmd.Parameters.AddWithValue("@StructuredRecess", intake.StructuredRecess);
                        cmd.Parameters.AddWithValue("@StructuredBreaks", intake.StructuredBreaks);
                        cmd.Parameters.AddWithValue("@SmallGroups", intake.SmallGroups);
                        cmd.Parameters.AddWithValue("@SocialWorkerVisits", intake.SocialWorkerVisits);
                        cmd.Parameters.AddWithValue("@PsychologistVisits", intake.PsychologistVisits);
                        cmd.Parameters.AddWithValue("@EntrySocialSkillsLevel", intake.EntrySocialSkillsLevel);
                        cmd.Parameters.AddWithValue("@EntryDate", intake.EntryDate);
                        cmd.Parameters.AddWithValue("@RiskScore", intake.RiskScore);
                        cmd.ExecuteNonQuery();
                    }
                }

                foreach (var behavior in generatedBehavior)
                {
                    string query = $@"
                    INSERT INTO {tableDailyBehavior} (StudentID, Timestamp, Level, Step, VerbalAggression, PhysicalAggression, Elopement, OutOfSpot, 
                        WorkRefusal, ProvokingPeers, InappropriateLanguage, OutOfLane, ZoneOfRegulation, AcademicEngagement, SocialInteractions, 
                        EmotionalRegulation, StaffComments, WeeklyEmotionDate, WeeklyEmotionPictogram)
                    VALUES (@StudentID, @Timestamp, @Level, @Step, @VerbalAggression, @PhysicalAggression, @Elopement, @OutOfSpot, @WorkRefusal, 
                        @ProvokingPeers, @InappropriateLanguage, @OutOfLane, @ZoneOfRegulation, @AcademicEngagement, @SocialInteractions, 
                        @EmotionalRegulation, @StaffComments, @WeeklyEmotionDate, @WeeklyEmotionPictogram)";
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
                        cmd.Parameters.AddWithValue("@ZoneOfRegulation", behavior.ZoneOfRegulation);
                        cmd.Parameters.AddWithValue("@AcademicEngagement", behavior.AcademicEngagement);
                        cmd.Parameters.AddWithValue("@SocialInteractions", behavior.SocialInteractions);
                        cmd.Parameters.AddWithValue("@EmotionalRegulation", behavior.EmotionalRegulation);
                        cmd.Parameters.AddWithValue("@StaffComments", behavior.StaffComments);
                        cmd.Parameters.AddWithValue("@WeeklyEmotionDate", behavior.WeeklyEmotionDate.HasValue ? (object)behavior.WeeklyEmotionDate.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@WeeklyEmotionPictogram", behavior.WeeklyEmotionPictogram);
                        cmd.ExecuteNonQuery();
                    }
                }

                foreach (var exit in generatedExitData)
                {
                    string query = $@"
                    INSERT INTO {tableExitData} (StudentID, ExitReason, ExitDate, LengthOfStay, ExitAcademicLevel, ExitSocialSkillsLevel)
                    VALUES (@StudentID, @ExitReason, @ExitDate, @LengthOfStay, @ExitAcademicLevel, @ExitSocialSkillsLevel);";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@StudentID", exit.StudentID);
                        cmd.Parameters.AddWithValue("@ExitReason", exit.ExitReason);
                        cmd.Parameters.AddWithValue("@ExitDate", exit.ExitDate);
                        cmd.Parameters.AddWithValue("@LengthOfStay", exit.LengthOfStay);
                        cmd.Parameters.AddWithValue("@ExitAcademicLevel", exit.ExitAcademicLevel);
                        cmd.Parameters.AddWithValue("@ExitSocialSkillsLevel", exit.ExitSocialSkillsLevel);

                        cmd.ExecuteNonQuery();
                    }
                }

                lblStatus.Text = "Generated data loaded into database.";
            }
        }

        // Save Generated Data as CSVs
        private void btnSaveGeneratedCSV_Click(object sender, EventArgs e)
        {
            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BridgeVueData", DateTime.Now.ToString("yyyyMMdd_HHmmss"));
            Directory.CreateDirectory(folderPath);

            using (var writer = new StreamWriter(Path.Combine(folderPath, "StudentProfile.csv")))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(generatedProfiles);
            }

            using (var writer = new StreamWriter(Path.Combine(folderPath, "IntakeData.csv")))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(generatedIntake);
            }

            using (var writer = new StreamWriter(Path.Combine(folderPath, "DailyBehavior.csv")))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(generatedBehavior);
            }

            using (var writer = new StreamWriter(Path.Combine(folderPath, "ExitData.csv")))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(generatedExitData);
            }

            lblStatus.Text = $"Synthetic data saved to {folderPath}";


            btnLoadStudentProfile_Click(sender, e);
        }

        private void btnLoadStudentProfile_Click(object sender, EventArgs e)
        {

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
    }

    public class StudentProfile
    {
        public int StudentID { get; set; }
        public int Grade { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Ethnicity { get; set; } = string.Empty;
        public int SpecialEd { get; set; }
        public int IEP { get; set; }
    }

    public class IntakeData
    {
        public int IntakeID { get; set; }
        public int StudentID { get; set; }
        public string EntryReason { get; set; } = string.Empty;
        public int PriorIncidents { get; set; }
        public int OfficeReferrals { get; set; }
        public int Suspensions { get; set; }
        public int Expulsions { get; set; }
        public string EntryAcademicLevel { get; set; } = string.Empty;
        public int CheckInOut { get; set; }
        public int StructuredRecess { get; set; }
        public int StructuredBreaks { get; set; }
        public int SmallGroups { get; set; }
        public int SocialWorkerVisits { get; set; }
        public int PsychologistVisits { get; set; }
        public string EntrySocialSkillsLevel { get; set; } = string.Empty;
        public DateTime EntryDate { get; set; }
        public int RiskScore { get; set; }
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
        public string ZoneOfRegulation { get; set; } = string.Empty;
        public int AcademicEngagement { get; set; }
        public int SocialInteractions { get; set; }
        public int EmotionalRegulation { get; set; }
        public string StaffComments { get; set; } = string.Empty;
        public DateTime? WeeklyEmotionDate { get; set; }
        public string WeeklyEmotionPictogram { get; set; } = string.Empty;
    }

    public class ExitData
    {
        public int ExitID { get; set; }
        public int StudentID { get; set; }
        public string ExitReason { get; set; } = string.Empty;
        public DateTime ExitDate { get; set; }
        public int LengthOfStay { get; set; }
        public string ExitAcademicLevel { get; set; } = string.Empty;
        public string ExitSocialSkillsLevel { get; set; } = string.Empty;
    }
}
