// BridgeVueApp/DataGeneration/DataGenerationManager.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BridgeVueApp.Database;
using BridgeVueApp.Models;


namespace BridgeVueApp.DataGeneration
{
    public static class DataGenerationManager
    {
        
        // Saves generated data in the correct order:
        // 1) Inserts StudentProfile rows and captures DB StudentID
        // 2) Builds LocalKey -> StudentID map
        // 3) Remaps Intake/Daily/Exit StudentID from that map
        // 4) Inserts child tables (with FK pre-checks in DatabaseLoader)
        
        public static void SaveGeneratedData(
            List<StudentProfile> profiles,
            List<IntakeData> intakeList,
            List<DailyBehavior> behaviors,
            List<ExitData> exitList,
            IProgress<string> progress = null)
        {
            if (profiles == null) throw new ArgumentNullException(nameof(profiles));
            if (intakeList == null) throw new ArgumentNullException(nameof(intakeList));
            if (behaviors == null) throw new ArgumentNullException(nameof(behaviors));
            if (exitList == null) throw new ArgumentNullException(nameof(exitList));

            // Ensure every profile has a LocalKey (generator should do this, but just in case)
            foreach (var p in profiles)
                if (p.LocalKey == Guid.Empty) p.LocalKey = Guid.NewGuid();

            progress?.Report("Saving: inserting StudentProfile rows...");
            // --- Insert profiles and capture DB identities (mutates StudentProfile.StudentID)
            var insertedProfiles = DatabaseLoader.BulkInsertStudentProfiles(profiles);

            // Build LocalKey -> real StudentID map
            var idMap = insertedProfiles.ToDictionary(p => p.LocalKey, p => p.StudentID);

            progress?.Report("Saving: remapping child rows to real StudentIDs...");
            // --- Remap child data to real StudentIDs
            RemapIntake(intakeList, idMap);
            RemapBehavior(behaviors, idMap);
            RemapExit(exitList, idMap);

            // --- Final safety check: no 0 or negative IDs
            if (intakeList.Any(i => i.StudentID <= 0))
                throw new InvalidOperationException("IntakeData contains non-remapped StudentID (<= 0). Check StudentLocalKey mapping.");
            if (behaviors.Any(b => b.StudentID <= 0))
                throw new InvalidOperationException("DailyBehavior contains non-remapped StudentID (<= 0). Check StudentLocalKey mapping.");
            if (exitList.Any(x => x.StudentID <= 0))
                throw new InvalidOperationException("ExitData contains non-remapped StudentID (<= 0). Check StudentLocalKey mapping.");

            progress?.Report("Saving: inserting IntakeData...");
            DatabaseLoader.BulkInsertIntakeData(intakeList);

            progress?.Report("Saving: inserting DailyBehavior...");
            DatabaseLoader.BulkInsertDailyBehavior(behaviors);

            progress?.Report("Saving: inserting ExitData...");
            DatabaseLoader.BulkInsertExitData(exitList);

            progress?.Report("All generated data saved successfully.");
        }

        // Remap IntakeData to use real StudentID from the map
        private static void RemapIntake(List<IntakeData> intakeList, Dictionary<Guid, int> idMap)
        {
            for (int idx = 0; idx < intakeList.Count; idx++)
            {
                var row = intakeList[idx];
                if (!idMap.TryGetValue(row.StudentLocalKey, out var sid))
                    throw new InvalidOperationException(
                        $"IntakeData[{idx}] has unknown StudentLocalKey {row.StudentLocalKey}. " +
                        "Make sure you set StudentLocalKey = parentProfile.LocalKey during generation.");
                row.StudentID = sid;
            }
        }

        // Remap DailyBehavior to use real StudentID from the map
        private static void RemapBehavior(List<DailyBehavior> behaviors, Dictionary<Guid, int> idMap)
        {
            for (int idx = 0; idx < behaviors.Count; idx++)
            {
                var row = behaviors[idx];
                if (!idMap.TryGetValue(row.StudentLocalKey, out var sid))
                    throw new InvalidOperationException(
                        $"DailyBehavior[{idx}] has unknown StudentLocalKey {row.StudentLocalKey}. " +
                        "Make sure you set StudentLocalKey = parentProfile.LocalKey during generation.");
                row.StudentID = sid;
            }
        }

        // Remap ExitData to use real StudentID from the map
        private static void RemapExit(List<ExitData> exitList, Dictionary<Guid, int> idMap)
        {
            for (int idx = 0; idx < exitList.Count; idx++)
            {
                var row = exitList[idx];
                if (!idMap.TryGetValue(row.StudentLocalKey, out var sid))
                    throw new InvalidOperationException(
                        $"ExitData[{idx}] has unknown StudentLocalKey {row.StudentLocalKey}. " +
                        "Make sure you set StudentLocalKey = parentProfile.LocalKey during generation.");
                row.StudentID = sid;
            }
        }

        // Generate synthetic data for testing purposes
        public static async Task GenerateSyntheticOnlyAsync(int numStudents, IProgress<string> progress = null)
        {
            // 1) Generate using YOUR existing generator classes
            var profiles = SyntheticDataGenerator.GenerateStudentProfiles(numStudents, progress);
            var intake = SyntheticDataGenerator.GenerateIntakeData(profiles, progress);
            var behaviors = SyntheticDataGenerator.GenerateDailyBehavior(profiles, intake, progress);
            var exits = SyntheticDataGenerator.GenerateExitData(profiles, progress);

            // 2) Save (run on background thread so UI stays responsive)
            await Task.Run(() =>
                SaveGeneratedData(profiles, intake, behaviors, exits, progress)  // matches signature
            );
        }
    }
}
