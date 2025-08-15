// SyntheticDataGenerator.cs
using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using BridgeVueApp.Models;
using BridgeVueApp.DataGeneration;

namespace BridgeVueApp.DataGeneration
{
    public static class SyntheticDataGenerator
    {
        // Generates a list of student profiles with random data
        public static List<StudentProfile> GenerateStudentProfiles(int count, IProgress<string> progress)
        {
            var students = new List<StudentProfile>();

            var faker = new Faker<StudentProfile>()
                .RuleFor(s => s.LocalKey, f => Guid.NewGuid())  // Assign a unique LocalKey to each profile
                .RuleFor(s => s.FirstName, (f, s) => f.Name.FirstName())
                .RuleFor(s => s.LastName, (f, s) => f.Name.LastName())
                .RuleFor(s => s.Grade, (f, s) => f.Random.Int(3, 6))
                .RuleFor(s => s.Age, (f, s) => s.Grade + 5)
                .RuleFor(s => s.Gender, (f, s) => f.PickRandom("Male", "Female"))
                .RuleFor(s => s.GenderNumeric, (f, s) => s.Gender == "Male" ? 1 : 2)
                .RuleFor(s => s.Ethnicity, (f, s) => f.PickRandom("White", "Black", "Hispanic", "Asian", "Other"))
                .RuleFor(s => s.EthnicityNumeric, (f, s) => DataGenerationUtils.GetEthnicityNumeric(s.Ethnicity))
                .RuleFor(s => s.SpecialEd, (f, s) => f.Random.Bool(0.2f))
                .RuleFor(s => s.IEP, (f, s) => f.Random.Bool(0.3f))
                .RuleFor(s => s.HasKnownOutcome, (f, s) => false)
                .RuleFor(s => s.DidSucceed, (f, s) => null as bool?)
                .RuleFor(s => s.CreatedDate, (f, s) => DateTime.UtcNow)
                .RuleFor(s => s.ModifiedDate, (f, s) => DateTime.UtcNow);
                
            for (int i = 0; i < count; i++)
            {
                if (i % 5 == 0)
                    progress?.Report($"Generating student {i + 1}/{count}...");

                students.Add(faker.Generate());
            }

            return students;
        }



        // Generates intake data for each student based on their profile
        public static List<IntakeData> GenerateIntakeData(List<StudentProfile> students, IProgress<string> progress)
        {
            var intakeList = new List<IntakeData>();
            var faker = new Faker();

            foreach (var student in students)
            {
                float familySupport = faker.Random.Float(0.0f, 1.0f);
                int priorIncidents = faker.Random.Int(0, 5);
                float stress = faker.Random.Float(0.0f, 1.0f);

                // Risk score calculation
                float riskScore = (priorIncidents / 5.0f) * 0.4f + (1 - familySupport) * 0.3f + stress * 0.3f;

                var intake = new IntakeData
                {
                    StudentLocalKey = student.LocalKey, // Link to StudentProfile
                    EntryReason = faker.PickRandom("Aggression", "Anxiety", "Trauma", "Withdrawn", "Disruptive", "Other"),
                    PriorIncidents = priorIncidents,
                    OfficeReferrals = faker.Random.Int(0, 3),
                    Suspensions = faker.Random.Int(0, 2),
                    Expulsions = faker.Random.Int(0, 1),
                    EntryAcademicLevel = faker.PickRandom("Below Grade", "At Grade", "Above Grade"),
                    EntrySocialSkillsLevel = faker.PickRandom("Low", "Medium", "High"),
                    CheckInOut = faker.Random.Bool(0.5f),
                    StructuredRecess = faker.Random.Bool(0.4f),
                    StructuredBreaks = faker.Random.Bool(0.3f),
                    SmallGroups = faker.Random.Int(0, 3),
                    SocialWorkerVisits = faker.Random.Int(0, 3),
                    PsychologistVisits = faker.Random.Int(0, 2),
                    EntryDate = DateTime.UtcNow.AddDays(-faker.Random.Int(5, 20)),
                    RiskScore = (int)(riskScore * 10),
                    StudentStressLevelNormalized = stress,
                    FamilySupportNormalized = familySupport,
                    AcademicAbilityNormalized = faker.Random.Float(0.0f, 1.0f),
                    EmotionalRegulationNormalized = 1 - stress,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow
                };

                intake.EntryReasonNumeric = DataGenerationUtils.GetEntryReasonNumeric(intake.EntryReason);
                intake.EntryAcademicLevelNumeric = DataGenerationUtils.GetAcademicLevelNumeric(intake.EntryAcademicLevel);
                intake.EntrySocialSkillsLevelNumeric = DataGenerationUtils.GetSocialSkillsLevelNumeric(intake.EntrySocialSkillsLevel);

                intakeList.Add(intake);
            }

            progress?.Report("Intake data generated.");
            return intakeList;
        }

