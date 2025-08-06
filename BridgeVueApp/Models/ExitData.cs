using System;

namespace BridgeVueApp.Models
{
    public class ExitData
    {
        public int ExitID { get; set; }
        public int StudentID { get; set; }
        public string ExitReason { get; set; }
        public int ExitReasonNumeric { get; set; }
        public DateTime ExitDate { get; set; }
        public int LengthOfStay { get; set; }
        public string ExitAcademicLevel { get; set; }
        public int ExitAcademicLevelNumeric { get; set; }
        public string ExitSocialSkillsLevel { get; set; }
        public int ExitSocialSkillsLevelNumeric { get; set; }
        public int AcademicImprovement { get; set; }
        public int SocialSkillsImprovement { get; set; }
        public float OverallImprovementScore { get; set; }
        public float ProgramEffectivenessScore { get; set; }
        public bool SuccessIndicator { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

}
