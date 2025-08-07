using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Text;

namespace BridgeVueApp.Database
{
    public static class DatabaseInitializer
    {
        public static void CreateDatabaseIfNotExists(IProgress<string> progress = null)
        {
            using var conn = new SqlConnection(DatabaseConfig.BaseConnection);
            conn.Open();

            string sql = $"IF DB_ID('{DatabaseConfig.DbName}') IS NULL CREATE DATABASE [{DatabaseConfig.DbName}]";
            using var cmd = new SqlCommand(sql, conn);
            cmd.ExecuteNonQuery();

            progress?.Report($"✅ Database '{DatabaseConfig.DbName}' ensured.");
        }

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
                        IF OBJECT_ID('vw_BehavioralAggregates', 'V') IS NOT NULL DROP VIEW vw_BehavioralAggregates;
                        IF OBJECT_ID('vw_MLReadyData', 'V') IS NOT NULL DROP VIEW vw_MLReadyData;

                        -- Drop existing tables (respecting FK constraints)
                        IF OBJECT_ID('{DatabaseConfig.TableExitData}', 'U') IS NOT NULL DROP TABLE {DatabaseConfig.TableExitData};
                        IF OBJECT_ID('{DatabaseConfig.TableDailyBehavior}', 'U') IS NOT NULL DROP TABLE {DatabaseConfig.TableDailyBehavior};
                        IF OBJECT_ID('{DatabaseConfig.TableIntakeData}', 'U') IS NOT NULL DROP TABLE {DatabaseConfig.TableIntakeData};
                        IF OBJECT_ID('{DatabaseConfig.TableStudentProfile}', 'U') IS NOT NULL DROP TABLE {DatabaseConfig.TableStudentProfile};

                        -- Create tables...
                        CREATE TABLE {DatabaseConfig.TableStudentProfile} (
                            StudentID INT PRIMARY KEY,
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
                        CASE WHEN e.StudentID IS NOT NULL THEN 1 ELSE 0 END AS HasKnownOutcome
                    FROM {DatabaseConfig.TableStudentProfile} sp
                    INNER JOIN {DatabaseConfig.TableIntakeData} i ON sp.StudentID = i.StudentID
                    LEFT JOIN {DatabaseConfig.TableExitData} e ON sp.StudentID = e.StudentID";

                new SqlCommand(mlView, conn).ExecuteNonQuery();
                progress?.Report("📊 View vw_MLReadyData created.");
            }
            catch (Exception ex)
            {
                progress?.Report($"⚠️ Error creating views: {ex.Message}");
            }
        }
    }
}
