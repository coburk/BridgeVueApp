using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using BridgeVueApp.Models;
using BridgeVueApp.Database;


namespace BridgeVueApp.Database


{
    public static class DatabaseLoader
    {
        public static List<StudentProfile> BulkInsertStudentProfiles(List<StudentProfile> profiles)
        {
            var inserted = new List<StudentProfile>();

            using (SqlConnection conn = new SqlConnection(DatabaseConfig.FullConnection))
            {
                conn.Open();

                foreach (var profile in profiles)
                {
                    string query = $@"
                INSERT INTO {DatabaseConfig.TableStudentProfile} (FirstName, LastName, Grade, Age, Gender, GenderNumeric, 
                    Ethnicity, EthnicityNumeric, SpecialEd, IEP, HasKnownOutcome, DidSucceed, CreatedDate, ModifiedDate)
                VALUES (@FirstName, @LastName, @Grade, @Age, @Gender, @GenderNumeric, 
                    @Ethnicity, @EthnicityNumeric, @SpecialEd, @IEP, @HasKnownOutcome, @DidSucceed, @CreatedDate, @ModifiedDate);";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@FirstName", profile.FirstName);
                        cmd.Parameters.AddWithValue("@LastName", profile.LastName);
                        cmd.Parameters.AddWithValue("@Grade", profile.Grade);
                        cmd.Parameters.AddWithValue("@Age", profile.Age);
                        cmd.Parameters.AddWithValue("@Gender", profile.Gender);
                        cmd.Parameters.AddWithValue("@GenderNumeric", profile.GenderNumeric);
                        cmd.Parameters.AddWithValue("@Ethnicity", profile.Ethnicity);
                        cmd.Parameters.AddWithValue("@EthnicityNumeric", profile.EthnicityNumeric);
                        cmd.Parameters.AddWithValue("@SpecialEd", profile.SpecialEd);
                        cmd.Parameters.AddWithValue("@IEP", profile.IEP);
                        cmd.Parameters.AddWithValue("@HasKnownOutcome", profile.HasKnownOutcome);
                        cmd.Parameters.AddWithValue("@DidSucceed", profile.DidSucceed ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@CreatedDate", profile.CreatedDate);
                        cmd.Parameters.AddWithValue("@ModifiedDate", profile.ModifiedDate);
                        cmd.ExecuteNonQuery();
                    }

                    inserted.Add(profile);
                }
            }

            return inserted;
        }


