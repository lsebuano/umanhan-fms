using FluentValidation;
using Umanhan.Masterdata.Api;
using Umanhan.Models.Dtos;
using Umanhan.Services;
using Umanhan.Services.Interfaces;

namespace Umanhan.Masterdata.Api.Endpoints
{
    public class LaborerEndpoints
    {
        private readonly LaborerService _laborerService;
        private readonly IValidator<LaborerDto> _validator;
        private readonly ILogger<LaborerEndpoints> _logger;
        //private readonly ICacheService _cacheService;

        private const string MODULE_CACHE_KEY = "laborer";

        public LaborerEndpoints(LaborerService laborerService, IValidator<LaborerDto> validator, ILogger<LaborerEndpoints> logger)
        {
            _laborerService = laborerService;
            _validator = validator;
            _logger = logger;
            //_cacheService = cacheService;
        }

        public async Task<IResult> GetAllLaborersAsync()
        {
            try
            {
                //string key = $"{MODULE_CACHE_KEY}:list";
                //var result = await _cacheService.GetOrSetAsync(key, async () =>
                //{
                var result = await _laborerService.GetAllLaborersAsync().ConfigureAwait(false);
                //    return laborers;
                //});
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving laborers");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetLaborerByIdAsync(Guid id)
        {
            try
            {
                var laborer = await _laborerService.GetLaborerByIdAsync(id).ConfigureAwait(false);
                return laborer is not null ? Results.Ok(laborer) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving laborer with ID {LaborerId}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> CreateLaborerAsync(LaborerDto laborer)
        {
            var validationResult = await _validator.ValidateAsync(laborer).ConfigureAwait(false);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            try
            {
                var newLaborer = await _laborerService.CreateLaborerAsync(laborer).ConfigureAwait(false);

                //string key = $"{MODULE_CACHE_KEY}:list";
                //_ = _cacheService.RemoveAsync(key);

                return Results.Created($"/api/laborers/{newLaborer.LaborerId}", newLaborer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating laborer");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> UpdateLaborerAsync(Guid id, LaborerDto laborer)
        {
            if (id != laborer.LaborerId)
                return Results.BadRequest("Laborer ID mismatch");

            var validationResult = await _validator.ValidateAsync(laborer).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var updatedLaborer = await _laborerService.UpdateLaborerAsync(laborer).ConfigureAwait(false);
                if (updatedLaborer is not null)
                {
                    //// Clear cache for the list of system settings
                    //_ = _cacheService.RemoveAsync($"{MODULE_CACHE_KEY}:list");
                    return Results.Ok(updatedLaborer);
                }
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating laborer with ID {LaborerId}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> DeleteLaborerAsync(Guid id)
        {
            try
            {
                var deletedLaborer = await _laborerService.DeleteLaborerAsync(id).ConfigureAwait(false);
                if (deletedLaborer is not null)
                {
                    //// Clear cache for the list of system settings
                    //_ = _cacheService.RemoveAsync($"{MODULE_CACHE_KEY}:list");
                    return Results.Ok(deletedLaborer);
                }
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting laborer with ID {LaborerId}", id);
                return Results.Problem(ex.Message);
            }
        }
    }
}
