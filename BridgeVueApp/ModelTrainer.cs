using Microsoft.ML;
using Microsoft.ML.AutoML;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.IO;
using BridgeVueApp.Database;

namespace BridgeVueApp
{
    public class ModelTrainer
    {
        private static string connectionString = DatabaseConfig.FullConnection;
        private static string modelDirectory = @"Models\"; // Ensure this folder exists

        public static void TrainAndLogModel(IDataView trainData, IDataView testData)
        {
            var mlContext = new MLContext(seed: 0);

            // Train using AutoML
            var experiment = mlContext.Auto().CreateBinaryClassificationExperiment(maxExperimentTimeInSeconds: 60);
            var result = experiment.Execute(trainData, labelColumnName: "DidSucceed");

            var bestRun = result.BestRun;
            var metrics = bestRun.ValidationMetrics;
            string algorithmName = bestRun.TrainerName;
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string modelPath = Path.Combine(modelDirectory, $"Model_{algorithmName}_{timestamp}.zip");

            // Save the best model
            mlContext.Model.Save(bestRun.Model, trainData.Schema, modelPath);

            var performance = new ModelPerformance
            {
                ModelName = algorithmName,
                TrainingDate = DateTime.Now,
                Accuracy = (float)metrics.Accuracy,
                F1Score = (float)metrics.F1Score,
                AUC = (float)metrics.AreaUnderRocCurve,
                Precision = (float)metrics.PositivePrecision,
                Recall = (float)metrics.PositiveRecall,
                TrainingDataSize = (int)(trainData.GetRowCount() ?? 0),
                TestDataSize = (int)(testData.GetRowCount() ?? 0),
                IsCurrentBest = true,
                ModelFilePath = modelPath
            };

            SaveModelPerformance(performance);
            Console.WriteLine($"\n✅ Best Model '{algorithmName}' saved and logged successfully.\n");
        }

        private static void SaveModelPerformance(ModelPerformance perf)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Optionally reset previous bests
                string resetSql = $"UPDATE {DatabaseConfig.TableModelPerformances} SET IsCurrentBest = 0 WHERE IsCurrentBest = 1";
                using (var resetCmd = new SqlCommand(resetSql, conn))
                {
                    resetCmd.ExecuteNonQuery();
                }

                string insertSql = $@"
                    INSERT INTO {DatabaseConfig.TableModelPerformances} (
                        ModelName, TrainingDate, Accuracy, F1Score, AUC, Precision, Recall,
                        TrainingDataSize, TestDataSize, IsCurrentBest, ModelFilePath
                    )
                    VALUES (
                        @ModelName, @TrainingDate, @Accuracy, @F1Score, @AUC, @Precision, @Recall,
                        @TrainingDataSize, @TestDataSize, @IsCurrentBest, @ModelFilePath
                    );";

