using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgeVueApp.Database
{
    public static class DatabaseConfig
    {
        // Database name and tables
        public const string DbName = "BridgeVue";
        public const string TableStudentProfile = "StudentProfile";
        public const string TableIntakeData = "IntakeData";
        public const string TableDailyBehavior = "DailyBehavior";
        public const string TableExitData = "ExitData";
        public const string TableModelPerformance = "ModelPerformance";
        public const string TableModelMetricsHistory = "ModelMetricsHistory";
        public const string TableDatasetSnapshot = "DatasetSnapshot";
        public const string TableModelDataUsage = "ModelDataUsage";
        public const string TableInferenceLog = "InferenceLog";
        public const string vStudentPredictionData = "vStudentPredictionData";
        public const string vStudentMLDataRaw = "vStudentMLDataRaw";
        public const string vStudentMLTrainingData = "vStudentMLTrainingData";
        public const string vStudentMLScoringData = "vStudentMLScoringData";



        // Connection strings
        public static string BaseConnection =>
            "Server=localhost;Integrated Security=true;TrustServerCertificate=True;";

        public static string FullConnection =>
            $"Server=localhost;Database={DbName};Integrated Security=true;TrustServerCertificate=True;Encrypt=False;";
            //$"Server=localhost;Database={DbName};Trusted_Connection=True;Encrypt=False;";
    }
}

