using System;

namespace BridgeVueApp.Models
{
    public class DailyBehavior
    {
        public int BehaviorID { get; set; }
        public int StudentID { get; set; }
        public DateTime Timestamp { get; set; }
        public int Level { get; set; }
        public int Step { get; set; }
        public int VerbalAggression { get; set; }
        public int PhysicalAggression { get; set; }
        public int Elopement { get; set; }
        public int OutOfSpot { get; set; }
        public int WorkRefusal { get; set; }
        public int ProvokingPeers { get; set; }
        public int InappropriateLanguage { get; set; }
        public int OutOfLane { get; set; }
        public string ZoneOfRegulation { get; set; }
        public int ZoneOfRegulationNumeric { get; set; }
        public int AcademicEngagement { get; set; }
        public int SocialInteractions { get; set; }
        public int EmotionalRegulation { get; set; }
        public float AggressionRiskNormalized { get; set; }
        public float EngagementLevelNormalized { get; set; }
        public int DayInProgram { get; set; }
        public int WeekInProgram { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

}
