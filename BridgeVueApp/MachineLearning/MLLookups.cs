using System.Collections.Generic;

namespace BridgeVueApp.MachineLearning;

public static class MLLookups
{
    public static readonly Dictionary<int, string> Gender = new()
    {
        { 0, "UnSpecified" },
        { 1, "Male" },
        { 2, "Female" }
    };

    public static readonly Dictionary<int, string> Ethnicity = new()
    {
        { 0, "Unknown" },
        { 1, "White" },
        { 2, "Black" },
        { 3, "Hispanic" },
        { 4, "Asian" },
        { 5, "Other" }
    };

    public static readonly Dictionary<int, string> EntryReason = new()
    {
        { 0, "Unknown" },
        { 1, "Aggression" },
        { 2, "Anxiety" },
        { 3, "Trauma" },
        { 4, "Withdrawn" },
        { 5, "Disruptive" },
        { 6, "Other" }
    };

    public static readonly Dictionary<int, string> EntryAcademicLevel = new()
    {
        { 0, "Unknown" },
        { 1, "Below Grade" },
        { 2, "At Grade" },
        { 3, "Above Grade" }
    };

    public static readonly Dictionary<int, string> EntrySocialSkillsLevel = new()
    {
        { 0, "Unknown" },
        { 1, "Low" },
        { 2, "Medium" },
        { 3, "High" },
     };

    public static readonly Dictionary<int, string> ExitReason = new()
{
    { 0, "Unknown" },
    { 1, "Referred Out" },
    { 2, "ABS" },
    { 3, "Transferred" },
    { 4, "ACC" },
    { 5, "Returned Successfully" },
    { 6, "Other" }
};

    public static readonly Dictionary<int, string> ExitAcademicLevel = new()
    {
        { 0, "Unknown" },
        { 1, "Below Grade" },
        { 2, "At Grade" },
        { 3, "Above Grade" }
    };

    public static readonly Dictionary<int, string> ExitSocialSkillsLevel = new()
    {
        { 0, "Unknown" },
        { 1, "Low" },
        { 2, "Medium" },
        { 3, "High" }
    };

    // Generic helper
    public static string Lookup(Dictionary<int, string> dict, int code)
        => dict.TryGetValue(code, out var value) ? value : "Unknown";
}