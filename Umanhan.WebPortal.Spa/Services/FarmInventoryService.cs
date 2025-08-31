using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using System;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Models.Models;
using static System.Net.WebRequestMethods;

namespace Umanhan.WebPortal.Spa.Services
{
    public class FarmInventoryService
    {
        private readonly ApiService _apiService;
        private readonly HttpClient _httpClient;

        public FarmInventoryService(ApiService apiService, HttpClient httpClient)
        {
            _apiService = apiService;
            _httpClient = httpClient;
        }

        public Task<ApiResponse<IEnumerable<FarmInventoryDto>>> GetAllInventoriesAsync()
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<FarmInventoryDto>>("OperationsAPI", "api/farm-inventories");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<IEnumerable<FarmInventoryDto>>> GetFarmInventoriesAsync(Guid farmId)
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<FarmInventoryDto>>("OperationsAPI", $"api/farm-inventories/farm-id/{farmId}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<FarmInventoryDto>> GetFarmInventoryByIdAsync(Guid id)
        {
            try
            {
                return _apiService.GetAsync<FarmInventoryDto>("OperationsAPI", $"api/farm-inventories/{id}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<string>> GetS3PresignedUrlAsync(string filename, string contentType)
        {
            try
            {
                filename = Uri.EscapeDataString(filename);
                contentType = Uri.EscapeDataString(contentType);

                return _apiService.GetAsync<string>("OperationsAPI", $"api/farm-inventories/presigned-url/{filename}/{contentType}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<FarmInventoryDto> CreateFarmInventoryAsync(FarmInventoryDto inventory)
        {
            try
            {
                var response = await _apiService.PostAsync<FarmInventoryDto, FarmInventoryDto>("OperationsAPI", "api/farm-inventories", inventory).ConfigureAwait(false);
                if (!response.IsSuccess)
                {
                    if (response.Errors != null && response.Errors.Any())
                    {
                        var errors = new Dictionary<string, List<string>>();
                        foreach (var error in response.Errors)
                        {
                            errors.Add(error.Key, error.Value);
                        }
                        response.ErrorMessage = string.Join(", ", errors.SelectMany(x => x.Value));
                    }
                }
                return response.Data;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public async Task<FarmInventoryDto> UpdateFarmInventoryAsync(FarmInventoryDto inventory)
        {
            try
            {
                var response = await _apiService.PutAsync<FarmInventoryDto, FarmInventoryDto>("OperationsAPI", $"api/farm-inventories/{inventory.FarmInventoryId}", inventory).ConfigureAwait(false);
                if (!response.IsSuccess)
                {
                    if (response.Errors != null && response.Errors.Any())
                    {
                        var errors = new Dictionary<string, List<string>>();
                        foreach (var error in response.Errors)
                        {
                            errors.Add(error.Key, error.Value);
                        }
                        response.ErrorMessage = string.Join(", ", errors.SelectMany(x => x.Value));
                    }
                }
                return response.Data;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public async Task<FarmInventoryDto> DeleteFarmInventoryAsync(Guid id)
        {
            try
            {
                var response = await _apiService.DeleteAsync<FarmInventoryDto>("OperationsAPI", $"api/farm-inventories/{id}").ConfigureAwait(false);
                if (!response.IsSuccess)
                {
                    if (response.Errors != null && response.Errors.Any())
                    {
                        var errors = new Dictionary<string, List<string>>();
                        foreach (var error in response.Errors)
                        {
                            errors.Add(error.Key, error.Value);
                        }
                        response.ErrorMessage = string.Join(", ", errors.SelectMany(x => x.Value));
                    }
                }
                return response.Data;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public async Task<Dictionary<string, string>> GetPresignedUrlAsync(string fn, string ct)
        {
            try
            {
                string extension = Path.GetExtension(fn);
                string origFileName = Path.GetFileName(fn);
                string filename = Uri.EscapeDataString($"{Guid.NewGuid()}{extension}");
                string contentType = Uri.EscapeDataString(ct);

                var response = await _apiService.GetAsync<Dictionary<string, string>>("OperationsAPI", $"api/farm-inventories/presigned-url/{filename}/{contentType}").ConfigureAwait(false);
                if (response.IsSuccess)
                {
                    return response.Data;
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public async Task<ApiResponse<bool>> UploadFileToS3Async(IBrowserFile file, Guid id, string key, string presignedUrl)
        {
            try
            {
                decimal maxFileSizeInMB = 2m; // 2 MB limit
                long maxAllowedSizeInBytes = (long)(maxFileSizeInMB * 1024 * 1024);

                string extension = Path.GetExtension(file.Name);
                string origFileName = Path.GetFileName(file.Name);
                string contentType = file.ContentType;
                //string filename = Uri.EscapeDataString($"{key}{extension}");

                if (file.Size > 2 * 1024 * 1024) // 2 MB limit
                {
                    throw new InvalidOperationException("File size exceeds the maximum allowed size of 2 MB.");
                }

                var content = file.OpenReadStream(maxAllowedSizeInBytes);
                //return await _apiService.PutStreamAsync<bool>("OperationsAPI", $"api/farm-inventories/upload-file/{filename}", content, file.ContentType, headers);
                //var response = await _apiService.GetAsync<string>("OperationsAPI", $"api/farm-inventories/presigned-url/{filename}").ConfigureAwait(false);
                //if (response.IsSuccess)
                //{
                //var presignedUrl = response.Data;
                using var contentStream = new StreamContent(content);
                contentStream.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);

                var uploadResponse = await _httpClient.PutAsync(presignedUrl, contentStream).ConfigureAwait(false);
                uploadResponse.EnsureSuccessStatusCode();
                if (uploadResponse.IsSuccessStatusCode)
                {
                    var updateResponse = await _apiService.PutAsync<string, FarmInventoryDto>("OperationsAPI", $"api/farm-inventories/{id}/{key}/{Uri.EscapeDataString(contentType)}", key).ConfigureAwait(false);
                    if (updateResponse.IsSuccess)
                    {
                        return ApiResponse<bool>.Success(true);
                    }
                    else
                    {
                        return ApiResponse<bool>.Failure("File Update Error", "Failed to update file information after upload.", updateResponse.Errors);
                    }
                }
                else
                {
                    return ApiResponse<bool>.Failure("File Upload Failed", "Failed to upload file to S3.");
                }
                //}
                //else
                //{
                //    return ApiResponse<bool>.Failure("Presigned URL Error", "Failed to get presigned URL for file upload.");
                //}
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Failure("File Upload Error", ex.Message, new Dictionary<string, List<string>> { { "FileUploadError", new List<string> { ex.Message } } });
            }
        }
    }
}
