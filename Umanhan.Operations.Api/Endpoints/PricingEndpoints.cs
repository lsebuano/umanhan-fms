using FluentValidation;
using Umanhan.Dtos;
using Umanhan.Services;

namespace Umanhan.Operations.Api.Endpoints
{
    public class PricingEndpoints
    {
        private readonly PricingService _pricingService;
        private readonly IValidator<PricingDto> _validator;
        private readonly ILogger<PricingEndpoints> _logger;

        public PricingEndpoints(PricingService pricingService, IValidator<PricingDto> validator, ILogger<PricingEndpoints> logger)
        {
            _pricingService = pricingService;
            _validator = validator;
            _logger = logger;
        }

        public async Task<IResult> GetAllPricingsAsync()
        {
            try
            {
                var list = await _pricingService.GetAllPricingsAsync().ConfigureAwait(false);
                return Results.Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all pricings");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetPricingsByFarmIdAsync(Guid farmId)
        {
            try
            {
                var list = await _pricingService.GetPricingsByFarmIdAsync(farmId).ConfigureAwait(false);
                return Results.Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pricings for farm {FarmId}", farmId);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetPricingByIdAsync(Guid id)
        {
            try
            {
                var obj = await _pricingService.GetPricingByIdAsync(id).ConfigureAwait(false);
                return Results.Ok(obj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pricing by ID {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> CreatePricingAsync(PricingDto obj)
        {
            var validationResult = await _validator.ValidateAsync(obj).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var newObj = await _pricingService.CreatePricingAsync(obj).ConfigureAwait(false);
                return Results.Created($"/api/pricing/{newObj.PricingId}", newObj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new pricing");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> UpdatePricingAsync(Guid id, PricingDto obj)
        {
            if (id != obj.PricingId)
            {
                return Results.BadRequest("Pricing ID mismatch");
            }

            var validationResult = await _validator.ValidateAsync(obj).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var updatedObj = await _pricingService.UpdatePricingAsync(obj).ConfigureAwait(false);
                return updatedObj is not null ? Results.Ok(updatedObj) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating pricing with ID {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> DeletePricingAsync(Guid id)
        {
            try
            {
                var deletedObj = await _pricingService.DeletePricingAsync(id).ConfigureAwait(false);
                return deletedObj is not null ? Results.Ok(deletedObj) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting pricing with ID {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> CalculateFinalPriceAsync(Guid profileId, decimal basePrice)
        {
            try
            {
                var finalPrice = await _pricingService.CalculateFinalPriceAsync(profileId, basePrice).ConfigureAwait(false);
                return Results.Ok(finalPrice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating final price for profile {ProfileId} with base price {BasePrice}", profileId, basePrice);
                return Results.Problem(ex.Message);
            }
        }
    }
}
