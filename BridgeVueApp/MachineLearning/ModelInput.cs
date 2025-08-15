using Microsoft.ML.Data;

namespace BridgeVueApp.MachineLearning
{
    public class ModelInput
    {
        public float Grade { get; set; }
        public float Age { get; set; }
        public float GenderNumeric { get; set; }
        public float EthnicityNumeric { get; set; }
        public float EntryReasonNumeric { get; set; }
        public float PriorIncidents { get; set; }
        public float OfficeReferrals { get; set; }
        public float Suspensions { get; set; }
        public float Expulsions { get; set; }
        public float EntryAcademicLevelNumeric { get; set; }
        public float SmallGroups { get; set; }
        public float SocialWorkerVisits { get; set; }
        public float PsychologistVisits { get; set; }
        public float EntrySocialSkillsLevelNumeric { get; set; }
        public float BehaviorDays { get; set; }
        public bool SpecialEd { get; set; }
        public bool IEP { get; set; }
        public bool CheckInOut { get; set; }
        public bool StructuredRecess { get; set; }
        public bool StructuredBreaks { get; set; }
        public float RiskScore { get; set; }
        public float StudentStressLevelNormalized { get; set; }
        public float FamilySupportNormalized { get; set; }
        public float AcademicAbilityNormalized { get; set; }
        public float EmotionalRegulationNormalized { get; set; }
        public float AvgVerbalAggression { get; set; }
        public float AvgPhysicalAggression { get; set; }
        public float AvgAcademicEngagement { get; set; }
        public float AvgSocialInteractions { get; set; }
        public float AvgEmotionalRegulation { get; set; }
        public float AvgAggressionRisk { get; set; }
        public float AvgEngagementLevel { get; set; }
        public float RedZonePct { get; set; }
        public float YellowZonePct { get; set; }
        public float BlueZonePct { get; set; }
        public float GreenZonePct { get; set; }
        

        [LoadColumn(999)] // ML.NET needs this to mark the prediction target
        public bool DidSucceed { get; set; }
    }
}

