using Microsoft.ML.Data;

namespace BridgeVueApp.MachineLearning
{
    public class ModelInput
    {
        public int Grade { get; set; }
        public int Age { get; set; }
        public int GenderNumeric { get; set; }
        public int EthnicityNumeric { get; set; }
        public bool SpecialEd { get; set; }
        public bool IEP { get; set; }
        public int EntryReasonNumeric { get; set; }
        public int PriorIncidents { get; set; }
        public int OfficeReferrals { get; set; }
        public int Suspensions { get; set; }
        public int Expulsions { get; set; }
        public int EntryAcademicLevelNumeric { get; set; }
        public bool CheckInOut { get; set; }
        public bool StructuredRecess { get; set; }
        public bool StructuredBreaks { get; set; }
        public int SmallGroups { get; set; }
        public int SocialWorkerVisits { get; set; }
        public int PsychologistVisits { get; set; }
        public int EntrySocialSkillsLevelNumeric { get; set; }
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
        public int BehaviorDays { get; set; }

        [LoadColumn(999)] // ML.NET needs this to mark the prediction target
        public bool DidSucceed { get; set; }
    }
}

