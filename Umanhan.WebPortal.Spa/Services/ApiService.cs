using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Umanhan.Models.Models;

namespace Umanhan.WebPortal.Spa.Services
{
    public class ApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IAccessTokenProvider _tokenProvider;

        private async Task<HttpClient> CreateClientAsync(string apiName)
        {
            var client = _httpClientFactory.CreateClient(apiName);
            var tokenResult = await _tokenProvider.RequestAccessToken(new AccessTokenRequestOptions
            {
                Scopes = new[] { "" }
            });
            if (tokenResult.TryGetToken(out var accessToken))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Value);
            }
            else
            {
                //throw new UnauthorizedAccessException("No access token found.");
            }
            return client;
        }

        private async Task<ApiResponse<TResponse>> HandleResponseAsync<TResponse>(HttpResponseMessage response)
        {
            var rawContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var statusCode = response.StatusCode;

            var apiResponse = new ApiResponse<TResponse>
            {
                IsSuccess = response.IsSuccessStatusCode,
                StatusCode = (int)statusCode
            };

            if (response.IsSuccessStatusCode)
            {
                try
                {
#if DEBUG
                    Console.WriteLine($"API call succeeded with status code: {statusCode} and content: {rawContent}");
#endif
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return ApiResponse<TResponse>.Success(default!);
                    }

                    apiResponse.Data = JsonSerializer.Deserialize<TResponse>(rawContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (apiResponse.Data != null)
                        return apiResponse;
                    else
                        return ApiResponse<TResponse>.Failure(statusCode.ToString(), "Deserialization returned null or empty.");
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"HandleResponseAsync.JsonException: {JsonSerializer.Serialize(ex)}");
                    return ApiResponse<TResponse>.Failure(statusCode.ToString(), "Success status but failed to deserialize the response body.");
                }
            }
#if DEBUG
            Console.WriteLine($"API call failed with status code: {statusCode} and content: {rawContent}");
