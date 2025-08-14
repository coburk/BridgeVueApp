using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace BridgeVueApp.Data
{
    public sealed class OrdinalCache
    {
        private readonly Dictionary<string, int> _map = new(StringComparer.OrdinalIgnoreCase);

        public OrdinalCache(SqlDataReader reader)
        {
            for (int i = 0; i < reader.FieldCount; i++)
                _map[reader.GetName(i)] = i;
        }

        public bool TryGet(string name, out int ordinal) => _map.TryGetValue(name, out ordinal);
    }
}
