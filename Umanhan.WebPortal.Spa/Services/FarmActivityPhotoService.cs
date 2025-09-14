using Microsoft.AspNetCore.Components.Forms;
using Umanhan.Dtos;
using Umanhan.Dtos.HelperModels;

namespace Umanhan.WebPortal.Spa.Services
{
    public class FarmActivityPhotoService
    {
        private readonly ApiService _apiService;
        private readonly HttpClient _httpClient;

        public FarmActivityPhotoService(ApiService apiService, HttpClient httpClient)
        {
            _apiService = apiService;
            _httpClient = httpClient;
        }

        public Task<ApiResponse<FarmActivityPhotoDto>> GetFarmActivityPhotoByIdAsync(Guid id)
        {
            try
            {
                return _apiService.GetAsync<FarmActivityPhotoDto>("OperationsAPI", $"api/farm-activity-photos/{id}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<IEnumerable<FarmActivityPhotoDto>>> GetFarmActivityPhotoByActivityAsync(Guid activityId)
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<FarmActivityPhotoDto>>("OperationsAPI", $"api/farm-activity-photos/activity/{activityId}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<FarmActivityPhotoDto> CreateFarmActivityPhotoAsync(FarmActivityPhotoDto farmPhoto)
        {
            try
            {
                var response = await _apiService.PostAsync<FarmActivityPhotoDto, FarmActivityPhotoDto>("OperationsAPI", $"api/farm-activity-photos", farmPhoto).ConfigureAwait(false);
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

        public async Task<FarmActivityPhotoDto> UpdateFarmActivityPhotoAsync(FarmActivityPhotoDto farmPhoto)
        {
            try
            {
                var response = await _apiService.PutAsync<FarmActivityPhotoDto, FarmActivityPhotoDto>("OperationsAPI", $"api/farm-activity-photos/{farmPhoto.PhotoId}", farmPhoto).ConfigureAwait(false);
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

        public async Task<FarmActivityPhotoDto> DeleteFarmActivityPhotoAsync(Guid id)
        {
            try
            {
                var response = await _apiService.DeleteAsync<FarmActivityPhotoDto>("OperationsAPI", $"api/farm-activity-photos/{id}").ConfigureAwait(false);
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

        public async Task<ApiResponse<FarmActivityPhotoDto>> UploadFileToS3Async(IBrowserFile file, Guid id, string key, string presignedUrl, string notes)
        {
            try
            {
                decimal maxFileSizeInMB = 2m; // 2 MB limit
                long maxAllowedSizeInBytes = (long)(maxFileSizeInMB * 1024 * 1024);

                string extension = Path.GetExtension(file.Name);
                string origFileName = Path.GetFileName(file.Name);
                string contentType = file.ContentType;
                //string filename = Uri.EscapeDataString($"{key}{extension}");

                if (file.Size > maxAllowedSizeInBytes) // 2 MB limit
                {
                    throw new InvalidOperationException("File size exceeds the maximum allowed size of 2 MB.");
                }

                var content = file.OpenReadStream(maxAllowedSizeInBytes);
                using var contentStream = new StreamContent(content);
                contentStream.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
                _httpClient.DefaultRequestHeaders.Authorization = null;

                var uploadResponse = await _httpClient.PutAsync(presignedUrl, contentStream).ConfigureAwait(false);
                uploadResponse.EnsureSuccessStatusCode();
                if (uploadResponse.IsSuccessStatusCode)
                {
                    var obj = new S3PhotoUploadDto
                    {
                        ActivityId = id,
                        S3ObjectKey = key,
                        S3ObjectContentType = contentType,
                        //OriginalFileName = origFileName,
                        Notes = notes
                    };
                    var createResponse = await _apiService.PostAsync<S3PhotoUploadDto, FarmActivityPhotoDto>("OperationsAPI", $"api/farm-activity-photos/upload", obj).ConfigureAwait(false);
                    if (createResponse.IsSuccess)
                    {
                        return createResponse;
                    }
                    else
                    {
                        return ApiResponse<FarmActivityPhotoDto>.Failure("File Update Error", "Failed to update file information after upload.", createResponse.Errors);
                    }
                }
                else
                {
                    return ApiResponse<FarmActivityPhotoDto>.Failure("File Upload Failed", "Failed to upload file to S3.");
                }
            }
            catch (Exception ex)
            {
                return ApiResponse<FarmActivityPhotoDto>.Failure("File Upload Error", ex.Message, new Dictionary<string, List<string>> { { "FileUploadError", new List<string> { ex.Message } } });
            }
        }
    }
}
