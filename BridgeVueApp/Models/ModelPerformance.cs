namespace BridgeVueApp.Models
{
    public class ModelPerformance
    {
        public string ModelName { get; set; }
        public DateTime TrainingDate { get; set; }
        public float Accuracy { get; set; }
        public float F1Score { get; set; }
        public float AUC { get; set; }
        public float Precision { get; set; }
        public float Recall { get; set; }
        public int TrainingDataSize { get; set; }
        public int TestDataSize { get; set; }
        public bool IsCurrentBest { get; set; }
        public string ModelFilePath { get; set; }
    }
}
