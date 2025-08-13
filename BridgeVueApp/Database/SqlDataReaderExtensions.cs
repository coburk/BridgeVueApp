using System;
using Microsoft.Data.SqlClient;

public static class SqlDataReaderExtensions
{
    public static bool TryGetOrdinal(this SqlDataReader r, string name, out int ordinal)
    {
        try
        {
            ordinal = r.GetOrdinal(name);
            return true;
        }
        catch (IndexOutOfRangeException)
        {
            ordinal = -1;
            return false;
        }
    }

    public static int GetInt32OrDefault(this SqlDataReader r, string name, int defaultValue = 0)
    {
        return r.TryGetOrdinal(name, out var ord) && !r.IsDBNull(ord) ? r.GetInt32(ord) : defaultValue;
    }

    public static float GetSingleOrDefault(this SqlDataReader r, string name, float defaultValue = 0f)
    {
        if (!r.TryGetOrdinal(name, out var ord) || r.IsDBNull(ord)) return defaultValue;
        var v = r.GetValue(ord);
        return Convert.ToSingle(v);
    }

    public static bool GetBooleanOrDefault(this SqlDataReader r, string name, bool defaultValue = false)
    {
        return r.TryGetOrdinal(name, out var ord) && !r.IsDBNull(ord) ? r.GetBoolean(ord) : defaultValue;
    }

    public static string GetStringOrDefault(this SqlDataReader r, string name, string defaultValue = "")
    {
        return r.TryGetOrdinal(name, out var ord) && !r.IsDBNull(ord) ? r.GetString(ord) : defaultValue;
    }
}

