using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Text;

namespace BridgeVueApp.Database
{
    public static class DatabaseInitializer
    {

        // Drops the database if it exists
        public static void DropDatabaseIfExists()
        {
            using var conn = new SqlConnection(DatabaseConfig.BaseConnection);
            conn.Open();


            using var cmd = new SqlCommand(@$"
                IF EXISTS (SELECT name FROM sys.databases WHERE name = N'{DatabaseConfig.DbName}')
                BEGIN
                    ALTER DATABASE [{DatabaseConfig.DbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                    DROP DATABASE [{DatabaseConfig.DbName}];
                END
            ", conn);
            cmd.ExecuteNonQuery();
        }


        // Creates the database if it does not exist
        public static void CreateDatabaseIfNotExists(IProgress<string> progress = null)
        {
            using var conn = new SqlConnection(DatabaseConfig.BaseConnection);
            conn.Open();

            string sql = $"IF DB_ID('{DatabaseConfig.DbName}') IS NULL CREATE DATABASE [{DatabaseConfig.DbName}]";
            using var cmd = new SqlCommand(sql, conn);
            cmd.ExecuteNonQuery();

            progress?.Report($"✅ Database '{DatabaseConfig.DbName}' created.");
        }


        // Creates all necessary tables and views
        public static void CreateTablesIfNotExist(IProgress<string> progress = null)
        {
            try
            {
                progress?.Report("🔄 Creating tables...");

                using (SqlConnection conn = new SqlConnection(DatabaseConfig.FullConnection))
                {
                    conn.Open();

                    string sql = $@"
                        -- Drop existing views
                        IF OBJECT_ID('{DatabaseConfig.vStudentPredictionData}', 'V') IS NOT NULL DROP VIEW {DatabaseConfig.vStudentPredictionData};
                        IF OBJECT_ID('{DatabaseConfig.vStudentMLData}', 'V') IS NOT NULL DROP VIEW {DatabaseConfig.vStudentMLData};

                        -- Drop existing tables (respecting FK constraints)
                        IF OBJECT_ID('{DatabaseConfig.TableExitData}', 'U') IS NOT NULL DROP TABLE {DatabaseConfig.TableExitData};
                        IF OBJECT_ID('{DatabaseConfig.TableDailyBehavior}', 'U') IS NOT NULL DROP TABLE {DatabaseConfig.TableDailyBehavior};
                        IF OBJECT_ID('{DatabaseConfig.TableIntakeData}', 'U') IS NOT NULL DROP TABLE {DatabaseConfig.TableIntakeData};
                        IF OBJECT_ID('{DatabaseConfig.TableStudentProfile}', 'U') IS NOT NULL DROP TABLE {DatabaseConfig.TableStudentProfile};
                        IF OBJECT_ID('{DatabaseConfig.TableModelPerformance}', 'U') IS NOT NULL DROP TABLE {DatabaseConfig.TableModelPerformance};
                        IF OBJECT_ID('{DatabaseConfig.TableModelMetricsHistory}', 'U') IS NOT NULL DROP TABLE {DatabaseConfig.TableModelMetricsHistory};                       

                        -- Create tables...
                        CREATE TABLE {DatabaseConfig.TableStudentProfile} (
                            StudentID INT IDENTITY(1001,1) PRIMARY KEY,
                            FirstName NVARCHAR(50),
                            LastName NVARCHAR(50),
                            Grade INT NOT NULL,
                            Age INT NOT NULL,
                            Gender NVARCHAR(10),
                            GenderNumeric INT NOT NULL DEFAULT 0,
                            Ethnicity NVARCHAR(20),
                            EthnicityNumeric INT NOT NULL DEFAULT 0,
                            SpecialEd BIT NOT NULL DEFAULT 0,
                            IEP BIT NOT NULL DEFAULT 0,
                            CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
                            ModifiedDate DATETIME2 DEFAULT GETUTCDATE(),
                            HasKnownOutcome BIT DEFAULT 0,
                            DidSucceed BIT NULL
                        );

                        CREATE TABLE {DatabaseConfig.TableIntakeData} (
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
                            StudentStressLevelNormalized FLOAT NOT NULL DEFAULT 0.0,
                            FamilySupportNormalized FLOAT NOT NULL DEFAULT 0.0,
                            AcademicAbilityNormalized FLOAT NOT NULL DEFAULT 0.0,
                            EmotionalRegulationNormalized FLOAT NOT NULL DEFAULT 0.0,
                            CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
                            ModifiedDate DATETIME2 DEFAULT GETUTCDATE(),
                            CONSTRAINT FK_Intake_Student FOREIGN KEY (StudentID) REFERENCES {DatabaseConfig.TableStudentProfile}(StudentID) ON DELETE CASCADE
                        );

                        CREATE TABLE {DatabaseConfig.TableDailyBehavior} (
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
                            AggressionRiskNormalized FLOAT NOT NULL DEFAULT 0.0,
                            EngagementLevelNormalized FLOAT NOT NULL DEFAULT 0.0,
                            DayInProgram INT NOT NULL DEFAULT 1,
                            WeekInProgram INT NOT NULL DEFAULT 1,
                            CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
                            CONSTRAINT FK_Behavior_Student FOREIGN KEY (StudentID) REFERENCES {DatabaseConfig.TableStudentProfile}(StudentID) ON DELETE CASCADE
                        );

                        CREATE TABLE {DatabaseConfig.TableExitData} (
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
                            AcademicImprovement INT NOT NULL DEFAULT 0,
                            SocialSkillsImprovement INT NOT NULL DEFAULT 0,
                            OverallImprovementScore FLOAT NOT NULL DEFAULT 0.0,
                            ProgramEffectivenessScore FLOAT NOT NULL DEFAULT 0.0,
                            SuccessIndicator BIT NOT NULL DEFAULT 0,
                            CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
                            ModifiedDate DATETIME2 DEFAULT GETUTCDATE(),
                            CONSTRAINT FK_Exit_Student FOREIGN KEY (StudentID) REFERENCES {DatabaseConfig.TableStudentProfile}(StudentID) ON DELETE CASCADE
                        );

                        CREATE TABLE {DatabaseConfig.TableModelPerformance} (
                            ModelID INT IDENTITY(1,1) PRIMARY KEY,
                            TrainingDate DATETIME2,
                            ModelName NVARCHAR(100),
                            ModelType NVARCHAR(100),
                            Hyperparameters NVARCHAR(500),
                            TrainingDuration INT,  -- Duration in seconds
                            Accuracy FLOAT,
                            F1Score FLOAT,
                            AUC FLOAT,
                            [Precision] FLOAT,
                            Recall FLOAT,
                            TrainingDataSize INT,
                            TestDataSize INT,
                            IsCurrentBest BIT,
                            ModelFilePath NVARCHAR(500)
                        );

                        CREATE TABLE {DatabaseConfig.TableModelMetricsHistory} (
                            MetricID INT IDENTITY(1,1) PRIMARY KEY,
                            ModelID INT,  -- Foreign key referencing ModelPerformance
                            Accuracy FLOAT,
                            F1Score FLOAT,
                            AUC FLOAT,
                            Precision FLOAT,
                            Recall FLOAT,
                            Timestamp DATETIME2
                            CONSTRAINT FK_ModelPerf_MetricsHist FOREIGN KEY (ModelID) REFERENCES {DatabaseConfig.TableModelPerformance}(ModelID) ON DELETE CASCADE
                        );



                        CREATE INDEX IX_Intake_StudentID ON {DatabaseConfig.TableIntakeData}(StudentID);
                        CREATE INDEX IX_Behavior_StudentID_Timestamp ON {DatabaseConfig.TableDailyBehavior}(StudentID, Timestamp);
                        CREATE INDEX IX_Exit_StudentID ON {DatabaseConfig.TableExitData}(StudentID);

                    ";

                    new SqlCommand(sql, conn).ExecuteNonQuery();

                    CreateViews(conn, progress);
                    progress?.Report("✅ Tables and views created successfully.");
                }
            }
            catch (Exception ex)
            {
                progress?.Report($"❌ Error creating tables: {ex.Message}");
                throw;
            }
        }

        private static void CreateViews(SqlConnection conn, IProgress<string> progress = null)
        {
            try
            {
                string mlView = $@"
                    CREATE OR ALTER VIEW {DatabaseConfig.vStudentMLData} AS
                    WITH Beh AS (
                        SELECT
                            db.StudentID,
                            AVG(CAST(db.VerbalAggression          AS float)) AS AvgVerbalAggression,
                            AVG(CAST(db.PhysicalAggression        AS float)) AS AvgPhysicalAggression,
                            AVG(CAST(db.AcademicEngagement        AS float)) AS AvgAcademicEngagement,
                            AVG(CAST(db.SocialInteractions        AS float)) AS AvgSocialInteractions,
                            AVG(CAST(db.EmotionalRegulation       AS float)) AS AvgEmotionalRegulation,
                            AVG(CAST(db.AggressionRiskNormalized  AS float)) AS AvgAggressionRisk,
                            AVG(CAST(db.EngagementLevelNormalized AS float)) AS AvgEngagementLevel,
                            SUM(CASE WHEN db.ZoneOfRegulation = 'Red'    THEN 1 ELSE 0 END) * 1.0 / NULLIF(COUNT(*),0) AS RedZonePct,
                            SUM(CASE WHEN db.ZoneOfRegulation = 'Yellow' THEN 1 ELSE 0 END) * 1.0 / NULLIF(COUNT(*),0) AS YellowZonePct,
                            SUM(CASE WHEN db.ZoneOfRegulation = 'Blue'   THEN 1 ELSE 0 END) * 1.0 / NULLIF(COUNT(*),0) AS BlueZonePct,
                            SUM(CASE WHEN db.ZoneOfRegulation = 'Green'  THEN 1 ELSE 0 END) * 1.0 / NULLIF(COUNT(*),0) AS GreenZonePct,
                            COUNT(DISTINCT CAST(db.[Timestamp] AS date)) AS BehaviorDays
                        FROM {DatabaseConfig.TableDailyBehavior} AS db
                        GROUP BY db.StudentID
                    )
                    SELECT
                        sp.StudentID,
                        sp.Grade,
                        sp.Age,
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
                        i.CheckInOut,
                        i.StructuredRecess,
                        i.StructuredBreaks,
                        i.SmallGroups,
                        i.SocialWorkerVisits,
                        i.PsychologistVisits,
                        i.EntrySocialSkillsLevelNumeric,
                        i.RiskScore,
                        i.StudentStressLevelNormalized,
                        i.FamilySupportNormalized,
                        i.AcademicAbilityNormalized,
                        i.EmotionalRegulationNormalized,
                    
                        b.AvgVerbalAggression,
                        b.AvgPhysicalAggression,
                        b.AvgAcademicEngagement,
                        b.AvgSocialInteractions,
                        b.AvgEmotionalRegulation,
                        b.AvgAggressionRisk,
                        b.AvgEngagementLevel,
                        b.RedZonePct,
                        b.YellowZonePct,
                        b.BlueZonePct,
                        b.GreenZonePct,
                        b.BehaviorDays
                    
                    FROM {DatabaseConfig.TableStudentProfile} AS sp
                    LEFT JOIN {DatabaseConfig.TableIntakeData} AS i  ON i.StudentID = sp.StudentID
                    LEFT JOIN Beh AS b                               ON b.StudentID = sp.StudentID;
                    ";

                using (var createMLView = new SqlCommand(mlView, conn))
                    createMLView.ExecuteNonQuery();
                progress?.Report("📊 View vStudentMLData created.");

                string predictionView = $@"
                    CREATE OR ALTER VIEW {DatabaseConfig.vStudentPredictionData} AS
                    SELECT 
                        StudentID,
                        AVG(CAST(VerbalAggression   AS FLOAT)) AS AvgVerbalAggression,
                        AVG(CAST(PhysicalAggression AS FLOAT)) AS AvgPhysicalAggression,
                        AVG(CAST(AcademicEngagement AS FLOAT)) AS AvgAcademicEngagement,
                        SUM(CASE WHEN ZoneOfRegulation = 'Red' THEN 1 ELSE 0 END) * 1.0 / COUNT(*) AS RedZonePct
                    FROM {DatabaseConfig.TableDailyBehavior}
                    GROUP BY StudentID;";

                using (var createPredView = new SqlCommand(predictionView, conn))
                    createPredView.ExecuteNonQuery();
                progress?.Report("📊 View vStudentPredictionData created.");
            }
            catch (Exception ex)
            {
                progress?.Report($"⚠️ Error creating views: {ex.Message}");
                throw;
            }
        }
    }
}