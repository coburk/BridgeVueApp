using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static BridgeVueApp.DataGeneration.SyntheticDataGenerator;

namespace BridgeVueApp.DataGeneration
{
    public class DataGenerationManager
    {
        private readonly IProgress<string> _progress;

        public DataGenerationManager(IProgress<string> progress)
        {
            _progress = progress;
        }

        // Generate synthetic data without saving to the database
        public async Task GenerateSyntheticOnlyAsync(bool includeWeeklyEmotions = false)

        {
            _progress?.Report("Starting synthetic data generation...");

            // Simulate some delay for UI responsiveness
            await Task.Delay(100);

            var students = SyntheticDataGenerator.GenerateStudentProfiles(50, _progress);
            var intakeList = SyntheticDataGenerator.GenerateIntakeData(students, _progress);
            var behaviorList = SyntheticDataGenerator.GenerateDailyBehavior(students, intakeList, _progress);
            var exitList = SyntheticDataGenerator.GenerateExitData(students, _progress);

            // Optionally include weekly emotions
            if (includeWeeklyEmotions)
            {
                var weeklyEmotions = SyntheticDataGenerator.GenerateWeeklyEmotionData(behaviorList);
                foreach (var entry in behaviorList)
                {
                    var weekly = weeklyEmotions
                        .FirstOrDefault(w => w.StudentID == entry.StudentID && w.WeekInProgram == entry.WeekInProgram);
                    if (weekly != null)
                    {
                        entry.WeeklyEmotionPictogram = weekly.WeeklyEmotionPictogram;
                        entry.WeeklyEmotionPictogramNumeric = weekly.WeeklyEmotionPictogramNumeric;
                        entry.WeeklyEmotionDate = weekly.WeeklyEmotionDate;
                    }
                }
            }


            // Save for later database insertion
            DataGenerationUtils.GeneratedProfiles = students;
            DataGenerationUtils.GeneratedIntake = intakeList;
            DataGenerationUtils.GeneratedBehavior = behaviorList;
            DataGenerationUtils.GeneratedExitData = exitList;

            DataGenerationUtils.LastExitSummary = new ExitSummary
            {
                Count = exitList.Count,
                AvgImprovement = exitList.Average(x => x.OverallImprovementScore),
                AvgEffectiveness = exitList.Average(x => x.ProgramEffectivenessScore),
                AvgAggression = behaviorList.Any() ? behaviorList.Average(x => x.AggressionRiskNormalized) : 0,
                AvgEngagement = behaviorList.Any() ? behaviorList.Average(x => x.EngagementLevelNormalized) : 0,
                AvgRedZonePercent = behaviorList.Count == 0 ? 0 :
                    behaviorList.Count(b => b.ZoneOfRegulation == "Red") / (float)behaviorList.Count
            };

            
            _progress?.Report("✅ Synthetic data generation complete.");
        }





        public async Task GenerateAllAsync()
        {
            // Update progress
            _progress?.Report("Starting synthetic data generation...");

            // Generate student profiles
            var students = SyntheticDataGenerator.GenerateStudentProfiles(50, _progress);
            _progress?.Report("✔️ Student profiles generated.");

            // Generate intake data
            var intakeList = SyntheticDataGenerator.GenerateIntakeData(students, _progress);
            _progress?.Report("✔️ Intake data generated.");

            // Generate daily behavior
            var behaviorList = SyntheticDataGenerator.GenerateDailyBehavior(students, intakeList, _progress);
            _progress?.Report("✔️ Daily behavior generated.");

            // ✅ Generate exit data — This gives us exitList!
            var exitList = SyntheticDataGenerator.GenerateExitData(students, _progress);
            _progress?.Report("✔️ Exit data generated.");

            // ✅ Now we can compute and assign the ExitSummary
            DataGenerationUtils.LastExitSummary = new ExitSummary
            {
                Count = exitList.Count,
                AvgImprovement = exitList.Average(e => e.OverallImprovementScore),
                AvgEffectiveness = exitList.Average(e => e.ProgramEffectivenessScore),
                AvgAggression = behaviorList.Average(b => b.AggressionRiskNormalized),
                AvgEngagement = behaviorList.Average(b => b.EngagementLevelNormalized),
                AvgRedZonePercent = behaviorList.Count == 0 ? 0 : behaviorList.Count(b => b.ZoneOfRegulation == "Red") / (float)behaviorList.Count
            };
        }


    }
}


