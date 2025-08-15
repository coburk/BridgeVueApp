using Microsoft.Data.SqlClient;
using BridgeVueApp.Database;

public static class PredictionLogger
{
    public static long LogPrediction(int modelId, string entityKey, string predictedLabel, float? score)
    {
        using var conn = new SqlConnection(DatabaseConfig.FullConnection);
        conn.Open();

        using var cmd = new SqlCommand($@"
            INSERT INTO {DatabaseConfig.TableInferenceLog}
            (ModelID, PredictedLabel, PredictedScore, EntityKey)
            OUTPUT INSERTED.InferenceID
            VALUES (@m,@l,@s,@k);", conn);
        cmd.Parameters.AddWithValue("@m", modelId);
        cmd.Parameters.AddWithValue("@l", predictedLabel);
        cmd.Parameters.AddWithValue("@s", (object?)score ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@k", (object?)entityKey ?? DBNull.Value);
        return (long)cmd.ExecuteScalar();
    }

    // Call this later when the ground truth becomes available
    public static void BackfillActual(long inferenceId, string actualLabel)
    {
        using var conn = new SqlConnection(DatabaseConfig.FullConnection);
        conn.Open();
        using var cmd = new SqlCommand($@"UPDATE {DatabaseConfig.TableInferenceLog} SET ActualLabel=@a WHERE InferenceID=@id;", conn);
        cmd.Parameters.AddWithValue("@a", actualLabel);
        cmd.Parameters.AddWithValue("@id", inferenceId);
        cmd.ExecuteNonQuery();
    }
}

