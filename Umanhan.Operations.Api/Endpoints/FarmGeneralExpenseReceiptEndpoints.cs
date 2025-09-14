using FluentValidation;
using Umanhan.Dtos;
using Umanhan.Services;

namespace Umanhan.Operations.Api.Endpoints
{
    public class FarmGeneralExpenseReceiptEndpoints
    {
        private readonly FarmGeneralExpenseService _farmGeneralExpenseService;
        private readonly FarmGeneralExpenseReceiptService _farmGeneralExpenseReceiptService;
        private readonly IValidator<FarmGeneralExpenseReceiptDto> _validator;
        private readonly ILogger<FarmGeneralExpenseReceiptEndpoints> _logger;

        private const string THUMBS_FOLDER = "thumbnails/";

        public FarmGeneralExpenseReceiptEndpoints(FarmGeneralExpenseReceiptService farmGeneralExpenseReceiptService,
            IValidator<FarmGeneralExpenseReceiptDto> validator,
            ILogger<FarmGeneralExpenseReceiptEndpoints> logger,
            FarmGeneralExpenseService farmGeneralExpenseService)
        {
            _farmGeneralExpenseReceiptService = farmGeneralExpenseReceiptService;
            _validator = validator;
            _logger = logger;
            _farmGeneralExpenseService = farmGeneralExpenseService;
        }

        public async Task<IResult> GetFarmGeneralExpenseReceiptByGeneralExpenseAsync(Guid farmGeneralExpenseId)
        {
            try
            {
                var farmGeneralExpenseReceipts = await _farmGeneralExpenseReceiptService.GetFarmGeneralExpenseReceiptsByGeneralExpenseAsync(farmGeneralExpenseId).ConfigureAwait(false);
                return farmGeneralExpenseReceipts is not null ? Results.Ok(farmGeneralExpenseReceipts) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving farm general expense receipts by farmGeneralExpense ID {GeneralExpenseId}", farmGeneralExpenseId);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetFarmGeneralExpenseReceiptByIdAsync(Guid id)
        {
            try
            {
                var farmGeneralExpenseReceipt = await _farmGeneralExpenseReceiptService.GetFarmGeneralExpenseReceiptByIdAsync(id).ConfigureAwait(false);
                return farmGeneralExpenseReceipt is not null ? Results.Ok(farmGeneralExpenseReceipt) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving farm general expense receipt by ID {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> CreateFarmGeneralExpenseReceiptAsync(FarmGeneralExpenseReceiptDto farmGeneralExpenseReceipt)
        {
            var validationResult = await _validator.ValidateAsync(farmGeneralExpenseReceipt).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var newFarmGeneralExpenseReceipt = await _farmGeneralExpenseReceiptService.CreateFarmGeneralExpenseReceiptAsync(farmGeneralExpenseReceipt).ConfigureAwait(false);
                return Results.Created($"/api/farm-general-expense-receipts/{newFarmGeneralExpenseReceipt.ReceiptId}", newFarmGeneralExpenseReceipt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating farm general expense receipt");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> UpdateFarmGeneralExpenseReceiptAsync(Guid id, FarmGeneralExpenseReceiptDto farmGeneralExpenseReceipt)
        {
            if (id != farmGeneralExpenseReceipt.ReceiptId)
            {
                return Results.BadRequest("Farm General Expense Receipt ID mismatch");
            }

            var validationResult = await _validator.ValidateAsync(farmGeneralExpenseReceipt).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var updatedFarmGeneralExpenseReceipt = await _farmGeneralExpenseReceiptService.UpdateFarmGeneralExpenseReceiptAsync(farmGeneralExpenseReceipt).ConfigureAwait(false);
                return updatedFarmGeneralExpenseReceipt is not null ? Results.Ok(updatedFarmGeneralExpenseReceipt) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating farm general expense receipt with ID {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> DeleteFarmGeneralExpenseReceiptAsync(Guid id)
        {
            try
            {
                var deletedFarmGeneralExpenseReceipt = await _farmGeneralExpenseReceiptService.DeleteFarmGeneralExpenseReceiptAsync(id).ConfigureAwait(false);
                return deletedFarmGeneralExpenseReceipt is not null ? Results.Ok(deletedFarmGeneralExpenseReceipt) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting farm general expense receipt with ID {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> CreateFarmGeneralExpenseReceiptAsync(S3PhotoUploadDto obj, string s3BucketUrl)
        {
            if (string.IsNullOrEmpty(obj.S3ObjectKey))
            {
                return Results.BadRequest("S3 object key must be provided.");
            }

            try
            {
                var farmGeneralExpense = await _farmGeneralExpenseService.GetFarmGeneralExpenseByIdAsync(obj.ActivityId).ConfigureAwait(false);
                if (farmGeneralExpense is null)
                {
                    return Results.NotFound($"Farm General Expense with ID {obj.ActivityId} not found.");
                }

                var farmGeneralExpenseReceipt = new FarmGeneralExpenseReceiptDto
                {
                    GeneralExpenseId = obj.ActivityId,
                    Notes = obj.Notes,
                    MimeType = Uri.UnescapeDataString(obj.S3ObjectContentType),
                    //ImageFull = obj.S3ObjectKey,
                    ReceiptUrlFull = $"{s3BucketUrl.TrimEnd('/')}/{obj.S3ObjectKey.TrimStart('/')}",
                    //ImageThumbnail = $"{THUMBS_FOLDER}{obj.S3ObjectKey.TrimStart('/')}",
                    ReceiptUrlThumbnail = $"{s3BucketUrl.TrimEnd('/')}/{THUMBS_FOLDER}{obj.S3ObjectKey.TrimStart('/')}"
                };

                var newFarmGeneralExpenseReceipt = await _farmGeneralExpenseReceiptService.CreateFarmGeneralExpenseReceiptAsync(farmGeneralExpenseReceipt).ConfigureAwait(false);
                return Results.Created($"/api/farm-general-expense-receipts/{newFarmGeneralExpenseReceipt.ReceiptId}", newFarmGeneralExpenseReceipt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating farm general expense receipt with General Expense ID {Id}", obj.ActivityId);
                return Results.Problem(ex.Message);
            }
        }
    }
}
