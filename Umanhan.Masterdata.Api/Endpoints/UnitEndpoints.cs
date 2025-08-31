using FluentValidation;
using Umanhan.Masterdata.Api;
using Umanhan.Models.Dtos;
using Umanhan.Services;
using Umanhan.Services.Interfaces;

namespace Umanhan.Masterdata.Api.Endpoints
{
    public class UnitEndpoints
    {
        private readonly UnitService _unitService;
        private readonly IValidator<UnitDto> _validator;
        private readonly ILogger<UnitEndpoints> _logger;
        //private readonly ICacheService _cacheService;

        private const string MODULE_CACHE_KEY = "unit";

        public UnitEndpoints(UnitService unitService, IValidator<UnitDto> validator, ILogger<UnitEndpoints> logger)
        {
            _unitService = unitService;
            _validator = validator;
            _logger = logger;
            //_cacheService = cacheService;
        }

        public async Task<IResult> GetAllUnitsAsync()
        {
            try
            {
                string key = $"{MODULE_CACHE_KEY}:list";
                //var result = await _cacheService.GetOrSetAsync(key, async () =>
                //{
                var result = await _unitService.GetAllUnitsAsync("Crops", "Inventories").ConfigureAwait(false);
                //    return result;
                //});
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all units");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetUnitByIdAsync(Guid id)
        {
            try
            {
                var unit = await _unitService.GetUnitByIdAsync(id, "Crops", "Inventories").ConfigureAwait(false);
                return unit is not null ? Results.Ok(unit) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving unit with ID {UnitId}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> CreateUnitAsync(UnitDto unit)
        {
            var validationResult = await _validator.ValidateAsync(unit).ConfigureAwait(false);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            try
            {
                var newUnit = await _unitService.CreateUnitAsync(unit).ConfigureAwait(false);

                //string key = $"{MODULE_CACHE_KEY}:list";
                //_ = _cacheService.RemoveAsync(key);

                return Results.Created($"/api/units/{newUnit.UnitId}", newUnit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new unit");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> UpdateUnitAsync(Guid id, UnitDto unit)
        {
            if (id != unit.UnitId)
                return Results.BadRequest("Unit ID mismatch");

            var validationResult = await _validator.ValidateAsync(unit).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var updatedUnit = await _unitService.UpdateUnitAsync(unit).ConfigureAwait(false);
                if (updatedUnit is not null)
                {
                    //// Clear cache for the list of system settings
                    //_ = _cacheService.RemoveAsync($"{MODULE_CACHE_KEY}:list");
                    return Results.Ok(updatedUnit);
                }
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating unit with ID {UnitId}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> DeleteUnitAsync(Guid id)
        {
            try
            {
                var deletedUnit = await _unitService.DeleteUnitAsync(id).ConfigureAwait(false);
                if (deletedUnit is not null)
                {
                    //// Clear cache for the list of system settings
                    //_ = _cacheService.RemoveAsync($"{MODULE_CACHE_KEY}:list");
                    return Results.Ok(deletedUnit);
                }
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting unit with ID {UnitId}", id);
                return Results.Problem(ex.Message);
            }
        }
    }
}
