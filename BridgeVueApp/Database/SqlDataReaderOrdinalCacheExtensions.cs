using System;
using System.Globalization;
using Microsoft.Data.SqlClient;
using BridgeVueApp.Data;

namespace BridgeVueApp.Data
{
    public static class SqlDataReaderOrdinalCacheExtensions
    {
        public static int GetInt32OrDefault(this SqlDataReader r, OrdinalCache ords, string name, int def = 0)
            => ords.TryGet(name, out var o) && !r.IsDBNull(o) ? r.GetInt32(o) : def;

        public static float GetSingleOrDefault(this SqlDataReader r, OrdinalCache ords, string name, float def = 0f)
        {
            if (!ords.TryGet(name, out var o) || r.IsDBNull(o)) return def;
            return Convert.ToSingle(r.GetValue(o), CultureInfo.InvariantCulture);
        }

        public static bool GetBooleanOrDefault(this SqlDataReader r, OrdinalCache ords, string name, bool def = false)
            => ords.TryGet(name, out var o) && !r.IsDBNull(o) ? r.GetBoolean(o) : def;
    }
}

