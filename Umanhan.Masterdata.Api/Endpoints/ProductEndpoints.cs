using FluentValidation;
using Umanhan.Masterdata.Api;
using Umanhan.Models.Dtos;
using Umanhan.Models.Models;
using Umanhan.Services;
using Umanhan.Services.Interfaces;

namespace Umanhan.Masterdata.Api.Endpoints
{
    public class ProductEndpoints
    {
        private readonly ProductService _productService;
        private readonly ILogger<ProductEndpoints> _logger;
        //private readonly ICacheService _cacheService;

        private const string MODULE_CACHE_KEY = "product";
        private const string MODULE_CACHE_TAG = "product:list:all";

        public ProductEndpoints(ProductService productService, ILogger<ProductEndpoints> logger)
        {
            _productService = productService;
            _logger = logger;
            //_cacheService = cacheService;
        }

        public async Task<IResult> GetProductsByFarmAsync(Guid farmId)
        {
            try
            {
                //string key = $"{MODULE_CACHE_KEY}:list:{farmId}";
                var products = await _productService.GetProductsByFarmAsync(farmId).ConfigureAwait(false);
                return Results.Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products for farm with ID {FarmId}", farmId);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetProductsByFarmByTypeAsync(Guid farmId, Guid typeId)
        {
            try
            {
                //string key = $"{MODULE_CACHE_KEY}:list:{farmId}:{typeId}";
                var products = await _productService.GetProductsByFarmByTypeAsync(farmId, typeId).ConfigureAwait(false);
                return Results.Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products for farm with ID {FarmId} and type ID {TypeId}", farmId, typeId);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetProductByIdAsync(Guid typeId, Guid id)
        {
            try
            {
                var products = await _productService.GetProductByIdAsync(typeId, id).ConfigureAwait(false);
                return Results.Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product with ID {ProductId} for type ID {TypeId}", id, typeId);
                return Results.Problem(ex.Message);
            }
        }
    }
}