        // Generates daily behavior records for each student based on their intake data
        public static List<DailyBehavior> GenerateDailyBehavior(List<StudentProfile> students, List<IntakeData> intakeList, IProgress<string> progress)
        {
            var behaviorList = new List<DailyBehavior>();
            var faker = new Faker();
            int behaviorId = 1;

            foreach (var student in students)
            {
                var intake = intakeList.FirstOrDefault(i => i.StudentLocalKey == student.LocalKey); // FIX: LocalKey
                if (intake == null) continue;

                int numDays = faker.Random.Int(5, 15);
                DateTime start = DateTime.UtcNow.AddDays(-numDays);

                float stress = intake.StudentStressLevelNormalized;
                float support = intake.FamilySupportNormalized;
                float aggressionBias = 0.5f * stress + 0.3f * (1 - support);

                for (int day = 0; day < numDays; day++)
                {
                    float aggressionRisk = faker.Random.Float(0.0f, 1.0f) * aggressionBias;
                    float engagementLevel = faker.Random.Float(0.0f, 1.0f) * (support + 0.5f);
                    float regulation = faker.Random.Float(0.0f, 1.0f) * (1 - stress + 0.5f);

                    string zone = DataGenerationUtils.GetZoneBasedOnBehavior(aggressionRisk, engagementLevel);

                    var record = new DailyBehavior
                    {
                        BehaviorID = behaviorId++,
                        StudentLocalKey = student.LocalKey, // keep LocalKey
                        Timestamp = start.AddDays(day).AddHours(faker.Random.Int(8, 15)),
                        Level = faker.Random.Int(1, 5),
                        Step = faker.Random.Int(1, 3),
                        VerbalAggression = faker.Random.Bool(aggressionRisk) ? 1 : 0,
                        PhysicalAggression = faker.Random.Bool(aggressionRisk * 0.8f) ? 1 : 0,
                        Elopement = faker.Random.Bool(aggressionRisk * 0.5f) ? 1 : 0,
                        OutOfSpot = faker.Random.Bool(aggressionRisk * 0.4f) ? 1 : 0,
                        WorkRefusal = faker.Random.Bool(aggressionRisk * 0.6f) ? 1 : 0,
                        ProvokingPeers = faker.Random.Bool(aggressionRisk * 0.5f) ? 1 : 0,
                        InappropriateLanguage = faker.Random.Bool(aggressionRisk * 0.7f) ? 1 : 0,
                        OutOfLane = faker.Random.Bool(aggressionRisk * 0.3f) ? 1 : 0,
                        ZoneOfRegulation = zone,
                        ZoneOfRegulationNumeric = DataGenerationUtils.GetZoneNumeric(zone),
                        AcademicEngagement = (int)(engagementLevel * 3),
                        SocialInteractions = (int)(regulation * 3),
                        EmotionalRegulation = (int)(regulation * 3),
                        AggressionRiskNormalized = aggressionRisk,
                        EngagementLevelNormalized = engagementLevel,
                        DayInProgram = day + 1,
                        WeekInProgram = (day / 5) + 1,
                        CreatedDate = DateTime.UtcNow
                    };

                    behaviorList.Add(record);
                }
            }

            progress?.Report("Daily behavior data generated.");
            return behaviorList;
        }



        // Generates weekly emotion data based on daily behaviors
        public static List<DailyBehavior> GenerateWeeklyEmotionData(List<DailyBehavior> behaviorList, IProgress<string> progress = null)
        {
            progress?.Report("📅 Generating weekly emotion check-ins...");

            var rand = new Random();
            var groupedByStudent = behaviorList.GroupBy(b => b.StudentLocalKey); // FIX

            foreach (var group in groupedByStudent)
            {
                var orderedDays = group.OrderBy(b => b.Timestamp).ToList();

                for (int i = 0; i < orderedDays.Count; i += 5)
                {
                    var window = orderedDays.Skip(i).Take(5).ToList();
                    if (window.Count == 0) continue;

                    double redZoneCount = window.Count(b => b.ZoneOfRegulation == "Red");
                    double totalAggression = window.Sum(b => b.VerbalAggression + b.PhysicalAggression);
                    double avgEngagement = window.Average(b => b.AcademicEngagement);
                    double avgEmotionScore = (redZoneCount * 2 + totalAggression - avgEngagement) / 5.0;

                    string emotion;
                    if (avgEmotionScore > 2.5) emotion = rand.NextDouble() < 0.7 ? "Angry" : "Sad";
                    else if (avgEmotionScore > 1.5) emotion = "Anxious";
                    else if (avgEmotionScore > 0.5) emotion = rand.NextDouble() < 0.7 ? "Calm" : "Happy";
                    else emotion = "Happy";

                    var record = window.First();
                    record.WeeklyEmotionDate = record.Timestamp.Date;
                    record.WeeklyEmotionPictogram = emotion;
                    record.WeeklyEmotionPictogramNumeric = DataGenerationUtils.GetEmotionNumeric(emotion);
                }
            }

            progress?.Report("✅ Weekly emotion check-ins generated.");
            return behaviorList;
        }



