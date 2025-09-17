using System;
using System.Globalization;
using Microsoft.Data.SqlClient;

public static class SqlDataReaderExtensions
{
    public static bool TryGetOrdinal(this SqlDataReader r, string name, out int ordinal)
    {
        try { ordinal = r.GetOrdinal(name); return true; }
        catch (IndexOutOfRangeException) { ordinal = -1; return false; }
    }

    // ----- non-nullable -----
    public static int GetInt32OrDefault(this SqlDataReader r, string name, int defaultValue = 0)
        => r.TryGetOrdinal(name, out var o) && !r.IsDBNull(o) ? r.GetInt32(o) : defaultValue;

    public static long GetInt64OrDefault(this SqlDataReader r, string name, long defaultValue = 0L)
        => r.TryGetOrdinal(name, out var o) && !r.IsDBNull(o) ? r.GetInt64(o) : defaultValue;

    public static float GetSingleOrDefault(this SqlDataReader r, string name, float defaultValue = 0f)
    {
        if (!r.TryGetOrdinal(name, out var o) || r.IsDBNull(o)) return defaultValue;
        var v = r.GetValue(o);
        return Convert.ToSingle(v, CultureInfo.InvariantCulture); // handles double/decimal too
    }

    public static double GetDoubleOrDefault(this SqlDataReader r, string name, double defaultValue = 0d)
    {
        if (!r.TryGetOrdinal(name, out var o) || r.IsDBNull(o)) return defaultValue;
        var v = r.GetValue(o);
        return Convert.ToDouble(v, CultureInfo.InvariantCulture);
    }

    public static decimal GetDecimalOrDefault(this SqlDataReader r, string name, decimal defaultValue = 0m)
    {
        if (!r.TryGetOrdinal(name, out var o) || r.IsDBNull(o)) return defaultValue;
        var v = r.GetValue(o);
        return Convert.ToDecimal(v, CultureInfo.InvariantCulture);
    }

    public static bool GetBooleanOrDefault(this SqlDataReader r, string name, bool defaultValue = false)
        => r.TryGetOrdinal(name, out var o) && !r.IsDBNull(o) ? r.GetBoolean(o) : defaultValue;

    // Interprets bit/0/1/“true”/“false”
    public static bool GetBooleanFlexibleOrDefault(this SqlDataReader r, string name, bool defaultValue = false)
    {
        if (!r.TryGetOrdinal(name, out var o) || r.IsDBNull(o)) return defaultValue;
        var v = r.GetValue(o);
        return v switch
        {
            bool b => b,
            byte by => by != 0,
            short s => s != 0,
            int i => i != 0,
            long l => l != 0,
            string s => bool.TryParse(s, out var parsed) ? parsed : defaultValue,
            _ => defaultValue
        };
    }

    public static string GetStringOrDefault(this SqlDataReader r, string name, string defaultValue = "")
        => r.TryGetOrdinal(name, out var o) && !r.IsDBNull(o) ? r.GetString(o) : defaultValue;

    public static DateTime GetDateTimeOrDefault(this SqlDataReader r, string name, DateTime defaultValue)
        => r.TryGetOrdinal(name, out var o) && !r.IsDBNull(o) ? r.GetDateTime(o) : defaultValue;

    public static Guid GetGuidOrDefault(this SqlDataReader r, string name, Guid defaultValue)
        => r.TryGetOrdinal(name, out var o) && !r.IsDBNull(o) ? r.GetGuid(o) : defaultValue;

    // ----- nullable -----
    public static int? GetInt32OrNull(this SqlDataReader r, string name)
        => r.TryGetOrdinal(name, out var o) && !r.IsDBNull(o) ? r.GetInt32(o) : (int?)null;

    public static long? GetInt64OrNull(this SqlDataReader r, string name)
        => r.TryGetOrdinal(name, out var o) && !r.IsDBNull(o) ? r.GetInt64(o) : (long?)null;

    public static float? GetSingleOrNull(this SqlDataReader r, string name)
    {
        if (!r.TryGetOrdinal(name, out var o) || r.IsDBNull(o)) return null;
        var v = r.GetValue(o);
        return Convert.ToSingle(v, CultureInfo.InvariantCulture);
    }

    public static double? GetDoubleOrNull(this SqlDataReader r, string name)
    {
        if (!r.TryGetOrdinal(name, out var o) || r.IsDBNull(o)) return null;
        var v = r.GetValue(o);
        return Convert.ToDouble(v, CultureInfo.InvariantCulture);
    }

    public static decimal? GetDecimalOrNull(this SqlDataReader r, string name)
    {
        if (!r.TryGetOrdinal(name, out var o) || r.IsDBNull(o)) return null;
        var v = r.GetValue(o);
        return Convert.ToDecimal(v, CultureInfo.InvariantCulture);
    }

    public static bool? GetBooleanOrNull(this SqlDataReader r, string name)
        => r.TryGetOrdinal(name, out var o) && !r.IsDBNull(o) ? r.GetBoolean(o) : (bool?)null;

    public static DateTime? GetDateTimeOrNull(this SqlDataReader r, string name)
        => r.TryGetOrdinal(name, out var o) && !r.IsDBNull(o) ? r.GetDateTime(o) : (DateTime?)null;

    public static Guid? GetGuidOrNull(this SqlDataReader r, string name)
        => r.TryGetOrdinal(name, out var o) && !r.IsDBNull(o) ? r.GetGuid(o) : (Guid?)null;
}
