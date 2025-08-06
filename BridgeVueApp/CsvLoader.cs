using Microsoft.Data.SqlClient;
using System.Data;
using System.IO;

namespace BridgeVueApp
{
    public static class CsvLoader
    {
        public static void BulkInsert(string tableName, string csvPath)
        {
            var dt = new DataTable();
            using var reader = new StreamReader(csvPath);
            string[] headers = reader.ReadLine().Split(',');

            foreach (var header in headers)
                dt.Columns.Add(header);

            while (!reader.EndOfStream)
            {
                string[] row = reader.ReadLine().Split(',');
                dt.Rows.Add(row);
            }

            using var conn = new SqlConnection(DatabaseConfig.FullConnection);
            conn.Open();

            using var bulk = new SqlBulkCopy(conn)
            {
                DestinationTableName = tableName
            };
            bulk.WriteToServer(dt);
        }
    }
}