        private const double KnownOutcomeRate = 0.85; // ~85% labeled now
        private static readonly Random OutcomeRand = new Random(123);

        // Logistic helper
        private static double Logistic(double z) => 1.0 / (1.0 + Math.Exp(-z));

        // Compute success probability from Intake + optional Behavior aggregates
        private static double ComputeSuccessProbability(
            IntakeData i,
            (double? avgAggression, double? avgEngagement, double? avgRegulation, double? redZonePct)? beh = null)
        {
            // Normalize inputs
            double fam = i.FamilySupportNormalized;          // 0..1
            double acad = i.AcademicAbilityNormalized;       // 0..1
            double emo = i.EmotionalRegulationNormalized;   // 0..1
            double stress = i.StudentStressLevelNormalized;  // 0..1
            double risk01 = Math.Clamp(i.RiskScore / 10.0, 0.0, 1.0); // RiskScore is 0..10 -> 0..1
            double incidents = Math.Min(i.PriorIncidents, 15);

            // Base linear model from Intake
            double z = -0.4
                     + 0.8 * fam
                     + 0.8 * acad
                     + 0.6 * emo
                     - 0.7 * stress
                     - 0.8 * risk01
                     - 0.10 * incidents;

            // Optional behavior nudges if available
            if (beh.HasValue)
            {
                var (avgAgg, avgEng, avgReg, redPct) = beh.Value;
                if (avgEng.HasValue) z += 0.25 * (avgEng.Value / 3.0);          // AcademicEngagement 0..3
                if (avgReg.HasValue) z += 0.20 * (avgReg.Value / 3.0);          // EmotionalRegulation 0..3
                if (avgAgg.HasValue) z -= 0.20 * avgAgg.Value;                  // agg count per day proxy
                if (redPct.HasValue) z -= 0.30 * redPct.Value;                  // 0..1
            }

            double p = Logistic(z);
            // small noise so it's not overly deterministic
            p = Math.Clamp(p + (OutcomeRand.NextDouble() * 0.06 - 0.03), 0.02, 0.98);
            return p;
        }

        // Assign DidSucceed/HasKnownOutcome after Intake & Behavior exist
        private static void AssignOutcomes(
            List<StudentProfile> students,
            List<IntakeData> intake,
            List<DailyBehavior> behavior)
        {
            // Pre-aggregate behavior by student (LocalKey)
            var behByStudent = behavior
                .GroupBy(b => b.StudentLocalKey)
                .ToDictionary(
                    g => g.Key,
                    g =>
                    {
                        var list = g.ToList();
                        double days = Math.Max(1, list.Select(b => b.Timestamp.Date).Distinct().Count());
                        double avgAgg = list.Average(b => b.VerbalAggression + b.PhysicalAggression); // per day-ish
                        double avgEng = list.Average(b => b.AcademicEngagement); // 0..3
                        double avgReg = list.Average(b => b.EmotionalRegulation); // 0..3
                        double redPct = list.Count == 0 ? 0 : list.Count(b => b.ZoneOfRegulation == "Red") * 1.0 / list.Count;
                        return (avgAgg, avgEng, avgReg, redPct);
                    });

            foreach (var s in students)
            {
                var i = intake.FirstOrDefault(x => x.StudentLocalKey == s.LocalKey);
                if (i == null)
                {
                    // No intake? leave unlabeled
                    s.HasKnownOutcome = false;
                    s.DidSucceed = null;
                    continue;
                }

                behByStudent.TryGetValue(s.LocalKey, out var behAgg);

                // Decide if this student currently has a known outcome
                bool known = OutcomeRand.NextDouble() < KnownOutcomeRate;

                if (!known)
                {
                    s.HasKnownOutcome = false;
                    s.DidSucceed = null;
                    continue;
                }

                double p = ComputeSuccessProbability(i, behAgg);
                bool succ = OutcomeRand.NextDouble() < p;

                s.HasKnownOutcome = true;
                s.DidSucceed = succ;
            }
        }