        public static void BulkInsertIntakeData(List<IntakeData> intakeList)
        {
            using (SqlConnection conn = new SqlConnection(DatabaseConfig.FullConnection))
            {
                conn.Open();
                foreach (var intake in intakeList)
                {
                    string query = $@"
                        INSERT INTO {DatabaseConfig.TableIntakeData} (StudentID, EntryReason, EntryReasonNumeric, PriorIncidents,
                            OfficeReferrals, Suspensions, Expulsions, EntryAcademicLevel, EntryAcademicLevelNumeric,
                            CheckInOut, StructuredRecess, StructuredBreaks, SmallGroups, SocialWorkerVisits,
                            PsychologistVisits, EntrySocialSkillsLevel, EntrySocialSkillsLevelNumeric, EntryDate,
                            RiskScore, StudentStressLevelNormalized, FamilySupportNormalized, AcademicAbilityNormalized,
                            EmotionalRegulationNormalized, CreatedDate, ModifiedDate)
                        VALUES (@StudentID, @EntryReason, @EntryReasonNumeric, @PriorIncidents, @OfficeReferrals,
                            @Suspensions, @Expulsions, @EntryAcademicLevel, @EntryAcademicLevelNumeric,
                            @CheckInOut, @StructuredRecess, @StructuredBreaks, @SmallGroups, @SocialWorkerVisits,
                            @PsychologistVisits, @EntrySocialSkillsLevel, @EntrySocialSkillsLevelNumeric, @EntryDate,
                            @RiskScore, @StudentStressLevelNormalized, @FamilySupportNormalized, @AcademicAbilityNormalized,
                            @EmotionalRegulationNormalized, @CreatedDate, @ModifiedDate);";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@StudentID", intake.StudentID);
                        cmd.Parameters.AddWithValue("@EntryReason", intake.EntryReason);
                        cmd.Parameters.AddWithValue("@EntryReasonNumeric", intake.EntryReasonNumeric);
                        cmd.Parameters.AddWithValue("@PriorIncidents", intake.PriorIncidents);
                        cmd.Parameters.AddWithValue("@OfficeReferrals", intake.OfficeReferrals);
                        cmd.Parameters.AddWithValue("@Suspensions", intake.Suspensions);
                        cmd.Parameters.AddWithValue("@Expulsions", intake.Expulsions);
                        cmd.Parameters.AddWithValue("@EntryAcademicLevel", intake.EntryAcademicLevel);
                        cmd.Parameters.AddWithValue("@EntryAcademicLevelNumeric", intake.EntryAcademicLevelNumeric);
                        cmd.Parameters.AddWithValue("@CheckInOut", intake.CheckInOut);
                        cmd.Parameters.AddWithValue("@StructuredRecess", intake.StructuredRecess);
                        cmd.Parameters.AddWithValue("@StructuredBreaks", intake.StructuredBreaks);
                        cmd.Parameters.AddWithValue("@SmallGroups", intake.SmallGroups);
                        cmd.Parameters.AddWithValue("@SocialWorkerVisits", intake.SocialWorkerVisits);
                        cmd.Parameters.AddWithValue("@PsychologistVisits", intake.PsychologistVisits);
                        cmd.Parameters.AddWithValue("@EntrySocialSkillsLevel", intake.EntrySocialSkillsLevel);
                        cmd.Parameters.AddWithValue("@EntrySocialSkillsLevelNumeric", intake.EntrySocialSkillsLevelNumeric);
                        cmd.Parameters.AddWithValue("@EntryDate", intake.EntryDate);
                        cmd.Parameters.AddWithValue("@RiskScore", intake.RiskScore);
                        cmd.Parameters.AddWithValue("@StudentStressLevelNormalized", intake.StudentStressLevelNormalized);
                        cmd.Parameters.AddWithValue("@FamilySupportNormalized", intake.FamilySupportNormalized);
                        cmd.Parameters.AddWithValue("@AcademicAbilityNormalized", intake.AcademicAbilityNormalized);
                        cmd.Parameters.AddWithValue("@EmotionalRegulationNormalized", intake.EmotionalRegulationNormalized);
                        cmd.Parameters.AddWithValue("@CreatedDate", intake.CreatedDate);
                        cmd.Parameters.AddWithValue("@ModifiedDate", intake.ModifiedDate);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public static void BulkInsertDailyBehavior(List<DailyBehavior> behaviors)
        {
            using (SqlConnection conn = new SqlConnection(DatabaseConfig.FullConnection))
            {
                conn.Open();

                foreach (var b in behaviors)
                {
                    string query = $@"
                        INSERT INTO {DatabaseConfig.TableDailyBehavior} (StudentID, Timestamp, Level, Step, VerbalAggression,
                            PhysicalAggression, Elopement, OutOfSpot, WorkRefusal, ProvokingPeers, InappropriateLanguage,
                            OutOfLane, ZoneOfRegulation, ZoneOfRegulationNumeric, AcademicEngagement, SocialInteractions,
                            EmotionalRegulation, StaffComments, WeeklyEmotionDate, WeeklyEmotionPictogram,
                            WeeklyEmotionPictogramNumeric, AggressionRiskNormalized, EngagementLevelNormalized,
                            DayInProgram, WeekInProgram, CreatedDate)
                        VALUES (@StudentID, @Timestamp, @Level, @Step, @VerbalAggression, @PhysicalAggression,
                            @Elopement, @OutOfSpot, @WorkRefusal, @ProvokingPeers, @InappropriateLanguage,
                            @OutOfLane, @ZoneOfRegulation, @ZoneOfRegulationNumeric, @AcademicEngagement, @SocialInteractions,
                            @EmotionalRegulation, @StaffComments, @WeeklyEmotionDate, @WeeklyEmotionPictogram,
                            @WeeklyEmotionPictogramNumeric, @AggressionRiskNormalized, @EngagementLevelNormalized,
                            @DayInProgram, @WeekInProgram, @CreatedDate);";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@StudentID", b.StudentID);
                        cmd.Parameters.AddWithValue("@Timestamp", b.Timestamp);
                        cmd.Parameters.AddWithValue("@Level", b.Level);
                        cmd.Parameters.AddWithValue("@Step", b.Step);
                        cmd.Parameters.AddWithValue("@VerbalAggression", b.VerbalAggression);
                        cmd.Parameters.AddWithValue("@PhysicalAggression", b.PhysicalAggression);
                        cmd.Parameters.AddWithValue("@Elopement", b.Elopement);
                        cmd.Parameters.AddWithValue("@OutOfSpot", b.OutOfSpot);
                        cmd.Parameters.AddWithValue("@WorkRefusal", b.WorkRefusal);
                        cmd.Parameters.AddWithValue("@ProvokingPeers", b.ProvokingPeers);
                        cmd.Parameters.AddWithValue("@InappropriateLanguage", b.InappropriateLanguage);
                        cmd.Parameters.AddWithValue("@OutOfLane", b.OutOfLane);
                        cmd.Parameters.AddWithValue("@ZoneOfRegulation", b.ZoneOfRegulation ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ZoneOfRegulationNumeric", b.ZoneOfRegulationNumeric);
                        cmd.Parameters.AddWithValue("@AcademicEngagement", b.AcademicEngagement);
                        cmd.Parameters.AddWithValue("@SocialInteractions", b.SocialInteractions);
                        cmd.Parameters.AddWithValue("@EmotionalRegulation", b.EmotionalRegulation);
                        cmd.Parameters.AddWithValue("@StaffComments", b.StaffComments ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@WeeklyEmotionDate", (object)b.WeeklyEmotionDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@WeeklyEmotionPictogram", b.WeeklyEmotionPictogram ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@WeeklyEmotionPictogramNumeric", (object)b.WeeklyEmotionPictogramNumeric ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AggressionRiskNormalized", b.AggressionRiskNormalized);
                        cmd.Parameters.AddWithValue("@EngagementLevelNormalized", b.EngagementLevelNormalized);
                        cmd.Parameters.AddWithValue("@DayInProgram", b.DayInProgram);
                        cmd.Parameters.AddWithValue("@WeekInProgram", b.WeekInProgram);
                        cmd.Parameters.AddWithValue("@CreatedDate", b.CreatedDate);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public static void BulkInsertExitData(List<ExitData> exitList)
        {
            using (SqlConnection conn = new SqlConnection(DatabaseConfig.FullConnection))
            {
                conn.Open();
                foreach (var exit in exitList)
                {
                    string query = $@"
                        INSERT INTO {DatabaseConfig.TableExitData} (StudentID, ExitReason, ExitReasonNumeric, ExitDate, LengthOfStay,
                            ExitAcademicLevel, ExitAcademicLevelNumeric, ExitSocialSkillsLevel, ExitSocialSkillsLevelNumeric,
                            AcademicImprovement, SocialSkillsImprovement, OverallImprovementScore, ProgramEffectivenessScore,
                            SuccessIndicator, CreatedDate, ModifiedDate)
                        VALUES (@StudentID, @ExitReason, @ExitReasonNumeric, @ExitDate, @LengthOfStay,
                            @ExitAcademicLevel, @ExitAcademicLevelNumeric, @ExitSocialSkillsLevel, @ExitSocialSkillsLevelNumeric,
                            @AcademicImprovement, @SocialSkillsImprovement, @OverallImprovementScore,
                            @ProgramEffectivenessScore, @SuccessIndicator, @CreatedDate, @ModifiedDate);";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@StudentID", exit.StudentID);
                        cmd.Parameters.AddWithValue("@ExitReason", exit.ExitReason);
                        cmd.Parameters.AddWithValue("@ExitReasonNumeric", exit.ExitReasonNumeric);
                        cmd.Parameters.AddWithValue("@ExitDate", exit.ExitDate);
                        cmd.Parameters.AddWithValue("@LengthOfStay", exit.LengthOfStay);
                        cmd.Parameters.AddWithValue("@ExitAcademicLevel", exit.ExitAcademicLevel);
                        cmd.Parameters.AddWithValue("@ExitAcademicLevelNumeric", exit.ExitAcademicLevelNumeric);
                        cmd.Parameters.AddWithValue("@ExitSocialSkillsLevel", exit.ExitSocialSkillsLevel);
                        cmd.Parameters.AddWithValue("@ExitSocialSkillsLevelNumeric", exit.ExitSocialSkillsLevelNumeric);
                        cmd.Parameters.AddWithValue("@AcademicImprovement", exit.AcademicImprovement);
                        cmd.Parameters.AddWithValue("@SocialSkillsImprovement", exit.SocialSkillsImprovement);
                        cmd.Parameters.AddWithValue("@OverallImprovementScore", exit.OverallImprovementScore);
                        cmd.Parameters.AddWithValue("@ProgramEffectivenessScore", exit.ProgramEffectivenessScore);
                        cmd.Parameters.AddWithValue("@SuccessIndicator", exit.SuccessIndicator);
                        cmd.Parameters.AddWithValue("@CreatedDate", exit.CreatedDate);
                        cmd.Parameters.AddWithValue("@ModifiedDate", exit.ModifiedDate);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
