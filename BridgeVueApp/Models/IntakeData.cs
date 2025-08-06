using System;

namespace BridgeVueApp.Models
{
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
        public bool CheckInOut { get; set; }
        public bool StructuredRecess { get; set; }
        public bool StructuredBreaks { get; set; }
        public int SmallGroups { get; set; }
        public int SocialWorkerVisits { get; set; }
        public int PsychologistVisits { get; set; }
        public string EntrySocialSkillsLevel { get; set; }
        public int EntrySocialSkillsLevelNumeric { get; set; }
        public DateTime EntryDate { get; set; }
        public int RiskScore { get; set; }
        public float StudentStressLevelNormalized { get; set; }
        public float FamilySupportNormalized { get; set; }
        public float AcademicAbilityNormalized { get; set; }
        public float EmotionalRegulationNormalized { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

}

