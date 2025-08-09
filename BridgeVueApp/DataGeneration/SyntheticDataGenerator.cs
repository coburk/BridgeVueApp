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
                    StudentID = student.StudentID,
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
                var intake = intakeList.FirstOrDefault(i => i.StudentID == student.StudentID);
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
                        StudentID = student.StudentID,
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
            var groupedByStudent = behaviorList.GroupBy(b => b.StudentID);

            foreach (var group in groupedByStudent)
            {
                var orderedDays = group.OrderBy(b => b.Timestamp).ToList();

                for (int i = 0; i < orderedDays.Count; i += 5)
                {
                    var window = orderedDays.Skip(i).Take(5).ToList();

                    if (window.Count == 0)
                        continue;

                    // Compute a weighted score from the last 5 days
                    double redZoneCount = window.Count(b => b.ZoneOfRegulation == "Red");
                    double totalAggression = window.Sum(b => b.VerbalAggression + b.PhysicalAggression);
                    double avgEngagement = window.Average(b => b.AcademicEngagement);
                    double avgEmotionScore = (redZoneCount * 2 + totalAggression - avgEngagement) / 5.0;

                    string emotion;
                    if (avgEmotionScore > 2.5)
                        emotion = rand.NextDouble() < 0.7 ? "Angry" : "Sad";
                    else if (avgEmotionScore > 1.5)
                        emotion = "Anxious";
                    else if (avgEmotionScore > 0.5)
                        emotion = rand.NextDouble() < 0.7 ? "Calm" : "Happy";
                    else
                        emotion = "Happy";

                    var record = window.First();
                    record.WeeklyEmotionDate = record.Timestamp.Date;
                    record.WeeklyEmotionPictogram = emotion;
                    record.WeeklyEmotionPictogramNumeric = DataGenerationUtils.GetEmotionNumeric(emotion);
                }
            }

            progress?.Report("✅ Weekly emotion check-ins generated.");
            return behaviorList;
        }
    




        // Generates exit data for each student based on their profile and behavior
        public static List<ExitData> GenerateExitData(List<StudentProfile> students, IProgress<string> progress)
        {
            var exitList = new List<ExitData>();
            var faker = new Faker();

            float totalImprovement = 0;
            float totalEffectiveness = 0;
            float totalAggression = 0;
            float totalEngagement = 0;
            float totalRedZone = 0;
            int outcomeCount = 0;

            foreach (var student in students)
            {
                if (faker.Random.Bool(0.8f)) // 80% of students get exit data
                {
                    var intake = faker.PickRandom(students.Where(s => s.StudentID == student.StudentID));
                    float improvementScore = faker.Random.Float(0.0f, 1.0f);
                    float riskFactor = faker.Random.Float(0.0f, 1.0f);
                    float programEffectiveness = faker.Random.Float(0.0f, 1.0f);
                    float familySupport = faker.Random.Float(0.0f, 1.0f);
                    int behaviorDays = faker.Random.Int(10, 40);

                    float avgAggression = faker.Random.Float(0.0f, 1.0f);
                    float avgEngagement = faker.Random.Float(0.0f, 1.0f);
                    float redZonePercent = faker.Random.Float(0.0f, 1.0f);

                    string predictedOutcome = DataGenerationUtils.PredictOutcome(
                        avgAggression,
                        avgEngagement,
                        redZonePercent,
                        improvementScore,
                        riskFactor,
                        programEffectiveness,
                        familySupport,
                        behaviorDays
                    );

                    int lengthOfStay = DataGenerationUtils.CalculateLengthOfStay(predictedOutcome, improvementScore, riskFactor);
                    string entryAcademic = faker.PickRandom("Below Grade", "At Grade", "Above Grade");
                    string exitAcademic = DataGenerationUtils.CalculateExitAcademicLevel(entryAcademic, improvementScore);
                    string entrySocial = faker.PickRandom("Low", "Medium", "High");
                    string exitSocial = DataGenerationUtils.CalculateExitSocialSkillsLevel(entrySocial, improvementScore);

                    var exit = new ExitData
                    {
                        StudentID = student.StudentID,
                        ExitReason = predictedOutcome,
                        ExitReasonNumeric = DataGenerationUtils.GetExitReasonNumeric(predictedOutcome),
                        ExitDate = DateTime.UtcNow.AddDays(-faker.Random.Int(1, 5)),
                        LengthOfStay = lengthOfStay,
                        ExitAcademicLevel = exitAcademic,
                        ExitAcademicLevelNumeric = DataGenerationUtils.GetAcademicLevelNumeric(exitAcademic),
                        ExitSocialSkillsLevel = exitSocial,
                        ExitSocialSkillsLevelNumeric = DataGenerationUtils.GetSocialSkillsLevelNumeric(exitSocial),
                        AcademicImprovement = faker.Random.Int(0, 2),
                        SocialSkillsImprovement = faker.Random.Int(0, 2),
                        OverallImprovementScore = improvementScore,
                        ProgramEffectivenessScore = programEffectiveness,
                        SuccessIndicator = DataGenerationUtils.GetSuccessIndicator(predictedOutcome) == 1,
                        CreatedDate = DateTime.UtcNow,
                        ModifiedDate = DateTime.UtcNow
                    };

                    exitList.Add(exit);

                    // Tracking for summary report
                    totalImprovement += improvementScore;
                    totalEffectiveness += programEffectiveness;
                    totalAggression += avgAggression;
                    totalEngagement += avgEngagement;
                    totalRedZone += redZonePercent;
                    outcomeCount++;
                }
            }



            progress?.Report("Exit data generated.");
            return exitList;
        }


        public static void GenerateSyntheticData(IProgress<string> progress)
        {
            progress?.Report("⏳ Starting synthetic data generation...");

            var students = GenerateStudentProfiles(50, progress);
            var intake = GenerateIntakeData(students, progress);
            var behavior = GenerateDailyBehavior(students, intake, progress);
            var exit = GenerateExitData(students, progress);

            DataGenerationUtils.GeneratedProfiles = students;
            DataGenerationUtils.GeneratedIntake = intake;
            DataGenerationUtils.GeneratedBehavior = behavior;
            DataGenerationUtils.GeneratedExitData = exit;

            // Optional: compute summary and assign
            DataGenerationUtils.LastExitSummary = new ExitSummary
            {
                Count = exit.Count,
                AvgImprovement = exit.Average(e => e.OverallImprovementScore),
                AvgEffectiveness = exit.Average(e => e.ProgramEffectivenessScore),
                AvgAggression = exit.Average(e => e.OverallImprovementScore), // Replace with real aggression metric if tracked
                AvgEngagement = exit.Average(e => e.ProgramEffectivenessScore), // Replace with real engagement metric
                AvgRedZonePercent = 0f // Placeholder
            };

            Console.WriteLine($"Generated Profiles: {DataGenerationUtils.GeneratedProfiles.Count}");
            Console.WriteLine($"Generated Intake: {DataGenerationUtils.GeneratedIntake.Count}");
            Console.WriteLine($"Generated Behavior: {DataGenerationUtils.GeneratedBehavior.Count}");
            Console.WriteLine($"Generated Exit: {DataGenerationUtils.GeneratedExitData.Count}");

            progress?.Report("✅ Synthetic data generation completed successfully.");
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