#endif
            // handle 401 and 403 errors
            if (statusCode == HttpStatusCode.Unauthorized ||
                statusCode == HttpStatusCode.Forbidden)
            {
                //await _authService.LogoutUserAsync().ConfigureAwait(false);
                return ApiResponse<TResponse>.Failure(statusCode.ToString(), "You are not authorized to perform this action.");
            }

            if (statusCode == HttpStatusCode.NotFound)
            {
                // Resource not found, return a specific message
                if (string.IsNullOrWhiteSpace(rawContent))
                    return ApiResponse<TResponse>.Failure(statusCode.ToString(), "The requested resource was not found.");
                else
                    return ApiResponse<TResponse>.Failure(statusCode.ToString(), rawContent);
            }

            var mediaType = response.Content.Headers.ContentType?.MediaType;
            if (mediaType == "application/problem+json")
            {
                try
                {
                    var problem = JsonSerializer.Deserialize<ProblemDetails>(rawContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    var title = problem?.Title ?? "An error occurred";
                    var detail = problem?.Detail ?? rawContent;
#if DEBUG
                    Console.WriteLine($"ProblemDetails parsed with title: {title} and detail: {detail}");
#endif
                    return ApiResponse<TResponse>.Failure(statusCode.ToString(), $"{title}: {detail}");
                }
                catch (JsonException)
                {
                    // Fallback if the ProblemDetails payload is malformed
                    return ApiResponse<TResponse>.Failure(statusCode.ToString(), rawContent);
                }
            }

            try
            {
                using var document = JsonDocument.Parse(rawContent);
                var root = document.RootElement;

                // If it has "errors" as a JSON object, that’s typically validation errors
                if (root.TryGetProperty("errors", out var errorsNode) &&
                    errorsNode.ValueKind == JsonValueKind.Object)
                {
#if DEBUG
                    Console.WriteLine("Validation errors found in response.");
#endif
                    var validationErrors = new Dictionary<string, List<string>>();
                    foreach (var fieldError in errorsNode.EnumerateObject())
                    {
                        var messages = fieldError.Value
                            .EnumerateArray()
                            .Select(x => x.GetString() ?? string.Empty)
                            .ToList();

                        validationErrors[fieldError.Name] = messages;
                    }

                    var detailMessage = root.TryGetProperty("detail", out var detailNode)
                        ? detailNode.GetString()
                        : null;

                    return ApiResponse<TResponse>.Failure(statusCode.ToString(), detailMessage, validationErrors);
                }

                // If it has a top‐level "detail", use that
                if (root.TryGetProperty("detail", out var simpleDetail))
                {
#if DEBUG
                    Console.WriteLine($"Simple detail found in response: {simpleDetail.GetString()}");
#endif
                    return ApiResponse<TResponse>.Failure(statusCode.ToString(), simpleDetail.GetString());
                }
            }
            catch (JsonException)
            {
                // If parsing fails, we’ll just return raw text below
            }

            return ApiResponse<TResponse>.Failure(statusCode.ToString(), rawContent);
        }

        public ApiService(IHttpClientFactory httpClientFactory, IAccessTokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
        }

        public async Task<ApiResponse<T>> GetAsync<T>(string apiName, string url)
        {
            var client = await CreateClientAsync(apiName).ConfigureAwait(false);
            HttpResponseMessage response;
            try
            {
                response = await client.GetAsync(url).ConfigureAwait(false);
            }
            catch (HttpRequestException ex)
            {
                // network error, DNS failure, server unreachable, etc.
                return ApiResponse<T>.Failure(HttpStatusCode.ServiceUnavailable.ToString(), $"Network error: {ex.Message}");
            }
            return await HandleResponseAsync<T>(response).ConfigureAwait(false);
        }

        public async Task<ApiResponse<TResponse>> PostAsync<TRequest, TResponse>(string apiName, string url, TRequest data)
        {
            var client = await CreateClientAsync(apiName).ConfigureAwait(false);
            HttpResponseMessage response;
            try
            {
                HttpContent? content = null;
                if (data != null)
                {
                    var json = JsonSerializer.Serialize(data);
                    content = new StringContent(json, Encoding.UTF8, "application/json");
                }
                response = await client.PostAsync(url, content!).ConfigureAwait(false);
            }
            catch (HttpRequestException ex)
            {
                return ApiResponse<TResponse>.Failure(HttpStatusCode.ServiceUnavailable.ToString(), $"Network error: {ex.Message}");
            }
            return await HandleResponseAsync<TResponse>(response).ConfigureAwait(false);
        }

        public async Task<ApiResponse<TResponse>> PostAsJsonAsync<TRequest, TResponse>(string apiName, string url, TRequest data)
        {
            var client = await CreateClientAsync(apiName).ConfigureAwait(false);
            HttpResponseMessage response;
            try
            {
                response = await client.PostAsJsonAsync(url, data).ConfigureAwait(false);
            }
            catch (HttpRequestException ex)
            {
                return ApiResponse<TResponse>.Failure(HttpStatusCode.ServiceUnavailable.ToString(), $"Network error: {ex.Message}");
            }
            return await HandleResponseAsync<TResponse>(response).ConfigureAwait(false);
        }

        public async Task<ApiResponse<TResponse>> PutAsync<TRequest, TResponse>(string apiName, string url, TRequest data)
        {
            var client = await CreateClientAsync(apiName).ConfigureAwait(false);
            HttpResponseMessage response;
            try
            {
                HttpContent? content = null;
                if (data != null)
                {
                    var json = JsonSerializer.Serialize(data);
                    content = new StringContent(json, Encoding.UTF8, "application/json");
                }
                response = await client.PutAsync(url, content!).ConfigureAwait(false);
            }
            catch (HttpRequestException ex)
            {
                return ApiResponse<TResponse>.Failure(HttpStatusCode.ServiceUnavailable.ToString(), $"Network error: {ex.Message}");
            }
            return await HandleResponseAsync<TResponse>(response).ConfigureAwait(false);
        }

        public async Task<ApiResponse<TResponse>> PutAsJsonAsync<TRequest, TResponse>(string apiName, string url, TRequest data)
        {
            var client = await CreateClientAsync(apiName).ConfigureAwait(false);
            HttpResponseMessage response;
            try
            {
                response = await client.PutAsJsonAsync(url, data).ConfigureAwait(false);
            }
            catch (HttpRequestException ex)
            {
                return ApiResponse<TResponse>.Failure(HttpStatusCode.ServiceUnavailable.ToString(), $"Network error: {ex.Message}");
            }
            return await HandleResponseAsync<TResponse>(response).ConfigureAwait(false);
        }

        public async Task<ApiResponse<TResponse>> PutStreamAsync<TResponse>(
            string apiName,
            string url,
            Stream contentStream,
            string mediaType = "application/octet-stream",
            Dictionary<string, string>? headers = null)
        {
            var client = await CreateClientAsync(apiName).ConfigureAwait(false);
            HttpResponseMessage response;
            try
            {
                using var content = new StreamContent(contentStream);
                content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        content.Headers.Add(header.Key, header.Value);
                    }
                }

                response = await client.PutAsync(url, content).ConfigureAwait(false);
            }
            catch (HttpRequestException ex)
            {
                return ApiResponse<TResponse>.Failure(HttpStatusCode.ServiceUnavailable.ToString(), $"Network error: {ex.Message}");
            }
            return await HandleResponseAsync<TResponse>(response).ConfigureAwait(false);
        }

        public async Task<ApiResponse<TResponse>> DeleteAsync<TResponse>(string apiName, string url)
        {
            var client = await CreateClientAsync(apiName).ConfigureAwait(false);
            HttpResponseMessage response;
            try
            {
                response = await client.DeleteAsync(url).ConfigureAwait(false);
            }
            catch (HttpRequestException ex)
            {
                return ApiResponse<TResponse>.Failure(HttpStatusCode.ServiceUnavailable.ToString(), $"Network error: {ex.Message}");
            }
            return await HandleResponseAsync<TResponse>(response).ConfigureAwait(false);
        }
    }
}
