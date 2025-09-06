using Amazon.S3;
using Amazon.S3.Model;
using FluentValidation;
using Umanhan.Dtos;
using Umanhan.Services;

namespace Umanhan.Operations.Api.Endpoints
{
    public class FarmInventoryEndpoints
    {
        private readonly FarmInventoryService _farmInventoryService;
        private readonly IValidator<FarmInventoryDto> _validator;
        private readonly ILogger<FarmInventoryEndpoints> _logger;
        private readonly IAmazonS3 _s3Client;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;

        private const string THUMBS_FOLDER = "thumbnails/";

        private async Task<(string, string)> GenerateS3PresignedUrlAsync(string bucketName, string filename, string contentType)
        {
            filename = filename.ToLowerInvariant();
            contentType = Uri.UnescapeDataString(contentType);

            if (string.IsNullOrWhiteSpace(bucketName))
            {
                throw new ArgumentException("Bucket name must be provided.");
            }

            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentException("Filename must be provided.");
            }
            if (filename.Length > 255)
            {
                throw new ArgumentException("Filename must not exceed 255 characters.");
            }
            if (!filename.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) &&
                !filename.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) &&
                !filename.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Only .jpg, .jpeg, and .png files are allowed.");
            }
            if (filename.Contains("..") || filename.Contains("//") || filename.Contains("\\"))
            {
                throw new ArgumentException("Invalid filename format.");
            }
            if (filename.Contains(" "))
            {
                filename = filename.Replace(" ", "_");
            }
            if (filename.StartsWith("/") || filename.StartsWith("\\"))
            {
                filename = filename.TrimStart('/', '\\');
            }

            var request = new GetPreSignedUrlRequest
            {
                BucketName = bucketName,
                Key = filename,
                Verb = HttpVerb.PUT,
                ContentType = contentType,
                Expires = DateTime.UtcNow.AddMinutes(5) // URL valid for 5 minutes
            };

            var url = await _s3Client.GetPreSignedURLAsync(request).ConfigureAwait(false);
            return (url, filename);
        }

        public FarmInventoryEndpoints(FarmInventoryService farmInventoryService,
            IValidator<FarmInventoryDto> validator,
            ILogger<FarmInventoryEndpoints> logger,
            IAmazonS3 s3Client,
            IHttpContextAccessor httpContextAccessor,
            HttpClient httpClient)
        {
            _farmInventoryService = farmInventoryService;
            _validator = validator;
            _logger = logger;
            _s3Client = s3Client;
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClient;
        }

        public async Task<IResult> GetFarmInventoriesAsync(Guid farmId)
        {
            try
            {
                var inventories = await _farmInventoryService.GetFarmInventoriesAsync(farmId, "Farm", "Inventory.Unit", "Unit").ConfigureAwait(false);
                return Results.Ok(inventories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving farm inventories for farm {FarmId}", farmId);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetFarmInventoriesAsync(Guid farmId, string behaviors)
        {
            try
            {
                var inventories = await _farmInventoryService.GetFarmInventoriesAsync(farmId, "Farm", "Inventory.Unit", "Unit").ConfigureAwait(false);
                if (string.IsNullOrEmpty(behaviors))
                    return Results.Ok(inventories);

                // filter by behaviors
                string[] behaviorsArray = behaviors.Split(',');
                inventories = inventories.Where(x => behaviorsArray.Any(b => string.Equals(b, x.InventoryCategoryConsumptionBehavior, StringComparison.OrdinalIgnoreCase)));

                return Results.Ok(inventories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving farm inventories for farm {FarmId} with behaviors {Behaviors}", farmId, behaviors);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetFarmInventoryByIdAsync(Guid id)
        {
            try
            {
                var farmInventory = await _farmInventoryService.GetFarmInventoryByIdAsync(id, "Farm", "Inventory.Unit", "Unit").ConfigureAwait(false);
                if (farmInventory is null)
                {
                    return Results.NotFound($"Farm inventory with ID {id} not found.");
                }
                return Results.Ok(farmInventory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving farm inventory with ID {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetS3PresignedUrlAsync(string photosBucketName, string filename, string contentType)
        {
            try
            {
                var url = await this.GenerateS3PresignedUrlAsync(photosBucketName, filename, contentType).ConfigureAwait(false);
                return Results.Ok(new Dictionary<string, string> {
                    { "Url", url.Item1 },
                    { "Key", url.Item2 }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting presigned URL for filename {Filename}", filename);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> CreateFarmInventoryAsync(FarmInventoryDto farmInventory)
        {
            var validationResult = await _validator.ValidateAsync(farmInventory).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var newFarmInventory = await _farmInventoryService.CreateFarmInventoryAsync(farmInventory).ConfigureAwait(false);
                return Results.Created($"/api/farm-inventories/{newFarmInventory.FarmInventoryId}", newFarmInventory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating farm inventory");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> UpdateFarmInventoryAsync(Guid id, FarmInventoryDto farmInventory)
        {
            if (id != farmInventory.FarmInventoryId)
            {
                return Results.BadRequest("Farm Inventory ID mismatch");
            }

            var validationResult = await _validator.ValidateAsync(farmInventory).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var updatedFarmInventory = await _farmInventoryService.UpdateFarmInventoryAsync(farmInventory).ConfigureAwait(false);
                return updatedFarmInventory is not null ? Results.Ok(updatedFarmInventory) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating farm inventory with ID {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> UpdateFarmInventoryPhotoAsync(Guid id, string s3ObjectKey, string s3ObjectContentType, string s3BucketUrl)
        {
            if (string.IsNullOrEmpty(s3ObjectKey))
            {
                return Results.BadRequest("S3 object key must be provided.");
            }

            try
            {
                var farmInventory = await _farmInventoryService.GetFarmInventoryByIdAsync(id).ConfigureAwait(false);
                if (farmInventory is null)
                {
                    return Results.NotFound($"Farm inventory with ID {id} not found.");
                }

                farmInventory.InventoryItemImageContentType = Uri.UnescapeDataString(s3ObjectContentType);
                farmInventory.InventoryItemImageFull = s3ObjectKey;
                farmInventory.InventoryItemImageS3UrlFull = $"{s3BucketUrl.TrimEnd('/')}/{s3ObjectKey.TrimStart('/')}";
                farmInventory.InventoryItemImageThumbnail = $"{THUMBS_FOLDER}{s3ObjectKey.TrimStart('/')}";
                farmInventory.InventoryItemImageS3UrlThumbnail = $"{s3BucketUrl.TrimEnd('/')}/{THUMBS_FOLDER}{s3ObjectKey.TrimStart('/')}";

                var updatedFarmInventory = await _farmInventoryService.UpdateFarmInventoryPhotoAsync(farmInventory).ConfigureAwait(false);
                return updatedFarmInventory is not null ? Results.Ok(updatedFarmInventory) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating farm inventory with ID {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> DeleteFarmInventoryAsync(Guid id)
        {
            try
            {
                var deletedFarmInventory = await _farmInventoryService.DeleteFarmInventoryAsync(id).ConfigureAwait(false);
                return deletedFarmInventory is not null ? Results.Ok(deletedFarmInventory) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting farm inventory with ID {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> UploadInventoryPhotoAsync(string bucketName, string filename)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var httpRequest = httpContext.Request;
                var contentType = httpRequest.ContentType ?? "application/octet-stream";

                string key = $"{filename.TrimStart('/')}";

                // Read raw stream from request body
                using var stream = new MemoryStream();
                await httpRequest.Body.CopyToAsync(stream).ConfigureAwait(false);
                stream.Position = 0;

                var url = await this.GenerateS3PresignedUrlAsync(bucketName, key, contentType);

                using var streamContent = new StreamContent(stream);
                var response = await _httpClient.PutAsync(url.Item1, streamContent);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Upload succeeded.");
                }
                else
                {
                    Console.WriteLine("Upload failed.");
                }

                //var request = new PutObjectRequest
                //{
                //    BucketName = bucketName,
                //    Key = key,
                //    InputStream = stream,
                //    ContentType = contentType,
                //};
                ////request.Metadata.Add("x-amz-meta-uploaded-by", "Umanhan.Operations.Api");
                ////request.Metadata.Add("x-amz-meta-uploaded-at", DateTime.UtcNow.ToString("o"));
                ////request.Metadata.Add("x-amz-meta-file-size", httpRequest.Headers.ContentLength.ToString());
                ////request.Metadata.Add("x-amz-meta-file-extension", Path.GetExtension(key).TrimStart('.').ToLowerInvariant());
                ////request.Metadata.Add("x-amz-meta-content-type", contentType);

                //await _s3Client.PutObjectAsync(request).ConfigureAwait(false);

                return Results.Ok(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading inventory photo with filename {Filename}", filename);
                return Results.Problem(ex.Message);
            }
        }
    }
}
