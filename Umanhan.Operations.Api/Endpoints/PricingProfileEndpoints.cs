using FluentValidation;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Services;

namespace Umanhan.Operations.Api.Endpoints
{
    public class PricingProfileEndpoints
    {
        private readonly PricingProfileService _pricingProfileService;
        private readonly IValidator<PricingProfileDto> _validator;
        private readonly ILogger<PricingProfileEndpoints> _logger;

        public PricingProfileEndpoints(PricingProfileService pricingService, IValidator<PricingProfileDto> validator, ILogger<PricingProfileEndpoints> logger)
        {
            _pricingProfileService = pricingService;
            _validator = validator;
            _logger = logger;
        }

        public async Task<IResult> GetPricingProfilesByFarmIdAsync(Guid farmId)
        {
            try
            {
                var list = await _pricingProfileService.GetPricingProfilesByFarmIdAsync(farmId).ConfigureAwait(false);
                return Results.Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pricings for farm {FarmId}", farmId);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetPricingProfileByIdAsync(Guid id)
        {
            try
            {
                var obj = await _pricingProfileService.GetPricingProfileByIdAsync(id).ConfigureAwait(false);
                return Results.Ok(obj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pricing profile by ID {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> CreatePricingProfileAsync(PricingProfileDto obj)
        {
            var validationResult = await _validator.ValidateAsync(obj).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var newObj = await _pricingProfileService.CreatePricingProfileAsync(obj).ConfigureAwait(false);
                return Results.Created($"/api/pricing-profile/{newObj.ProfileId}", newObj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new pricing profile.");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> UpdatePricingProfileAsync(Guid id, PricingProfileDto obj)
        {
            if (id != obj.ProfileId)
            {
                return Results.BadRequest("Pricing Profile ID mismatch");
            }

            var validationResult = await _validator.ValidateAsync(obj).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var updatedObj = await _pricingProfileService.UpdatePricingProfileAsync(obj).ConfigureAwait(false);
                return updatedObj is not null ? Results.Ok(updatedObj) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating pricing profile with ID {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> DeletePricingProfileAsync(Guid id)
        {
            try
            {
                var deletedObj = await _pricingProfileService.DeletePricingProfileAsync(id).ConfigureAwait(false);
                return deletedObj is not null ? Results.Ok(deletedObj) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting pricing profile with ID {Id}", id);
                return Results.Problem(ex.Message);
            }
        }
    }
}
