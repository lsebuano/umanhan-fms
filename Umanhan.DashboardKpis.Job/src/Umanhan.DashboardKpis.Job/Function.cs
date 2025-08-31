using Amazon.Lambda.Core;
using Npgsql;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Umanhan.DashboardKpis.Job
{
    public class Function
    {
        public async Task<string> FunctionHandler(object input, ILambdaContext context)
        {
            var connStr = Environment.GetEnvironmentVariable("DB_CONNECTION");

            // These values could also come from the input object or environment variables
            string source = "LambdaTrigger";
            Guid farmId = Guid.Parse(Environment.GetEnvironmentVariable("FARM_ID")); // example: "e54a1234-abcd-4567-b890-12ef34abcd56"
            string frequency = "monthly";
            int year = DateTime.UtcNow.Year;

            try
            {
                await using var conn = new NpgsqlConnection(connStr);
                await conn.OpenAsync();

                var command = new NpgsqlCommand("CALL sp_generate_farm_kpis_for_year(@source, @farm_id, @frequency, @year)", conn);
                command.Parameters.AddWithValue("source", source);
                command.Parameters.AddWithValue("farm_id", farmId);
                command.Parameters.AddWithValue("frequency", frequency);
                command.Parameters.AddWithValue("year", year);

                await command.ExecuteNonQueryAsync();

                context.Logger.LogInformation("Stored procedure executed successfully.");
                return $"Stored procedure executed successfully for Farm ID: {farmId} for year {year}.";
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Error executing stored procedure: {ex.Message}");
                return $"Error: {ex.Message}";
            }
        }
    }

}
