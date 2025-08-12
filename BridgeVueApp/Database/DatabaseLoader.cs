// DatabaseLoader.cs
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Microsoft.Data.SqlClient;
using BridgeVueApp.Models;
using BridgeVueApp.Database;

namespace BridgeVueApp.Database
{
    public static class DatabaseLoader
    {
        // =========================
        // Public entry points
        // =========================

        public static List<StudentProfile> BulkInsertStudentProfiles(List<StudentProfile> profiles)
        {
            if (profiles == null || profiles.Count == 0) return profiles ?? new List<StudentProfile>();

            using var conn = new SqlConnection(DatabaseConfig.FullConnection);
            conn.Open();
            DebugWriteCurrentDb(conn);

            using var tx = conn.BeginTransaction();
            try
            {
                var result = BulkInsertStudentProfiles(conn, tx, profiles);
                tx.Commit();
                return result;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public static void BulkInsertIntakeData(List<IntakeData> intakeList)
        {
            if (intakeList == null || intakeList.Count == 0) return;

            using var conn = new SqlConnection(DatabaseConfig.FullConnection);
            conn.Open();
            DebugWriteCurrentDb(conn);

            using var tx = conn.BeginTransaction();
            try
            {
                AssertStudentIdsExist(conn, tx, intakeList.Select(i => i.StudentID), DatabaseConfig.TableIntakeData);
                BulkInsertIntakeData(conn, tx, intakeList);
                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public static void BulkInsertDailyBehavior(List<DailyBehavior> behaviors)
        {
            if (behaviors == null || behaviors.Count == 0) return;

            using var conn = new SqlConnection(DatabaseConfig.FullConnection);
            conn.Open();
            DebugWriteCurrentDb(conn);

            using var tx = conn.BeginTransaction();
            try
            {
                AssertStudentIdsExist(conn, tx, behaviors.Select(b => b.StudentID), DatabaseConfig.TableDailyBehavior);
                BulkInsertDailyBehavior(conn, tx, behaviors);
                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public static void BulkInsertExitData(List<ExitData> exitList)
        {
            if (exitList == null || exitList.Count == 0) return;

            using var conn = new SqlConnection(DatabaseConfig.FullConnection);
            conn.Open();
            DebugWriteCurrentDb(conn);

            using var tx = conn.BeginTransaction();
            try
            {
                AssertStudentIdsExist(conn, tx, exitList.Select(x => x.StudentID), DatabaseConfig.TableExitData);
                BulkInsertExitData(conn, tx, exitList);
                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        // =========================
        // Transaction-friendly overloads
        // =========================

        public static List<StudentProfile> BulkInsertStudentProfiles(SqlConnection conn, SqlTransaction tx, List<StudentProfile> profiles)
        {
            if (profiles == null || profiles.Count == 0) return profiles ?? new List<StudentProfile>();

            foreach (var p in profiles)
            {
                var sql = $@"
INSERT INTO {DatabaseConfig.TableStudentProfile}
(FirstName, LastName, Grade, Age, Gender, GenderNumeric, 
 Ethnicity, EthnicityNumeric, SpecialEd, IEP, HasKnownOutcome, DidSucceed, CreatedDate, ModifiedDate)
OUTPUT INSERTED.StudentID
VALUES
(@FirstName, @LastName, @Grade, @Age, @Gender, @GenderNumeric,
 @Ethnicity, @EthnicityNumeric, @SpecialEd, @IEP, @HasKnownOutcome, @DidSucceed, @CreatedDate, @ModifiedDate);";

                using var cmd = CreateCommand(conn, tx, sql);
                AddParam(cmd, "@FirstName", SqlDbType.NVarChar, p.FirstName, 100);
                AddParam(cmd, "@LastName", SqlDbType.NVarChar, p.LastName, 100);
                AddParam(cmd, "@Grade", SqlDbType.Int, p.Grade);
                AddParam(cmd, "@Age", SqlDbType.Int, p.Age);
                AddParam(cmd, "@Gender", SqlDbType.NVarChar, p.Gender, 50);
                AddParam(cmd, "@GenderNumeric", SqlDbType.Int, p.GenderNumeric);
                AddParam(cmd, "@Ethnicity", SqlDbType.NVarChar, p.Ethnicity, 100);
                AddParam(cmd, "@EthnicityNumeric", SqlDbType.Int, p.EthnicityNumeric);
                AddParam(cmd, "@SpecialEd", SqlDbType.Bit, p.SpecialEd);
                AddParam(cmd, "@IEP", SqlDbType.Bit, p.IEP);
                AddParam(cmd, "@HasKnownOutcome", SqlDbType.Bit, p.HasKnownOutcome);
                AddParam(cmd, "@DidSucceed", SqlDbType.Bit, p.DidSucceed);
                AddParam(cmd, "@CreatedDate", SqlDbType.DateTime2, p.CreatedDate);
                AddParam(cmd, "@ModifiedDate", SqlDbType.DateTime2, p.ModifiedDate);

                var newIdObj = cmd.ExecuteScalar();
                p.StudentID = Convert.ToInt32(newIdObj); // switch to Int64 if BIGINT
            }

            return profiles;
        }

        public static void BulkInsertIntakeData(SqlConnection conn, SqlTransaction tx, List<IntakeData> intakeList)
        {
            if (intakeList == null || intakeList.Count == 0) return;

            foreach (var i in intakeList)
            {
                var sql = $@"
INSERT INTO {DatabaseConfig.TableIntakeData}
(StudentID, EntryReason, EntryReasonNumeric, PriorIncidents,
 OfficeReferrals, Suspensions, Expulsions, EntryAcademicLevel, EntryAcademicLevelNumeric,
 CheckInOut, StructuredRecess, StructuredBreaks, SmallGroups, SocialWorkerVisits,
 PsychologistVisits, EntrySocialSkillsLevel, EntrySocialSkillsLevelNumeric, EntryDate,
 RiskScore, StudentStressLevelNormalized, FamilySupportNormalized, AcademicAbilityNormalized,
 EmotionalRegulationNormalized, CreatedDate, ModifiedDate)
VALUES
(@StudentID, @EntryReason, @EntryReasonNumeric, @PriorIncidents,
 @OfficeReferrals, @Suspensions, @Expulsions, @EntryAcademicLevel, @EntryAcademicLevelNumeric,
 @CheckInOut, @StructuredRecess, @StructuredBreaks, @SmallGroups, @SocialWorkerVisits,
 @PsychologistVisits, @EntrySocialSkillsLevel, @EntrySocialSkillsLevelNumeric, @EntryDate,
 @RiskScore, @StudentStressLevelNormalized, @FamilySupportNormalized, @AcademicAbilityNormalized,
 @EmotionalRegulationNormalized, @CreatedDate, @ModifiedDate);";

                using var cmd = CreateCommand(conn, tx, sql);
                AddParam(cmd, "@StudentID", SqlDbType.Int, i.StudentID); // use BigInt if schema requires
                AddParam(cmd, "@EntryReason", SqlDbType.NVarChar, i.EntryReason, 100);
                AddParam(cmd, "@EntryReasonNumeric", SqlDbType.Int, i.EntryReasonNumeric);
                AddParam(cmd, "@PriorIncidents", SqlDbType.Int, i.PriorIncidents);
                AddParam(cmd, "@OfficeReferrals", SqlDbType.Int, i.OfficeReferrals);
                AddParam(cmd, "@Suspensions", SqlDbType.Int, i.Suspensions);
                AddParam(cmd, "@Expulsions", SqlDbType.Int, i.Expulsions);
                AddParam(cmd, "@EntryAcademicLevel", SqlDbType.NVarChar, i.EntryAcademicLevel, 50);
                AddParam(cmd, "@EntryAcademicLevelNumeric", SqlDbType.Int, i.EntryAcademicLevelNumeric);
                AddParam(cmd, "@CheckInOut", SqlDbType.Bit, i.CheckInOut);
                AddParam(cmd, "@StructuredRecess", SqlDbType.Bit, i.StructuredRecess);
                AddParam(cmd, "@StructuredBreaks", SqlDbType.Bit, i.StructuredBreaks);
                AddParam(cmd, "@SmallGroups", SqlDbType.Bit, i.SmallGroups);
                AddParam(cmd, "@SocialWorkerVisits", SqlDbType.Int, i.SocialWorkerVisits);
                AddParam(cmd, "@PsychologistVisits", SqlDbType.Int, i.PsychologistVisits);
                AddParam(cmd, "@EntrySocialSkillsLevel", SqlDbType.NVarChar, i.EntrySocialSkillsLevel, 50);
                AddParam(cmd, "@EntrySocialSkillsLevelNumeric", SqlDbType.Int, i.EntrySocialSkillsLevelNumeric);
                AddParam(cmd, "@EntryDate", SqlDbType.DateTime2, i.EntryDate);
                AddParam(cmd, "@RiskScore", SqlDbType.Float, i.RiskScore); // Double in SQL Server
                AddParam(cmd, "@StudentStressLevelNormalized", SqlDbType.Float, i.StudentStressLevelNormalized);
                AddParam(cmd, "@FamilySupportNormalized", SqlDbType.Float, i.FamilySupportNormalized);
                AddParam(cmd, "@AcademicAbilityNormalized", SqlDbType.Float, i.AcademicAbilityNormalized);
                AddParam(cmd, "@EmotionalRegulationNormalized", SqlDbType.Float, i.EmotionalRegulationNormalized);
                AddParam(cmd, "@CreatedDate", SqlDbType.DateTime2, i.CreatedDate);
                AddParam(cmd, "@ModifiedDate", SqlDbType.DateTime2, i.ModifiedDate);

                ExecuteWithFkCatch(cmd, "IntakeData", i.StudentID);
            }
        }

        public static void BulkInsertDailyBehavior(SqlConnection conn, SqlTransaction tx, List<DailyBehavior> behaviors)
        {
            if (behaviors == null || behaviors.Count == 0) return;

            foreach (var b in behaviors)
            {
                var sql = $@"
INSERT INTO {DatabaseConfig.TableDailyBehavior}
(StudentID, [Timestamp], Level, Step, VerbalAggression,
 PhysicalAggression, Elopement, OutOfSpot, WorkRefusal, ProvokingPeers, InappropriateLanguage,
 OutOfLane, ZoneOfRegulation, ZoneOfRegulationNumeric, AcademicEngagement, SocialInteractions,
 EmotionalRegulation, StaffComments, WeeklyEmotionDate, WeeklyEmotionPictogram,
 WeeklyEmotionPictogramNumeric, AggressionRiskNormalized, EngagementLevelNormalized,
 DayInProgram, WeekInProgram, CreatedDate)
VALUES
(@StudentID, @Timestamp, @Level, @Step, @VerbalAggression,
 @PhysicalAggression, @Elopement, @OutOfSpot, @WorkRefusal, @ProvokingPeers, @InappropriateLanguage,
 @OutOfLane, @ZoneOfRegulation, @ZoneOfRegulationNumeric, @AcademicEngagement, @SocialInteractions,
 @EmotionalRegulation, @StaffComments, @WeeklyEmotionDate, @WeeklyEmotionPictogram,
 @WeeklyEmotionPictogramNumeric, @AggressionRiskNormalized, @EngagementLevelNormalized,
 @DayInProgram, @WeekInProgram, @CreatedDate);";

                using var cmd = CreateCommand(conn, tx, sql);
                AddParam(cmd, "@StudentID", SqlDbType.Int, b.StudentID); // use BigInt if schema requires
                AddParam(cmd, "@Timestamp", SqlDbType.DateTime2, b.Timestamp);
                AddParam(cmd, "@Level", SqlDbType.Int, b.Level);
                AddParam(cmd, "@Step", SqlDbType.Int, b.Step);
                AddParam(cmd, "@VerbalAggression", SqlDbType.Int, b.VerbalAggression);
                AddParam(cmd, "@PhysicalAggression", SqlDbType.Int, b.PhysicalAggression);
                AddParam(cmd, "@Elopement", SqlDbType.Int, b.Elopement);
                AddParam(cmd, "@OutOfSpot", SqlDbType.Int, b.OutOfSpot);
                AddParam(cmd, "@WorkRefusal", SqlDbType.Int, b.WorkRefusal);
                AddParam(cmd, "@ProvokingPeers", SqlDbType.Int, b.ProvokingPeers);
                AddParam(cmd, "@InappropriateLanguage", SqlDbType.Int, b.InappropriateLanguage);
                AddParam(cmd, "@OutOfLane", SqlDbType.Int, b.OutOfLane);
                AddParam(cmd, "@ZoneOfRegulation", SqlDbType.NVarChar, b.ZoneOfRegulation, 50);
                AddParam(cmd, "@ZoneOfRegulationNumeric", SqlDbType.Int, b.ZoneOfRegulationNumeric);
                AddParam(cmd, "@AcademicEngagement", SqlDbType.Int, b.AcademicEngagement);
                AddParam(cmd, "@SocialInteractions", SqlDbType.Int, b.SocialInteractions);
                AddParam(cmd, "@EmotionalRegulation", SqlDbType.Int, b.EmotionalRegulation);
                AddParam(cmd, "@StaffComments", SqlDbType.NVarChar, b.StaffComments, -1); // NVARCHAR(MAX)
                AddParam(cmd, "@WeeklyEmotionDate", SqlDbType.DateTime2, b.WeeklyEmotionDate);
                AddParam(cmd, "@WeeklyEmotionPictogram", SqlDbType.NVarChar, b.WeeklyEmotionPictogram, 50);
                AddParam(cmd, "@WeeklyEmotionPictogramNumeric", SqlDbType.Int, b.WeeklyEmotionPictogramNumeric);
                AddParam(cmd, "@AggressionRiskNormalized", SqlDbType.Float, b.AggressionRiskNormalized);
                AddParam(cmd, "@EngagementLevelNormalized", SqlDbType.Float, b.EngagementLevelNormalized);
                AddParam(cmd, "@DayInProgram", SqlDbType.Int, b.DayInProgram);
                AddParam(cmd, "@WeekInProgram", SqlDbType.Int, b.WeekInProgram);
                AddParam(cmd, "@CreatedDate", SqlDbType.DateTime2, b.CreatedDate);

                ExecuteWithFkCatch(cmd, "DailyBehavior", b.StudentID);
            }
        }

        public static void BulkInsertExitData(SqlConnection conn, SqlTransaction tx, List<ExitData> exitList)
        {
            if (exitList == null || exitList.Count == 0) return;

            foreach (var x in exitList)
            {
                var sql = $@"
INSERT INTO {DatabaseConfig.TableExitData}
(StudentID, ExitReason, ExitReasonNumeric, ExitDate, LengthOfStay,
 ExitAcademicLevel, ExitAcademicLevelNumeric, ExitSocialSkillsLevel, ExitSocialSkillsLevelNumeric,
 AcademicImprovement, SocialSkillsImprovement, OverallImprovementScore, ProgramEffectivenessScore,
 SuccessIndicator, CreatedDate, ModifiedDate)
VALUES
(@StudentID, @ExitReason, @ExitReasonNumeric, @ExitDate, @LengthOfStay,
 @ExitAcademicLevel, @ExitAcademicLevelNumeric, @ExitSocialSkillsLevel, @ExitSocialSkillsLevelNumeric,
 @AcademicImprovement, @SocialSkillsImprovement, @OverallImprovementScore, @ProgramEffectivenessScore,
 @SuccessIndicator, @CreatedDate, @ModifiedDate);";

                using var cmd = CreateCommand(conn, tx, sql);
                AddParam(cmd, "@StudentID", SqlDbType.Int, x.StudentID); // use BigInt if schema requires
                AddParam(cmd, "@ExitReason", SqlDbType.NVarChar, x.ExitReason, 100);
                AddParam(cmd, "@ExitReasonNumeric", SqlDbType.Int, x.ExitReasonNumeric);
                AddParam(cmd, "@ExitDate", SqlDbType.DateTime2, x.ExitDate);
                AddParam(cmd, "@LengthOfStay", SqlDbType.Int, x.LengthOfStay);
                AddParam(cmd, "@ExitAcademicLevel", SqlDbType.NVarChar, x.ExitAcademicLevel, 50);
                AddParam(cmd, "@ExitAcademicLevelNumeric", SqlDbType.Int, x.ExitAcademicLevelNumeric);
                AddParam(cmd, "@ExitSocialSkillsLevel", SqlDbType.NVarChar, x.ExitSocialSkillsLevel, 50);
                AddParam(cmd, "@ExitSocialSkillsLevelNumeric", SqlDbType.Int, x.ExitSocialSkillsLevelNumeric);
                AddParam(cmd, "@AcademicImprovement", SqlDbType.Int, x.AcademicImprovement);
                AddParam(cmd, "@SocialSkillsImprovement", SqlDbType.Int, x.SocialSkillsImprovement);
                AddParam(cmd, "@OverallImprovementScore", SqlDbType.Float, x.OverallImprovementScore);
                AddParam(cmd, "@ProgramEffectivenessScore", SqlDbType.Float, x.ProgramEffectivenessScore);
                AddParam(cmd, "@SuccessIndicator", SqlDbType.Bit, x.SuccessIndicator);
                AddParam(cmd, "@CreatedDate", SqlDbType.DateTime2, x.CreatedDate);
                AddParam(cmd, "@ModifiedDate", SqlDbType.DateTime2, x.ModifiedDate);

                ExecuteWithFkCatch(cmd, "ExitData", x.StudentID);
            }
        }

        // =========================
        // Helpers
        // =========================

        private static SqlCommand CreateCommand(SqlConnection conn, SqlTransaction tx, string sql)
        {
            var cmd = conn.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            return cmd;
        }

        private static void AddParam(SqlCommand cmd, string name, SqlDbType type, object value, int size = 0)
        {
            var p = cmd.Parameters.Add(name, type);
            if (size != 0) p.Size = size;
            p.Value = ToDb(value);
        }

        // Normalize values -> DBNull where appropriate
        private static object ToDb(object value)
        {
            if (value == null) return DBNull.Value;

            if (value is string s)
                return string.IsNullOrWhiteSpace(s) ? (object)DBNull.Value : s;

            // Allow Bit to accept bool directly
            if (value is bool b) return b;

            if (value is DateTime dt) return dt;

            // Numeric primitives
            if (value is int i) return i;
            if (value is long l) return l;
            if (value is double d) return d;
            if (value is float f) return f;        // if column is REAL; if FLOAT(53), you can cast to (double)f
            if (value is decimal m) return m;

            return value;
        }

        private static void DebugWriteCurrentDb(SqlConnection conn)
        {
            try
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT DB_NAME();";
                var db = cmd.ExecuteScalar() as string ?? "<unknown>";
                Debug.WriteLine($"[DatabaseLoader] Connected to DB: {db}");
            }
            catch { /* ignore */ }
        }

        // ----- FK discovery + pre-checks -----

        private sealed class FkTarget
        {
            public string ParentSchema { get; init; } = "dbo";
            public string ParentTable { get; init; } = "StudentProfile";
            public string ParentColumn { get; init; } = "StudentID";
        }

        // childTableFullName: e.g., "[dbo].[IntakeData]" or "dbo.IntakeData" or "IntakeData"
        private static FkTarget GetFkTargetForChild(SqlConnection conn, SqlTransaction tx, string childTableFullName, string childColumn = "StudentID")
        {
            // Normalize schema/name
            var t = childTableFullName.Trim();
            t = t.Trim('[', ']'); // remove outer brackets if any
            string childSchema = "dbo", childName = t;
            var parts = t.Split('.', 2);
            if (parts.Length == 2)
            {
                childSchema = parts[0].Trim('[', ']');
                childName = parts[1].Trim('[', ']');
            }

            const string sql = @"
SELECT 
    sch_p.name  AS ParentSchema,
    t_p.name    AS ParentTable,
    c_p.name    AS ParentColumn
FROM sys.foreign_keys fk
JOIN sys.foreign_key_columns fkc ON fkc.constraint_object_id = fk.object_id
JOIN sys.tables t_c    ON t_c.object_id  = fk.parent_object_id
JOIN sys.schemas sch_c ON sch_c.schema_id = t_c.schema_id
JOIN sys.columns c_c   ON c_c.object_id = t_c.object_id AND c_c.column_id = fkc.parent_column_id
JOIN sys.tables t_p    ON t_p.object_id  = fk.referenced_object_id
JOIN sys.schemas sch_p ON sch_p.schema_id = t_p.schema_id
JOIN sys.columns c_p   ON c_p.object_id = t_p.object_id AND c_p.column_id = fkc.referenced_column_id
WHERE sch_c.name = @childSchema AND t_c.name = @childTable AND c_c.name = @childColumn;";

            using var cmd = conn.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = sql;
            cmd.Parameters.Add(new SqlParameter("@childSchema", SqlDbType.NVarChar, 128) { Value = childSchema });
            cmd.Parameters.Add(new SqlParameter("@childTable", SqlDbType.NVarChar, 128) { Value = childName });
            cmd.Parameters.Add(new SqlParameter("@childColumn", SqlDbType.NVarChar, 128) { Value = childColumn });

            using var rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                return new FkTarget
                {
                    ParentSchema = rdr.GetString(0),
                    ParentTable = rdr.GetString(1),
                    ParentColumn = rdr.GetString(2)
                };
            }

            // Fallback if no FK metadata found (shouldn't happen if FK exists)
            return new FkTarget();
        }

        private static void AssertStudentIdsExist(SqlConnection conn, SqlTransaction tx, IEnumerable<int> ids, string childTableFullName)
        {
            var fk = GetFkTargetForChild(conn, tx, childTableFullName, "StudentID");
            var unique = new HashSet<int>(ids.Where(id => id > 0));

            var missing = new List<int>();
            using var check = conn.CreateCommand();
            check.Transaction = tx;
            check.CommandText = $"SELECT 1 FROM [{fk.ParentSchema}].[{fk.ParentTable}] WHERE [{fk.ParentColumn}] = @id;";
            var p = check.Parameters.Add("@id", SqlDbType.Int); // switch to BigInt if schema is BIGINT

            foreach (var id in unique)
            {
                p.Value = id;
                var exists = check.ExecuteScalar() != null;
                if (!exists) missing.Add(id);
            }

            if (missing.Count > 0)
            {
                var db = "<unknown>";
                try
                {
                    using var c = conn.CreateCommand();
                    c.Transaction = tx;
                    c.CommandText = "SELECT DB_NAME();";
                    db = c.ExecuteScalar() as string ?? db;
                }
                catch { /* ignore */ }

                var sample = string.Join(", ", missing.Take(10));
                throw new InvalidOperationException(
                    $"FK pre-check failed for {childTableFullName}. Missing {missing.Count} StudentID(s) in [{fk.ParentSchema}].[{fk.ParentTable}] on DB '{db}'. " +
                    $"Sample: {sample}{(missing.Count > 10 ? ", ..." : "")}");
            }
        }

        private static void ExecuteWithFkCatch(SqlCommand cmd, string label, int studentId)
        {
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (SqlException ex) when (ex.Number == 547) // FK violation
            {
                throw new InvalidOperationException(
                    $"Foreign key violation while inserting into {label} for StudentID {studentId}. " +
                    $"Verify StudentIDs map to DB identities and exist in the FK parent table.", ex);
            }
        }
    }
}