                using (var cmd = new SqlCommand(insertSql, conn))
                {
                    cmd.Parameters.AddWithValue("@ModelName", perf.ModelName);
                    cmd.Parameters.AddWithValue("@TrainingDate", perf.TrainingDate);
                    cmd.Parameters.AddWithValue("@Accuracy", perf.Accuracy);
                    cmd.Parameters.AddWithValue("@F1Score", perf.F1Score);
                    cmd.Parameters.AddWithValue("@AUC", perf.AUC);
                    cmd.Parameters.AddWithValue("@Precision", perf.Precision);
                    cmd.Parameters.AddWithValue("@Recall", perf.Recall);
                    cmd.Parameters.AddWithValue("@TrainingDataSize", perf.TrainingDataSize);
                    cmd.Parameters.AddWithValue("@TestDataSize", perf.TestDataSize);
                    cmd.Parameters.AddWithValue("@IsCurrentBest", perf.IsCurrentBest);
                    cmd.Parameters.AddWithValue("@ModelFilePath", perf.ModelFilePath);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static List<ModelInput> LoadTrainingDataFromDatabase()
        {
            List<ModelInput> data = new List<ModelInput>();

            string sql = "SELECT * FROM vStudentMLData WHERE HasKnownOutcome = 1 ORDER BY StudentID ASC";
            using (SqlConnection conn = new SqlConnection(DatabaseConfig.FullConnection))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        data.Add(new ModelInput
                        {
                            Grade = reader.GetInt32(reader.GetOrdinal("Grade")),
                            Age = reader.GetInt32(reader.GetOrdinal("Age")),
                            GenderNumeric = reader.GetInt32(reader.GetOrdinal("GenderNumeric")),
                            EthnicityNumeric = reader.GetInt32(reader.GetOrdinal("EthnicityNumeric")),
                            SpecialEd = reader.GetBoolean(reader.GetOrdinal("SpecialEd")),
                            IEP = reader.GetBoolean(reader.GetOrdinal("IEP")),
                            EntryReasonNumeric = reader.GetInt32(reader.GetOrdinal("EntryReasonNumeric")),
                            PriorIncidents = reader.GetInt32(reader.GetOrdinal("PriorIncidents")),
                            OfficeReferrals = reader.GetInt32(reader.GetOrdinal("OfficeReferrals")),
                            Suspensions = reader.GetInt32(reader.GetOrdinal("Suspensions")),
                            Expulsions = reader.GetInt32(reader.GetOrdinal("Expulsions")),
                            EntryAcademicLevelNumeric = reader.GetInt32(reader.GetOrdinal("EntryAcademicLevelNumeric")),
                            CheckInOut = reader.GetBoolean(reader.GetOrdinal("CheckInOut")),
                            StructuredRecess = reader.GetBoolean(reader.GetOrdinal("StructuredRecess")),
                            StructuredBreaks = reader.GetBoolean(reader.GetOrdinal("StructuredBreaks")),
                            SmallGroups = reader.GetInt32(reader.GetOrdinal("SmallGroups")),
                            SocialWorkerVisits = reader.GetInt32(reader.GetOrdinal("SocialWorkerVisits")),
                            PsychologistVisits = reader.GetInt32(reader.GetOrdinal("PsychologistVisits")),
                            EntrySocialSkillsLevelNumeric = reader.GetInt32(reader.GetOrdinal("EntrySocialSkillsLevelNumeric")),
                            RiskScore = Convert.ToSingle(reader["RiskScore"]),
                            StudentStressLevelNormalized = Convert.ToSingle(reader["StudentStressLevelNormalized"]),
                            FamilySupportNormalized = Convert.ToSingle(reader["FamilySupportNormalized"]),
                            AcademicAbilityNormalized = Convert.ToSingle(reader["AcademicAbilityNormalized"]),
                            EmotionalRegulationNormalized = Convert.ToSingle(reader["EmotionalRegulationNormalized"]),
                            AvgVerbalAggression = Convert.ToSingle(reader["AvgVerbalAggression"]),
                            AvgPhysicalAggression = Convert.ToSingle(reader["AvgPhysicalAggression"]),
                            AvgAcademicEngagement = Convert.ToSingle(reader["AvgAcademicEngagement"]),
                            AvgSocialInteractions = Convert.ToSingle(reader["AvgSocialInteractions"]),
                            AvgEmotionalRegulation = Convert.ToSingle(reader["AvgEmotionalRegulation"]),
                            AvgAggressionRisk = Convert.ToSingle(reader["AvgAggressionRisk"]),
                            AvgEngagementLevel = Convert.ToSingle(reader["AvgEngagementLevel"]),
                            RedZonePct = Convert.ToSingle(reader["RedZonePct"]),
                            YellowZonePct = Convert.ToSingle(reader["YellowZonePct"]),
                            BlueZonePct = Convert.ToSingle(reader["BlueZonePct"]),
                            GreenZonePct = Convert.ToSingle(reader["GreenZonePct"]),
                            BehaviorDays = reader.GetInt32(reader.GetOrdinal("BehaviorDays")),
                            DidSucceed = reader.GetBoolean(reader.GetOrdinal("DidSucceed"))
                        });
                    }
                }
            }

            return data;
        }
    }
}
