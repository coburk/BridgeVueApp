using Microsoft.Data.SqlClient;
using BridgeVueApp.Data;

namespace BridgeVueApp.MachineLearning
{
    public static class ModelInputFactory
    {
        // Fast path for batch: pass an OrdinalCache you created once per reader
        public static ML_Class_Success.ModelInput FromReader(SqlDataReader reader, OrdinalCache ords)
        {
            return new ML_Class_Success.ModelInput
            {
                Grade = reader.GetInt32OrDefault(ords, "Grade"),
                Age = reader.GetInt32OrDefault(ords, "Age"),
                GenderNumeric = reader.GetInt32OrDefault(ords, "GenderNumeric"),
                EthnicityNumeric = reader.GetInt32OrDefault(ords, "EthnicityNumeric"),
                SpecialEd = reader.GetBooleanOrDefault(ords, "SpecialEd"),
                IEP = reader.GetBooleanOrDefault(ords, "IEP"),
                EntryReasonNumeric = reader.GetInt32OrDefault(ords, "EntryReasonNumeric"),
                PriorIncidents = reader.GetInt32OrDefault(ords, "PriorIncidents"),
                OfficeReferrals = reader.GetInt32OrDefault(ords, "OfficeReferrals"),
                Suspensions = reader.GetInt32OrDefault(ords, "Suspensions"),
                Expulsions = reader.GetInt32OrDefault(ords, "Expulsions"),
                EntryAcademicLevelNumeric = reader.GetInt32OrDefault(ords, "EntryAcademicLevelNumeric"),
                CheckInOut = reader.GetBooleanOrDefault(ords, "CheckInOut"),
                StructuredRecess = reader.GetBooleanOrDefault(ords, "StructuredRecess"),
                StructuredBreaks = reader.GetBooleanOrDefault(ords, "StructuredBreaks"),
                SmallGroups = reader.GetInt32OrDefault(ords, "SmallGroups"),
                SocialWorkerVisits = reader.GetInt32OrDefault(ords, "SocialWorkerVisits"),
                PsychologistVisits = reader.GetInt32OrDefault(ords, "PsychologistVisits"),
                EntrySocialSkillsLevelNumeric = reader.GetInt32OrDefault(ords, "EntrySocialSkillsLevelNumeric"),
                RiskScore = reader.GetSingleOrDefault(ords, "RiskScore"),
                StudentStressLevelNormalized = reader.GetSingleOrDefault(ords, "StudentStressLevelNormalized"),
                FamilySupportNormalized = reader.GetSingleOrDefault(ords, "FamilySupportNormalized"),
                AcademicAbilityNormalized = reader.GetSingleOrDefault(ords, "AcademicAbilityNormalized"),
                EmotionalRegulationNormalized = reader.GetSingleOrDefault(ords, "EmotionalRegulationNormalized"),
                AvgVerbalAggression = reader.GetSingleOrDefault(ords, "AvgVerbalAggression"),
                AvgPhysicalAggression = reader.GetSingleOrDefault(ords, "AvgPhysicalAggression"),
                AvgAcademicEngagement = reader.GetSingleOrDefault(ords, "AvgAcademicEngagement"),
                AvgSocialInteractions = reader.GetSingleOrDefault(ords, "AvgSocialInteractions"),
                AvgEmotionalRegulation = reader.GetSingleOrDefault(ords, "AvgEmotionalRegulation"),
                AvgAggressionRisk = reader.GetSingleOrDefault(ords, "AvgAggressionRisk"),
                AvgEngagementLevel = reader.GetSingleOrDefault(ords, "AvgEngagementLevel"),
                RedZonePct = reader.GetSingleOrDefault(ords, "RedZonePct"),
                YellowZonePct = reader.GetSingleOrDefault(ords, "YellowZonePct"),
                BlueZonePct = reader.GetSingleOrDefault(ords, "BlueZonePct"),
                GreenZonePct = reader.GetSingleOrDefault(ords, "GreenZonePct"),
                BehaviorDays = reader.GetInt32OrDefault(ords, "BehaviorDays")
            };
        }

        // Convenience for single-row reads (creates a local cache)
        public static ML_Class_Success.ModelInput FromReader(SqlDataReader reader)
        {
            var ords = new OrdinalCache(reader);
            return FromReader(reader, ords);
        }
    }
}