        // Generates exit data for each student based on their profile and behavior
        public static List<ExitData> GenerateExitData(List<StudentProfile> students, IProgress<string> progress)
        {
            var exitList = new List<ExitData>();
            var faker = new Faker();

            foreach (var student in students)
            {
                if (!faker.Random.Bool(0.8f)) continue; // 80% get exit data

                // Mirror success when known, otherwise synthesize from generic logic
                bool success = student.DidSucceed ?? faker.Random.Bool(0.6f);

                int lengthOfStay = faker.Random.Int(7, 45);
                string entryAcademic = faker.PickRandom("Below Grade", "At Grade", "Above Grade");
                string exitAcademic = DataGenerationUtils.CalculateExitAcademicLevel(entryAcademic, success ? 0.7f : 0.3f);
                string entrySocial = faker.PickRandom("Low", "Medium", "High");
                string exitSocial = DataGenerationUtils.CalculateExitSocialSkillsLevel(entrySocial, success ? 0.7f : 0.3f);

                string outcomeLabel = success ? "Completed Program" : faker.PickRandom("Transferred", "Exited Early", "Other");

                var exit = new ExitData
                {
                    StudentLocalKey = student.LocalKey,
                    ExitReason = outcomeLabel,
                    ExitReasonNumeric = DataGenerationUtils.GetExitReasonNumeric(outcomeLabel),
                    ExitDate = DateTime.UtcNow.AddDays(-faker.Random.Int(1, 5)),
                    LengthOfStay = lengthOfStay,
                    ExitAcademicLevel = exitAcademic,
                    ExitAcademicLevelNumeric = DataGenerationUtils.GetAcademicLevelNumeric(exitAcademic),
                    ExitSocialSkillsLevel = exitSocial,
                    ExitSocialSkillsLevelNumeric = DataGenerationUtils.GetSocialSkillsLevelNumeric(exitSocial),
                    AcademicImprovement = faker.Random.Int(0, 2),
                    SocialSkillsImprovement = faker.Random.Int(0, 2),
                    OverallImprovementScore = success ? faker.Random.Float(0.5f, 1.0f) : faker.Random.Float(0.0f, 0.6f),
                    ProgramEffectivenessScore = success ? faker.Random.Float(0.5f, 1.0f) : faker.Random.Float(0.0f, 0.6f),
                    SuccessIndicator = success,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow
                };

                exitList.Add(exit);
            }

            progress?.Report("Exit data generated.");
            return exitList;
        }



        public static void GenerateSyntheticData(IProgress<string> progress)
        {
            progress?.Report("Starting synthetic data generation...");

            var students = GenerateStudentProfiles(50, progress);
            var intake = GenerateIntakeData(students, progress);
            var behavior = GenerateDailyBehavior(students, intake, progress);
            behavior = GenerateWeeklyEmotionData(behavior, progress); // optional but fine

            // NEW: assign outcomes using Intake (+ Behavior)
            AssignOutcomes(students, intake, behavior);

            var exit = GenerateExitData(students, progress);

            DataGenerationUtils.GeneratedProfiles = students;
            DataGenerationUtils.GeneratedIntake = intake;
            DataGenerationUtils.GeneratedBehavior = behavior;
            DataGenerationUtils.GeneratedExitData = exit;

            DataGenerationUtils.LastExitSummary = new ExitSummary
            {
                Count = exit.Count,
                AvgImprovement = exit.Average(e => e.OverallImprovementScore),
                AvgEffectiveness = exit.Average(e => e.ProgramEffectivenessScore),
                AvgAggression = exit.Average(e => e.OverallImprovementScore), // TODO replace with real aggression metric if tracked
                AvgEngagement = exit.Average(e => e.ProgramEffectivenessScore), // TODO replace
                AvgRedZonePercent = 0f
            };

            Console.WriteLine($"Generated Profiles: {DataGenerationUtils.GeneratedProfiles.Count}");
            Console.WriteLine($"Generated Intake: {DataGenerationUtils.GeneratedIntake.Count}");
            Console.WriteLine($"Generated Behavior: {DataGenerationUtils.GeneratedBehavior.Count}");
            Console.WriteLine($"Generated Exit: {DataGenerationUtils.GeneratedExitData.Count}");

            progress?.Report("Synthetic data generation completed successfully.");
        }





        public class ExitSummary
        {
            public int Count { get; set; }
            public float AvgImprovement { get; set; }
            public float AvgEffectiveness { get; set; }
            public float AvgAggression { get; set; }
            public float AvgEngagement { get; set; }
            public float AvgRedZonePercent { get; set; }
        }
    }
}