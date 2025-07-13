using CsvHelper;
using Microsoft.Data.SqlClient;
using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;



namespace BridgeVueApp
{
    public partial class SetupForm : Form
    {
        private string connectionString = "Server=localhost;Integrated Security=true;TrustServerCertificate=True;";
        private string dbName = "BridgeVue";
        private string dbConnection => $"Server=localhost;Database={dbName};Integrated Security=true;TrustServerCertificate=True;";

        public SetupForm()
        {
            InitializeComponent();
        }

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

                    string createTables = @"
                        IF OBJECT_ID('StudentProfile', 'U') IS NULL
                        CREATE TABLE StudentProfile (
                            StudentID INT PRIMARY KEY,
                            Grade INT,
                            Age INT,
                            Gender NVARCHAR(10),
                            Ethnicity NVARCHAR(20),
                            SpecialEd BIT,
                            IEP BIT
                        );

                        IF OBJECT_ID('IntakeData', 'U') IS NULL
                        CREATE TABLE IntakeData (
                            StudentID INT PRIMARY KEY,
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
                            RiskScore INT
                        );

                        IF OBJECT_ID('DailyBehavior', 'U') IS NULL
                        CREATE TABLE DailyBehavior (
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
                        );
                    ";

                    SqlCommand cmd = new SqlCommand(createTables, conn);
                    cmd.ExecuteNonQuery();
                }

                lblStatus.Text = "Database and all tables created successfully.";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                lblStatus.Text = "Failed to create database or tables.";
            }
        }

        private void btnLoadStudentProfile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select Student Profile CSV";
                openFileDialog.Filter = "CSV Files (*.csv)|*.csv";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;

                    try
                    {
                        using (var reader = new StreamReader(filePath))
                        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                        using (SqlConnection conn = new SqlConnection(dbConnection))
                        {
                            conn.Open();
                            var records = csv.GetRecords<dynamic>();
                            foreach (var record in records)
                            {
                                string query = @"
                                    INSERT INTO StudentProfile (StudentID, Grade, Age, Gender, Ethnicity, SpecialEd, IEP)
                                    VALUES (@StudentID, @Grade, @Age, @Gender, @Ethnicity, @SpecialEd, @IEP)";
                                using (SqlCommand cmd = new SqlCommand(query, conn))
                                {
                                    cmd.Parameters.AddWithValue("@StudentID", record.StudentID);
                                    cmd.Parameters.AddWithValue("@Grade", record.Grade);
                                    cmd.Parameters.AddWithValue("@Age", record.Age);
                                    cmd.Parameters.AddWithValue("@Gender", record.Gender);
                                    cmd.Parameters.AddWithValue("@Ethnicity", record.Ethnicity);
                                    cmd.Parameters.AddWithValue("@SpecialEd", record.SpecialEd);
                                    cmd.Parameters.AddWithValue("@IEP", record.IEP);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }

                        lblStatus.Text = "Student Profile data loaded successfully.";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading CSV: " + ex.Message);
                        lblStatus.Text = "Data load failed.";
                    }
                }
            }
        }

        private void btnViewDatabaseInfo_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(dbConnection))
                {
                    conn.Open();
                    string query = @"
                SELECT 
                    t.NAME AS TableName,
                    SUM(p.rows) AS [RowCount]
                FROM 
                    sys.tables t
                INNER JOIN      
                    sys.indexes i ON t.OBJECT_ID = i.object_id
                INNER JOIN 
                    sys.partitions p ON i.object_id = p.OBJECT_ID AND i.index_id = p.index_id
                WHERE 
                    t.is_ms_shipped = 0 AND i.type <= 1
                GROUP BY 
                    t.NAME
                ORDER BY 
                    t.NAME;
            ";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    string output = $"BridgeVue Database Summary ({DateTime.Now}):\n\n";

                    while (reader.Read())
                    {
                        string tableName = reader["TableName"].ToString();
                        string rowCount = reader["RowCount"].ToString();
                        output += $"{tableName}: {rowCount} rows\n";
                    }

                    MessageBox.Show(output, "Database Info");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving database info: " + ex.Message);
            }
        }

    }
}
