namespace BridgeVueApp.MachineLearning
{
    public class ModelPerformance
    {
        // DB identity (optional to set at insert time)
        public int ModelID { get; set; }


        // Core run info
        public DateTime TrainingDate { get; set; }             // UTC recommended
        public string ModelName { get; set; }                  // NOT NULL in DB


        // New fields you tried to use:
        public string? ModelType { get; set; }                 // e.g., "ML.NET BinaryClassification (AutoML)"
        public string? Hyperparameters { get; set; }           // JSON text


        // Use the same name as your SQL column:
        public int? TrainingDurationSec { get; set; }          // seconds


        // Metrics (nullable is fine if you sometimes don’t have them)
        public float? Accuracy { get; set; }
        public float? F1Score { get; set; }
        public float? AUC { get; set; }
        public float? Precision { get; set; }
        public float? Recall { get; set; }


        // Sizes
        public int TrainingDataSize { get; set; }
        public int TestDataSize { get; set; }


        // Flags & artifact path
        public bool IsCurrentBest { get; set; }
        public string? ModelFilePath { get; set; }
    }
}

