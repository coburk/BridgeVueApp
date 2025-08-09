// DataGenerationUtils.cs
using System.Collections.Generic;
using System;
using System.Linq;
using static BridgeVueApp.DataGeneration.SyntheticDataGenerator;
using BridgeVueApp.Models;

namespace BridgeVueApp.DataGeneration
{

    public static class DataGenerationUtils
    {
        // Hold generated data in memory
        public static List<StudentProfile> GeneratedProfiles { get; set; } = new();
        public static List<IntakeData> GeneratedIntake { get; set; } = new();
        public static List<DailyBehavior> GeneratedBehavior { get; set; } = new();
        public static List<ExitData> GeneratedExitData { get; set; } = new();
        public static ExitSummary LastExitSummary { get; set; } = new();


        public static int GetGenderNumeric(string gender) => gender switch
        {
            "Male" => 1,
            "Female" => 2,
            _ => 0
        };

        public static int GetEthnicityNumeric(string ethnicity) => ethnicity switch
        {
            "White" => 1,
            "Black" => 2,
            "Hispanic" => 3,
            "Asian" => 4,
            "Other" => 5,
            _ => 0
        };

        public static int GetEntryReasonNumeric(string reason) => reason switch
        {
            "Aggression" => 1,
            "Anxiety" => 2,
            "Trauma" => 3,
            "Withdrawn" => 4,
            "Disruptive" => 5,
            "Other" => 6,
            _ => 0
        };

        public static int GetAcademicLevelNumeric(string level) => level switch
        {
            "Below Grade" => 1,
            "At Grade" => 2,
            "Above Grade" => 3,
            _ => 0
        };

        public static int GetSocialSkillsLevelNumeric(string level) => level switch
        {
            "Low" => 1,
            "Medium" => 2,
            "High" => 3,
            _ => 0
        };

        public static int GetZoneNumeric(string zone) => zone switch
        {
            "Red" => 1,
            "Yellow" => 2,
            "Blue" => 3,
            "Green" => 4,
            _ => 0
        };

        public static int GetEmotionNumeric(string emotion) => emotion switch
        {
            "Happy" => 5,
            "Calm" => 4,
            "Anxious" => 3,
            "Angry" => 2,
            "Sad" => 1,
            _ => 0
        };

        public static int GetExitReasonNumeric(string reason) => reason switch
        {
            "Returned Successfully" => 5,
            "ACC" => 4,
            "Transferred" => 3,
            "ABS" => 2,
            "Referred Out" => 1,
            _ => 0
        };

        public static int GetSuccessIndicator(string exitReason) => exitReason switch
        {
            "Returned Successfully" => 1,
            "ACC" => 1,
            _ => 0
        };

        public static double GenerateNormalRandom(Random rand, double mean, double stdDev)
        {
            double u1 = 1.0 - rand.NextDouble();
            double u2 = 1.0 - rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return Math.Max(0, Math.Min(1, mean + stdDev * randStdNormal));
        }

        public static string GetWeightedChoice(Random rand, string[] choices, double[] weights)
        {
            double totalWeight = weights.Sum();
            double randomValue = rand.NextDouble() * totalWeight;
            double currentWeight = 0;
            for (int i = 0; i < choices.Length; i++)
            {
                currentWeight += weights[i];
                if (randomValue <= currentWeight)
                    return choices[i];
            }
            return choices[^1];
        }

        public static string GetZoneBasedOnBehavior(double aggressionRisk, double engagementLevel)
        {
            if (aggressionRisk > 0.7) return "Red";
            if (aggressionRisk > 0.4 || engagementLevel < 0.3) return "Yellow";
            if (engagementLevel > 0.7 && aggressionRisk < 0.2) return "Green";
            return "Blue";
        }

        public static string GetEmotionBasedOnBehavior(double aggressionRisk, double engagementLevel)
        {
            var emotions = new[] { "Happy", "Calm", "Anxious", "Angry", "Sad" };
            var weights = new double[]
            {
                engagementLevel * 0.8,
                (1 - engagementLevel) * 0.6,
                aggressionRisk * 0.9,
                (1 - engagementLevel) * 0.4,
                aggressionRisk * 0.5,
                engagementLevel * 0.6
            };
            return GetWeightedChoice(new Random(), emotions, weights);
        }

        public static string PredictOutcome(double avgAggression, double avgEngagement, double redZonePercent,
            double improvementScore, double riskFactor, double programEffectiveness,
            double familySupport, int behaviorDays)
        {
            double score = 0;
            score += (5 - avgEngagement) / 5 * 0.2;
            score += Math.Max(0, 1 - avgAggression) * 0.1;
            score += Math.Max(0, 1 - redZonePercent) * 0.1;
            score += Math.Max(0, improvementScore) * 0.2;
            score += programEffectiveness * 0.1;
            score += familySupport * 0.15;
            score += Math.Max(0, 1 - riskFactor) * 0.05;
            score += Math.Min(1, behaviorDays / 60.0) * 0.1;
            score += GenerateNormalRandom(new Random(), 0, 0.1);
            score = Math.Max(0, Math.Min(1, score));
            double randVal = new Random().NextDouble();

            if (score > 0.7) return "Returned Successfully";
            if (score > 0.55) return randVal < 0.8 ? "Returned Successfully" : "ACC";
            if (score > 0.4) return randVal < 0.3 ? "ACC" : randVal < 0.6 ? "Transferred" : "Referred Out";
            if (score > 0.3) return randVal < 0.2 ? "Transferred" : randVal < 0.7 ? "Referred Out" : "ABS";
            return randVal < 0.5 ? "Referred Out" : "ABS";
        }

        public static int CalculateLengthOfStay(string outcome, double improvementScore, double riskFactor)
        {
            int baseDays = outcome switch
            {
                "Returned Successfully" => 75,
                "Referred Out" => 45,
                "ABS" => 30,
                "ACC" => 90,
                _ => 60
            };
            double adjustment = improvementScore * 20 - riskFactor * 15;
            return Math.Max(14, baseDays + (int)adjustment + new Random().Next(-15, 15));
        }

        public static string CalculateExitAcademicLevel(string entryLevel, double improvementScore)
        {
            var rand = new Random();
            if (improvementScore > 0.3 && rand.NextDouble() < 0.4)
            {
                return entryLevel switch
                {
                    "Below Grade" => "At Grade",
                    "At Grade" => rand.NextDouble() < 0.3 ? "Above Grade" : "At Grade",
                    _ => entryLevel
                };
            }
            return entryLevel;
        }

        public static string CalculateExitSocialSkillsLevel(string entryLevel, double improvementScore)
        {
            var rand = new Random();
            if (improvementScore > 0.2 && rand.NextDouble() < 0.6)
            {
                return entryLevel switch
                {
                    "Low" => "Medium",
                    "Medium" => rand.NextDouble() < 0.4 ? "High" : "Medium",
                    _ => entryLevel
                };
            }
            return entryLevel;
        }
    }
}
