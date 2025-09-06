using FluentValidation;
using Npgsql;
using System.Net.Http.Headers;
using System.Text.Json;
using Umanhan.Dtos;
using Umanhan.Dtos.HelperModels;
using Umanhan.Services;

namespace Umanhan.Reports.Api.Endpoints
{
    public class ReportEndpoints
    {
        private readonly ReportService _reportService;
        private readonly IValidator<ReportDto> _validator;
        private readonly ISchemaProvider _schemaProvider;
        private readonly IHttpClientFactory _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ReportEndpoints> _logger;

        private HttpClient CreateClient()
        {
            var accessToken = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(accessToken))
                throw new UnauthorizedAccessException("No access token found.");

            var client = _httpClient.CreateClient("NlpAPI");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Replace("Bearer ", ""));

            return client;
        }

        public ReportEndpoints(IHttpContextAccessor httpContextAccessor, ReportService reportService, IValidator<ReportDto> validator,
            IHttpClientFactory httpClient, ISchemaProvider schemaProvider, ILogger<ReportEndpoints> logger)
        {
            _reportService = reportService;
            _validator = validator;
            _schemaProvider = schemaProvider;
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<IResult> GenerateNlpBasedReportAsync(string userPrompt)
        {
            if (string.IsNullOrWhiteSpace(userPrompt))
                return Results.BadRequest("User Prompt cannot be empty.");

            try
            {
                var schemaString = await _schemaProvider.GetSchemaAsync().ConfigureAwait(false);
                if (string.IsNullOrEmpty(schemaString))
                    return Results.Problem("Schema description is empty.");

                var client = CreateClient();
                var response = await client.PostAsJsonAsync("api/chat/generate-sql",
                    new GenerateSqlRequest
                    {
                        UserPrompt = userPrompt,
                        SchemaDescription = schemaString
                    })
                    .ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                    return Results.BadRequest($"Failed to generate SQL: {response.ReasonPhrase}");

                string result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var dto = new GenerateSqlResponse();
                dto = JsonSerializer.Deserialize<GenerateSqlResponse>(result, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                string sql = dto.Sql;
                var result3 = await _reportService.RunSqlAsync(sql).ConfigureAwait(false);
                if (!result3.Key)
                    return Results.Ok(result3.Value.FirstOrDefault());

                // ask AI to anylyze the data
                var response2 = await client.PostAsJsonAsync("api/chat/analyze-data",
                    new AnalyzeDataRequest
                    {
                        ConvoId = dto.ConvoId,
                        Data = result3.Value
                    })
                    .ConfigureAwait(false);

                if (!response2.IsSuccessStatusCode)
                    return Results.BadRequest($"Failed to analyze data: {response2.ReasonPhrase}");

                string result2 = await response2.Content.ReadAsStringAsync().ConfigureAwait(false);
                return Results.Ok(result2);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating NLP-based report");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GeneratePnlReportAsync(Guid farmId, DateTime period)
        {
            if (farmId == Guid.Empty)
                return Results.BadRequest("Farm ID cannot be empty.");

            try
            {
                var sql = $@"
SELECT FORMAT(ActivityDate, 'yyyy-MM') AS Period, 
  SUM(CASE WHEN Type='Sale' THEN Amount ELSE 0 END) AS Revenue, 
  SUM(CASE WHEN Category IN ('Seeds','Labor','Feed') THEN Amount ELSE 0 END) AS COGS, 
  SUM(CASE WHEN Category NOT IN ('Seeds','Labor','Feed','Sale') THEN Amount ELSE 0 END) AS OpEx 
FROM Transactions 
WHERE 1=1 
  AND FarmId = '{farmId}' 
  AND MONTH(ActivityDate) = '{period:MM}' 
  AND YEAR(ActivityDate) = '{period:yyyy}'
GROUP BY FORMAT(ActivityDate, 'yyyy-MM');";
                var data = await _reportService.RunSqlAsync(sql).ConfigureAwait(false);
                return Results.Ok(data);
            }
            catch (NpgsqlException ex)
            {
                _logger.LogError(ex, "Database error generating P&L report for farm ID {FarmId} on date {Period}", farmId, period);
                return Results.Problem(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating P&L report for farm ID {FarmId} on date {Period}", farmId, period);
                return Results.Problem(ex.Message);
            }
        }
    }
}
