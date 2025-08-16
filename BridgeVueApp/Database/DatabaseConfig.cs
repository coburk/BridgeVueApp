using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgeVueApp.Database
{
    public static class DatabaseConfig
    {
        public const string DbName = "BridgeVue";

        // Connections
        public static string BaseConnection =>
            "Server=localhost;Integrated Security=True;TrustServerCertificate=True;";
        public static string FullConnection =>
            $"Server=localhost;Database={DbName};Integrated Security=True;TrustServerCertificate=True;";

        // Core tables
        public const string TableStudentProfile = "dbo.StudentProfile";
        public const string TableIntakeData = "dbo.IntakeData";
        public const string TableDailyBehavior = "dbo.DailyBehavior";
        public const string TableExitData = "dbo.ExitData";

        // ML tracking tables
        public const string TableModelPerformance = "dbo.ModelPerformance";
        public const string TableModelMetricsHistory = "dbo.ModelMetricsHistory";
        public const string TableDatasetSnapshot = "dbo.DatasetSnapshot";      // used in ModelTrainer
        public const string TableModelDataUsage = "dbo.ModelDataUsage";       // used in ModelTrainer.ModelTracking
        public const string TableInferenceLog = "dbo.InferenceLog";         // batch scoring writes here

        // Views
        public const string vStudentMLDataRaw = "dbo.vStudentMLDataRaw";
        public const string vStudentPredictionData = "dbo.vStudentPredictionData";
        public const string vStudentMLTrainingData = "dbo.vStudentMLTrainingData"; 
    }

}

