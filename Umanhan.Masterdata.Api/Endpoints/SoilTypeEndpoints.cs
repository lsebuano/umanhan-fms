using FluentValidation;
using Umanhan.Dtos;
using Umanhan.Services;

namespace Umanhan.Masterdata.Api.Endpoints
{
    public class SoilTypeEndpoints
    {
        private readonly SoilTypeService _soilTypeService;
        private readonly IValidator<SoilTypeDto> _validator;
        private readonly ILogger<SoilTypeEndpoints> _logger;
        //private readonly ICacheService _cacheService;

        private const string MODULE_CACHE_KEY = "soiltype";

        public SoilTypeEndpoints(SoilTypeService soilTypeService, IValidator<SoilTypeDto> validator, ILogger<SoilTypeEndpoints> logger)
        {
            _soilTypeService = soilTypeService;
            _validator = validator;
            _logger = logger;
            //_cacheService = cacheService;
        }

        public async Task<IResult> GetAllSoilTypesAsync()
        {
            try
            {
                string key = $"{MODULE_CACHE_KEY}:list";
                //var result = await _cacheService.GetOrSetAsync(key, async () =>
                //{
                var result = await _soilTypeService.GetAllSoilTypesAsync().ConfigureAwait(false);
                //    return result;
                //});
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving soil types");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetSoilTypeByIdAsync(Guid id)
        {
            try
            {
                var soilType = await _soilTypeService.GetSoilTypeByIdAsync(id).ConfigureAwait(false);
                return soilType is not null ? Results.Ok(soilType) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving soil type with ID {SoilTypeId}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> CreateSoilTypeAsync(SoilTypeDto soilType)
        {
            var validationResult = await _validator.ValidateAsync(soilType).ConfigureAwait(false);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            try
            {
                var newSoilType = await _soilTypeService.CreateSoilTypeAsync(soilType).ConfigureAwait(false);

                //string key = $"{MODULE_CACHE_KEY}:list";
                //_ = _cacheService.RemoveAsync(key);

                return Results.Created($"/api/soil-types/{newSoilType.SoilId}", newSoilType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating soil type");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> UpdateSoilTypeAsync(Guid id, SoilTypeDto soilType)
        {
            if (id != soilType.SoilId)
                return Results.BadRequest("Soil Type ID mismatch");

            var validationResult = await _validator.ValidateAsync(soilType).ConfigureAwait(false);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            try
            {
                var updatedSoilType = await _soilTypeService.UpdateSoilTypeAsync(soilType).ConfigureAwait(false);
                if (updatedSoilType is not null)
                {
                    //// Clear cache for the list of system settings
                    //_ = _cacheService.RemoveAsync($"{MODULE_CACHE_KEY}:list");
                    return Results.Ok(updatedSoilType);
                }
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating soil type with ID {SoilTypeId}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> DeleteSoilTypeAsync(Guid id)
        {
            try
            {
                var deletedSoilType = await _soilTypeService.DeleteSoilTypeAsync(id).ConfigureAwait(false);
                if (deletedSoilType is not null)
                {
                    //// Clear cache for the list of system settings
                    //_ = _cacheService.RemoveAsync($"{MODULE_CACHE_KEY}:list");
                    return Results.Ok(deletedSoilType);
                }
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting soil type with ID {SoilTypeId}", id);
                return Results.Problem(ex.Message);
            }
        }
    }
}
