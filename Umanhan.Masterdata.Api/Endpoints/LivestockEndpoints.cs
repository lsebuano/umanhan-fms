using FluentValidation;
using Umanhan.Dtos;
using Umanhan.Services;

namespace Umanhan.Masterdata.Api.Endpoints
{
    public class LivestockEndpoints
    {
        private readonly LivestockService _livestockService;
        private readonly IValidator<LivestockDto> _validator;
        private readonly ILogger<LivestockEndpoints> _logger;
        //private readonly ICacheService _cacheService;

        private const string MODULE_CACHE_KEY = "livestock";

        public LivestockEndpoints(LivestockService livestockService, IValidator<LivestockDto> validator, ILogger<LivestockEndpoints> logger)
        {
            _livestockService = livestockService;
            _validator = validator;
            _logger = logger;
            //_cacheService = cacheService;
        }

        public async Task<IResult> GetAllLivestocksAsync()
        {
            try
            {
                //string key = $"{MODULE_CACHE_KEY}:list";
                //var result = await _cacheService.GetOrSetAsync(key, async () =>
                //{
                var result = await _livestockService.GetAllLivestocksAsync().ConfigureAwait(false);
                //    return result;
                //});
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving livestocks");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetLivestockByIdAsync(Guid id)
        {
            try
            {
                var livestock = await _livestockService.GetLivestockByIdAsync(id).ConfigureAwait(false);
                return livestock is not null ? Results.Ok(livestock) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving livestock with ID {LivestockId}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> CreateLivestockAsync(LivestockDto livestock)
        {
            var validationResult = await _validator.ValidateAsync(livestock).ConfigureAwait(false);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            try
            {
                var newLivestock = await _livestockService.CreateLivestockAsync(livestock).ConfigureAwait(false);

                //string key = $"{MODULE_CACHE_KEY}:list";
                //_ = _cacheService.RemoveAsync(key);

                return Results.Created($"/api/livestocks/{newLivestock.LivestockId}", newLivestock);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating livestock");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> UpdateLivestockAsync(Guid id, LivestockDto livestock)
        {
            if (id != livestock.LivestockId)
                return Results.BadRequest("Livestock ID mismatch");

            var validationResult = await _validator.ValidateAsync(livestock).ConfigureAwait(false);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            try
            {
                var updatedLivestock = await _livestockService.UpdateLivestockAsync(livestock).ConfigureAwait(false);
                if (updatedLivestock is not null)
                {
                    //// Clear cache for the list of system settings
                    //_ = _cacheService.RemoveAsync($"{MODULE_CACHE_KEY}:list");
                    return Results.Ok(updatedLivestock);
                }
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating livestock with ID {LivestockId}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> DeleteLivestockAsync(Guid id)
        {
            try
            {
                var deletedLivestock = await _livestockService.DeleteLivestockAsync(id).ConfigureAwait(false);
                if (deletedLivestock is not null)
                {
                    //// Clear cache for the list of system settings
                    //_ = _cacheService.RemoveAsync($"{MODULE_CACHE_KEY}:list");
                    return Results.Ok(deletedLivestock);
                }
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting livestock with ID {LivestockId}", id);
                return Results.Problem(ex.Message);
            }
        }
    }
}
