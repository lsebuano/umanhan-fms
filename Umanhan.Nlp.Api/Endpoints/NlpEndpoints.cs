using Microsoft.AspNetCore.OutputCaching;
using OpenAI;
using OpenAI.Chat;
using System.Text;
using System.Text.Json;
using Umanhan.Dtos.HelperModels;

namespace Umanhan.Nlp.Api.Endpoints
{
    public class NlpEndpoints
    {
        private const string CHAT_COMPLETION_MODEL = "gpt-4";

        private readonly ILogger<NlpEndpoints> _logger;

        public NlpEndpoints(ILogger<NlpEndpoints> logger)
        {
            _logger = logger;
        }

        public async Task<IResult> AnalyzeWeatherDataAsync(OpenAIClient client, IOutputCacheStore cache, UserChatRequest userRequest, CancellationToken cancellationToken)
        {
            string cacheKey = $"weather-analysis-{DateTime.Now.ToString("yyyyMMdd")}";

            // Check cache
            var cachedData = await cache.GetAsync(cacheKey, cancellationToken);
            if (cachedData != null)
            {
                var cachedJson = System.Text.Encoding.UTF8.GetString(cachedData);
                var cachedResponse = JsonSerializer.Deserialize<string>(cachedJson);
                return Results.Json(cachedResponse);
            }

            try
            {
                var messages = new List<ChatMessage>
                {
                    new SystemChatMessage($"Analyze this week's Philippine weather data and provide direct, actionable insights for managing a farm that grows {userRequest.Crops}. Respond in a structured, easy-to-read format using bullet points. Prioritize recommendations for irrigation, pest control, wind protection, work scheduling, crop growth, harvesting, and planting. Ensure the response is concise yet informative, speaking directly as a farm advisor. Start the response with 'The weather is'.")
                };

                // Add the user input as a User message
                if (!string.IsNullOrEmpty(userRequest.Input))
                {
                    messages.Add(new UserChatMessage(userRequest.Input));
                }

                var chatClient = client.GetChatClient("gpt-4");
                // Get response from OpenAI using the chat completion API
                var options = new ChatCompletionOptions { Temperature = 0.3f };
                var response = await chatClient.CompleteChatAsync(messages, options).ConfigureAwait(false);

                // Process the response from the AI and return it
                var aiResponse = response.Value.Content[0].Text;
                messages.Add(new AssistantChatMessage(aiResponse));  // Optionally add assistant response to history

                // Serialize and store in cache
                var responseJson = JsonSerializer.SerializeToUtf8Bytes(aiResponse);
                await cache.SetAsync(cacheKey, responseJson, null, TimeSpan.FromMinutes(10), cancellationToken);

                return Results.Ok(aiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing weather data for crops: {Crops}", userRequest.Crops);
                return Results.Problem("No weather insights for now.");
            }
        }

        public async Task<IResult> GeneratePostgreSqlQueryAsync(OpenAIClient client, IOutputCacheStore cache, GenerateSqlRequest request, CancellationToken cancellationToken)
        {
            string convoId = Math.Abs(request.UserPrompt.GetHashCode()).ToString();
            string cacheKey = $"sql-query-{convoId}";
            string convoCacheKey = $"sql-query-convo-{convoId}";

            // Check cache
            var cachedData = await cache.GetAsync(cacheKey, cancellationToken);
            if (cachedData != null)
            {
                var cachedJson = System.Text.Encoding.UTF8.GetString(cachedData);
                var cachedResponse = JsonSerializer.Deserialize<string>(cachedJson);
                return Results.Json(cachedResponse);
            }

            try
            {
                var prompt = new StringBuilder();
                prompt.AppendLine("### Schema");
                prompt.AppendLine(request.SchemaDescription);
                prompt.AppendLine();
                prompt.AppendLine("### Currency");
                prompt.AppendLine("Philippine Peso (PHP)");
                prompt.AppendLine();
                prompt.AppendLine("### User Request");
                prompt.AppendLine(request.UserPrompt);
                prompt.AppendLine();
                prompt.AppendLine("### Task");
                prompt.AppendLine("Generate a single valid PostgreSQL query that answers the user's request. Return only the SQL, no explanation.");

                var messages = new List<ChatMessage>
                {
                    new SystemChatMessage($"You are a postgresql expert."),
                    new UserChatMessage(prompt.ToString())
                };

                var chatClient = client.GetChatClient(CHAT_COMPLETION_MODEL);
                // Get response from OpenAI using the chat completion API
                var options = new ChatCompletionOptions { Temperature = 0.3f };
                var response = await chatClient.CompleteChatAsync(messages, options).ConfigureAwait(false);

                // Process the response from the AI and return it
                var aiResponse = response.Value.Content[0].Text;
                //messages.Add(new AssistantChatMessage(aiResponse));

                prompt.AppendLine();
                prompt.AppendLine("### AI Response");
                prompt.AppendLine(aiResponse);

                var convoJson = JsonSerializer.SerializeToUtf8Bytes(prompt.ToString());
                await cache.SetAsync(convoCacheKey, convoJson, null, TimeSpan.FromMinutes(10), cancellationToken);

                //// Serialize and store in cache
                //var responseJson = JsonSerializer.SerializeToUtf8Bytes(aiResponse);
                //await cache.SetAsync(cacheKey, responseJson, null, TimeSpan.FromMinutes(10), cancellationToken);

                return Results.Ok(new GenerateSqlResponse
                {
                    ConvoId = convoId,
                    Sql = aiResponse
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating SQL query for request: {UserPrompt}", request.UserPrompt);
                return Results.Problem("No SQL query generated.");
            }
        }

        public async Task<IResult> AnalyzeDataAsync(OpenAIClient client, IOutputCacheStore cache, AnalyzeDataRequest request, CancellationToken cancellationToken)
        {
            try
            {
                string insight = "Nothing to analyze.";
                string convoCacheKey = $"sql-query-convo-{request.ConvoId}";
                // Check cache
                var cachedData = await cache.GetAsync(convoCacheKey, cancellationToken);
                if (cachedData != null)
                {
                    var cachedJson = System.Text.Encoding.UTF8.GetString(cachedData);
                    var cachedMessages = JsonSerializer.Deserialize<string>(cachedJson);

                    var prompt = new StringBuilder(cachedMessages);
                    prompt.AppendLine();
                    prompt.AppendLine("### DB Results");
                    prompt.AppendLine($"{request.Data}");
                    prompt.AppendLine();
                    prompt.AppendLine($"### Task");
                    prompt.AppendLine("Analyze these results in light of the user's original request. Recommend some call-to-actions that would boost revenue especially when the prompt is about profit and loss.");

                    var messages = new List<ChatMessage>
                    {
                        new SystemChatMessage($"You are an expert data analyst."),
                        new UserChatMessage(prompt.ToString())
                    };
                    var chatClient = client.GetChatClient(CHAT_COMPLETION_MODEL);
                    var options = new ChatCompletionOptions { Temperature = 0.3f };
                    var response = await chatClient.CompleteChatAsync(messages, options).ConfigureAwait(false);

                    var aiResponse = response.Value.Content[0].Text;
                    prompt.AppendLine();
                    prompt.AppendLine("### AI Response");
                    prompt.AppendLine(aiResponse);

                    insight = aiResponse;

                    // update the cache
                    var convoJson = JsonSerializer.SerializeToUtf8Bytes(prompt.ToString());
                    await cache.SetAsync(convoCacheKey, convoJson, null, TimeSpan.FromMinutes(10), cancellationToken);
                }
                return Results.Ok(insight);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing data for conversation ID: {ConvoId}", request.ConvoId);
                return Results.Problem("Unable to analyze data.");
            }
        }

        public async Task<IResult> AnalyzeDataAsync(OpenAIClient client, AnalyzeDataRequest request)
        {
            try
            {
                string insight = "Nothing to analyze.";
                string convoCacheKey = $"data-convo-{request.ConvoId}";

                var prompt = new StringBuilder();
                prompt.AppendLine("### Data");
                prompt.AppendLine($"{request.Data}");
                prompt.AppendLine("### Currency");
                prompt.AppendLine("Philippine Peso (Php)");
                prompt.AppendLine($"### Task");
                prompt.AppendLine($"{request.Prompt}");

                var messages = new List<ChatMessage>
                    {
                        new SystemChatMessage($"You are an expert data analyst."),
                        new UserChatMessage(prompt.ToString())
                    };
                var chatClient = client.GetChatClient(CHAT_COMPLETION_MODEL);
                var options = new ChatCompletionOptions { Temperature = 0.3f };
                var response = await chatClient.CompleteChatAsync(messages, options).ConfigureAwait(false);

                var aiResponse = response.Value.Content[0].Text;
                prompt.AppendLine();
                prompt.AppendLine("### AI Response");
                prompt.AppendLine(aiResponse);

                insight = aiResponse;

                return Results.Ok(insight);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing data for conversation ID: {ConvoId}", request.ConvoId);
                return Results.Problem("Unable to analyze data.");
            }
        }

        public async Task<IResult> GenerateWeatherHeadlineAsync(OpenAIClient client, IOutputCacheStore cache, ForecastDailyWeather request, CancellationToken cancellationToken)
        {
            try
            {
                if (request == null)
                    return Results.BadRequest("Invalid weather data provided.");

                string cacheKey = $"weather-headline-{request.Date:yyyyMMdd}-{request.City?.ToLower()}";
                var cachedData = await cache.GetAsync(cacheKey, cancellationToken);
                if (cachedData != null)
                {
                    // Check cache
                    var cachedJson = System.Text.Encoding.UTF8.GetString(cachedData);
                    var cachedResponse = JsonSerializer.Deserialize<string>(cachedJson);
                    return Results.Ok(cachedResponse);
                }
                else
                {
                    var prompt = new StringBuilder();
                    prompt.AppendLine();
                    prompt.Append("You are a seasoned meteorologist specializing in Philippine weather. ");
                    prompt.Append("Your task is to output exactly one concise, headline summarizing the current weather for the city, and what would be the effect to the farm. ");
                    prompt.Append("Do NOT include any other text.");

                    prompt.AppendLine("These are the parameters:");
                    prompt.AppendLine($"Date: {request.Date}");
                    prompt.AppendLine($"City: {request.City}");
                    prompt.AppendLine($"Temperature: {request.TempAverage}");
                    prompt.AppendLine($"Temp Max: {request.TempMin}");
                    prompt.AppendLine($"Temp Min: {request.TempMax}");
                    prompt.AppendLine($"Humidity: {request.Humidity}");
                    prompt.AppendLine($"Wind Speed: {request.WindSpeed}");
                    prompt.AppendLine($"Wind Direction: {request.WindDirection}");
                    prompt.AppendLine($"Wind Gust: {request.WindGust}");

                    var messages = new List<ChatMessage>
                    {
                        new SystemChatMessage($"You are a seasoned meteorologist specializing in Philippine weather. "),
                        new UserChatMessage(prompt.ToString())
                    };
                    var chatClient = client.GetChatClient(CHAT_COMPLETION_MODEL);
                    var options = new ChatCompletionOptions { Temperature = 0.3f };
                    var response = await chatClient.CompleteChatAsync(messages, options).ConfigureAwait(false);

                    var aiResponse = response.Value.Content[0].Text;

                    // update the cache
                    var aiResponseJson = JsonSerializer.SerializeToUtf8Bytes(aiResponse);
                    await cache.SetAsync(cacheKey, aiResponseJson, null, TimeSpan.FromMinutes(3), cancellationToken);

                    return Results.Ok(aiResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating weather headline for date: {Date}", request.Date);
                return Results.Problem("Unable to analyze data.");
            }
        }
    }
}