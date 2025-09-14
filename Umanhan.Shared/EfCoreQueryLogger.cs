using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Npgsql;
using NpgsqlTypes;
using System.Diagnostics;
using System.Text.Json;

namespace Umanhan.Shared
{
    public class EfCoreQueryLogger : IObserver<KeyValuePair<string, object>>
    {
        private readonly string _connectionString;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EfCoreQueryLogger(string connectionString, IHttpContextAccessor httpContextAccessor)
        {
            _connectionString = connectionString;
            _httpContextAccessor = httpContextAccessor;
        }

        public void OnNext(KeyValuePair<string, object> kvp)
        {
            if (kvp.Key == RelationalEventId.CommandExecuted.Name)
            {
                dynamic command = kvp.Value;

                var sql = command.Command.CommandText;
                var durationMs = (int)command.Duration.TotalMilliseconds;

                // Collect query parameters
                var parameters = new Dictionary<string, object?>();
                foreach (var p in command.Command.Parameters)
                {
                    var value = p.Value;

                    // Mask sensitive data
                    if (p.ParameterName.ToLower().Contains("password") ||
                        p.ParameterName.ToLower().Contains("token") ||
                        p.ParameterName.ToLower().Contains("secret"))
                    {
                        value = "***MASKED***";
                    }

                    parameters[p.ParameterName] = value;
                }

                var jsonParams = parameters.Count > 0
                    ? JsonSerializer.Serialize(parameters)
                    : null;

                // Capture request context
                var httpContext = _httpContextAccessor.HttpContext;
                var apiEndpoint = httpContext?.Request?.Path.Value ?? "Unknown";
                var httpMethod = httpContext?.Request?.Method ?? "Unknown";
                var farmId = httpContext?.Request?.Headers["X-FarmId"].FirstOrDefault();
                var userId = httpContext?.User?.FindFirst("sub")?.Value;
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

                // Log asynchronously
                Task.Run(async () =>
                {
                    try
                    {
                        await using var conn = new NpgsqlConnection(_connectionString);
                        await conn.OpenAsync();

                        const string sqlInsert = @"
                            INSERT INTO ef_query_logs
                            (query, parameters, duration_ms, api_endpoint, http_method, farm_id, user_id, environment, created_at)
                            VALUES (@q, @params, @d, @api, @method, @farm, @user, @env, NOW());
                        ";

                        await using var cmd = new NpgsqlCommand(sqlInsert, conn);
                        cmd.Parameters.AddWithValue("@q", sql);

                        if (jsonParams != null)
                            cmd.Parameters.Add("@params", NpgsqlDbType.Jsonb).Value = jsonParams;
                        else
                            cmd.Parameters.AddWithValue("@params", DBNull.Value);

                        cmd.Parameters.AddWithValue("@d", durationMs);
                        cmd.Parameters.AddWithValue("@api", (object?)apiEndpoint ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@method", (object?)httpMethod ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@farm", farmId != null ? Guid.Parse(farmId) : (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@user", userId != null ? Guid.Parse(userId) : (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@env", environment);

                        await cmd.ExecuteNonQueryAsync();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"EF Query Logging Failed: {ex.Message}");
                    }
                });
            }
        }

        public void OnError(Exception error) =>
            Debug.WriteLine($"EF Query Logger Error: {error.Message}");

        public void OnCompleted() { }
    }
}