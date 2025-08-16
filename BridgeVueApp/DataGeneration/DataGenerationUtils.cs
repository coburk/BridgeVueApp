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
        // -------- canonical sets / helpers (multiclass ExitReason) --------
        public static readonly string[] CanonicalExitReasons =
        {
            "Returned Successfully",
            "ACC",
            "Transferred",
            "ABS",
            "Referred Out"
        };

        private static readonly HashSet<string> CanonicalExitReasonsSet =
            new HashSet<string>(CanonicalExitReasons, StringComparer.OrdinalIgnoreCase);

        public static bool IsCanonicalExitReason(string? s) =>
            !string.IsNullOrWhiteSpace(s) && CanonicalExitReasonsSet.Contains(s.Trim());

        public static int GetExitReasonNumeric(string reason) => reason switch
        {
            "Returned Successfully" => 5,
            "ACC"                   => 4,
            "Transferred"           => 3,
            "ABS"                   => 2,
            "Referred Out"          => 1,
            _                       => 0
        };

        public static string GetExitReasonByNumeric(int code) => code switch
        {
            5 => "Returned Successfully",
            4 => "ACC",
            3 => "Transferred",
            2 => "ABS",
            1 => "Referred Out",
            _ => "Referred Out"
        };

        // Binary success derived from ExitReason (kept as int to align with DB bit/int)
        public static int GetSuccessIndicator(string? exitReason) =>
            string.Equals(exitReason?.Trim(), "Returned Successfully", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(exitReason?.Trim(), "ACC", StringComparison.OrdinalIgnoreCase) ? 1 : 0;

        // -------- generated data holders (unchanged) --------
        public static List<StudentProfile> GeneratedProfiles { get; set; } = new();
        public static List<IntakeData> GeneratedIntake { get; set; } = new();
        public static List<DailyBehavior> GeneratedBehavior { get; set; } = new();
        public static List<ExitData> GeneratedExitData { get; set; } = new();
        public static ExitSummary LastExitSummary { get; set; } = new();

        // -------- lookups (unchanged) --------
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

        // -------- RNG utilities (remove scattered new Random() calls) --------
        // Note: Random isn't thread-safe. If you multi-thread generation, replace with ThreadLocal<Random>.
        private static readonly Random Rng = new Random();

        public static double GenerateNormalRandom(Random rand, double mean, double stdDev)
        {
            // Box–Muller
            double u1 = 1.0 - rand.NextDouble();
            double u2 = 1.0 - rand.NextDouble();
            double z0 = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return Math.Max(0, Math.Min(1, mean + stdDev * z0));
        }

        public static string GetWeightedChoice(Random rand, string[] choices, double[] weights)
        {
            if (choices == null || weights == null || choices.Length == 0)
                return string.Empty;

            // Guard: align lengths
            if (weights.Length != choices.Length)
            {
                // truncate or pad with small eps so code remains deterministic
                Array.Resize(ref weights, choices.Length);
                for (int i = 0; i < weights.Length; i++)
                    if (weights[i] < 0) weights[i] = 0;
            }

            double totalWeight = weights.Sum();
            if (totalWeight <= 0) return choices[^1];

            double u = rand.NextDouble() * totalWeight;
            double acc = 0;
            for (int i = 0; i < choices.Length; i++)
            {
                acc += weights[i];
                if (u <= acc) return choices[i];
            }
            return choices[^1];
        }

        // -------- behavior → zone / emotion --------
        public static string GetZoneBasedOnBehavior(double aggressionRisk, double engagementLevel)
        {
            if (aggressionRisk > 0.7) return "Red";
            if (aggressionRisk > 0.4 || engagementLevel < 0.3) return "Yellow";
            if (engagementLevel > 0.7 && aggressionRisk < 0.2) return "Green";
            return "Blue";
        }

        public static string GetEmotionBasedOnBehavior(double aggressionRisk, double engagementLevel)
        {
            // FIX: weights length must match emotions length (5). Also stop creating new Random() each call.
            var emotions = new[] { "Happy", "Calm", "Anxious", "Angry", "Sad" };
            var weights = new double[]
            {
                Math.Max(0, engagementLevel) * 0.8,            // Happy
                Math.Max(0, 1 - aggressionRisk) * 0.6,         // Calm
                Math.Max(0, aggressionRisk) * 0.9,             // Anxious
                Math.Max(0, 1 - engagementLevel) * 0.4,        // Angry
                Math.Max(0, aggressionRisk) * 0.5              // Sad
            };
            return GetWeightedChoice(Rng, emotions, weights);
        }

        // Outcome Generator
        public static string PredictOutcome(
            double avgAggression,
            double avgEngagement,
            double redZonePercent,
            double improvementScore,
            double riskFactor,
            double programEffectiveness,
            double familySupport,
            int behaviorDays)
        {
            // --- 1) Score: keep your features & weights; just small tidy + clamp ---
            double score = 0;
            score += (5 - avgEngagement) / 5 * 0.2;
            score += Math.Max(0, 1 - avgAggression) * 0.1;
            score += Math.Max(0, 1 - redZonePercent) * 0.1;
            score += Math.Max(0, improvementScore) * 0.2;
            score += programEffectiveness * 0.1;
            score += familySupport * 0.15;
            score += Math.Max(0, 1 - riskFactor) * 0.05;
            score += Math.Min(1, behaviorDays / 60.0) * 0.1;

            // Slightly smaller noise so ACC doesn’t get “washed out” in the mid band
            score += GenerateNormalRandom(new Random(), 0, 0.08);
            score = Math.Max(0, Math.Min(1, score));

            // --- 2) Piecewise probabilities for all 5 outcomes ---
            // Canonical order: RS, ACC, Transferred, ABS, Referred Out
            double wRS, wACC, wTrans, wABS, wRef;

            if (score >= 0.75)
            {
                // Very strong outcomes: overwhelmingly RS, but tiny mass remains elsewhere
                wRS = 0.88; wACC = 0.10; wTrans = 0.01; wRef = 0.01; wABS = 0.00;
            }
            else if (score >= 0.60)
            {
                // Strong: RS and ACC dominate
                wRS = 0.60; wACC = 0.30; wTrans = 0.05; wRef = 0.04; wABS = 0.01;
            }
            else if (score >= 0.45)
            {
                // Mixed: ACC peaks, Transferred/Referred present
                wRS = 0.25; wACC = 0.35; wTrans = 0.20; wRef = 0.15; wABS = 0.05;
            }
            else if (score >= 0.30)
            {
                // Weaker: Transferred/Referred dominate, ABS rises
                wRS = 0.05; wACC = 0.10; wTrans = 0.35; wRef = 0.35; wABS = 0.15;
            }
            else
            {
                // Very weak: ABS + Referred dominate; still non-zero others
                wRS = 0.02; wACC = 0.05; wTrans = 0.10; wRef = 0.38; wABS = 0.45;
            }

            // --- 3) Feature-driven adjustments (soft, multiplicative) ---
            // These keep your “complex script” feel while ensuring all classes have support.
            // Positive signals boost RS/ACC; risk tilts ABS/Ref; keep Trans moderate.
            double rsAdj = 1.0 + 0.40 * improvementScore + 0.20 * programEffectiveness + 0.20 * familySupport;
            double accAdj = 1.0 + 0.20 * improvementScore + 0.10 * familySupport;
            double absAdj = 1.0 + 0.40 * riskFactor - 0.20 * familySupport;
            double refAdj = 1.0 + 0.20 * riskFactor - 0.10 * programEffectiveness;
            double trAdj = 1.0 + 0.05 * (Math.Abs(score - 0.5)); // mid-band affinity, subtle

            wRS *= rsAdj;
            wACC *= accAdj;
            wABS *= absAdj;
            wRef *= refAdj;
            wTrans *= trAdj;

            // Soft floor so nothing collapses to zero (preserves natural imbalance)
            const double floor = 0.001;
            wRS = Math.Max(wRS, floor);
            wACC = Math.Max(wACC, floor);
            wTrans = Math.Max(wTrans, floor);
            wABS = Math.Max(wABS, floor);
            wRef = Math.Max(wRef, floor);

            // Normalize
            double sum = wRS + wACC + wTrans + wABS + wRef;
            var weights = new[] { wRS / sum, wACC / sum, wTrans / sum, wABS / sum, wRef / sum };
            var choices = new[] { "Returned Successfully", "ACC", "Transferred", "ABS", "Referred Out" };

            // --- 4) Sample using your existing helper ---
            return GetWeightedChoice(new Random(), choices, weights);
        }


        // -------- exit-level post-calcs --------
        public static int CalculateLengthOfStay(string outcome, double improvementScore, double riskFactor)
        {
            int baseDays = outcome switch
            {
                "Returned Successfully" => 75,
                "Referred Out" => 45,
                "ABS" => 30,
                "ACC" => 90,
                "Transferred" => 60,
                _ => 60
            };
            double adjustment = improvementScore * 20 - riskFactor * 15;
            return Math.Max(14, baseDays + (int)adjustment + Rng.Next(-15, 15));
        }

        public static string CalculateExitAcademicLevel(string entryLevel, double improvementScore)
        {
            if (improvementScore > 0.3 && Rng.NextDouble() < 0.4)
            {
                return entryLevel switch
                {
                    "Below Grade" => "At Grade",
                    "At Grade"    => Rng.NextDouble() < 0.3 ? "Above Grade" : "At Grade",
                    _             => entryLevel
                };
            }
            return entryLevel;
        }

        public static string CalculateExitSocialSkillsLevel(string entryLevel, double improvementScore)
        {
            if (improvementScore > 0.2 && Rng.NextDouble() < 0.6)
            {
                return entryLevel switch
                {
                    "Low"    => "Medium",
                    "Medium" => Rng.NextDouble() < 0.4 ? "High" : "Medium",
                    _        => entryLevel
                };
            }
            return entryLevel;
        }
    }
}
