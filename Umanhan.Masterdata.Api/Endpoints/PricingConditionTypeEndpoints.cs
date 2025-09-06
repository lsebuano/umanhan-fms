using Umanhan.Services;

namespace Umanhan.Masterdata.Api.Endpoints
{
    public class PricingConditionTypeEndpoints
    {
        private readonly PricingConditionTypeService _pricingConditionTypeService;
        //private readonly IValidator<PricingConditionTypeDto> _validator;
        private readonly ILogger<PricingConditionTypeEndpoints> _logger;
        //private readonly ICacheService _cacheService;

        private const string MODULE_CACHE_KEY = "pricingconditiontype";

        public PricingConditionTypeEndpoints(PricingConditionTypeService pricingConditionTypeService, ILogger<PricingConditionTypeEndpoints> logger)
        {
            _pricingConditionTypeService = pricingConditionTypeService;
            //_validator = validator;
            _logger = logger;
            //_cacheService = cacheService;
        }

        public async Task<IResult> GetPricingConditionTypesAsync()
        {
            try
            {
                string key = $"{MODULE_CACHE_KEY}:list";
                //var result = await _cacheService.GetOrSetAsync(key, async () =>
                //{
                var result = await _pricingConditionTypeService.GetAllPricingConditionTypesAsync().ConfigureAwait(false);
                //    return result;
                //});
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pricing condition types");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetPricingConditionTypeByIdAsync(Guid id)
        {
            try
            {
                var pricingConditionType = await _pricingConditionTypeService.GetPricingConditionTypeByIdAsync(id).ConfigureAwait(false);
                return pricingConditionType is not null ? Results.Ok(pricingConditionType) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pricing condition type with ID {PricingConditionTypeId}", id);
                return Results.Problem(ex.Message);
            }
        }
    }
}
