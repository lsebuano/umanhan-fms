using FluentValidation;
using Umanhan.Masterdata.Api;
using Umanhan.Models.Dtos;
using Umanhan.Services;
using Umanhan.Services.Interfaces;

namespace Umanhan.Masterdata.Api.Endpoints
{
    public class ProductTypeEndpoints
    {
        private readonly ProductTypeService _productTypeService;
        private readonly IValidator<ProductTypeDto> _validator;
        private readonly ILogger<ProductTypeEndpoints> _logger;
        //private readonly ICacheService _cacheService;

        private const string MODULE_CACHE_KEY = "producttype";

        public ProductTypeEndpoints(ProductTypeService productTypeService, IValidator<ProductTypeDto> validator, ILogger<ProductTypeEndpoints> logger)
        {
            _productTypeService = productTypeService;
            _validator = validator;
            _logger = logger;
            //_cacheService = cacheService;
        }

        public async Task<IResult> GetAllProductTypesAsync()
        {
            try
            {
                string key = $"{MODULE_CACHE_KEY}:list";
                //var result = await _cacheService.GetOrSetAsync(key, async () =>
                //{
                var result = await _productTypeService.GetAllProductTypesAsync().ConfigureAwait(false);
                //    return result;
                //});
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product types");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetProductTypeByIdAsync(Guid id)
        {
            try
            {
                var productType = await _productTypeService.GetProductTypeByIdAsync(id).ConfigureAwait(false);
                return productType is not null ? Results.Ok(productType) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product type with ID {ProductTypeId}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> CreateProductTypeAsync(ProductTypeDto productType)
        {
            var validationResult = await _validator.ValidateAsync(productType).ConfigureAwait(false);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            try
            {
                var newProductType = await _productTypeService.CreateProductTypeAsync(productType).ConfigureAwait(false);

                //string key = $"{MODULE_CACHE_KEY}:list";
                //_ = _cacheService.RemoveAsync(key);

                return Results.Created($"/api/product-types/{newProductType.TypeId}", newProductType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product type");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> UpdateProductTypeAsync(Guid id, ProductTypeDto productType)
        {
            if (id != productType.TypeId)
                return Results.BadRequest("Product Type ID mismatch");

            var validationResult = await _validator.ValidateAsync(productType).ConfigureAwait(false);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            try
            {
                var updatedProductType = await _productTypeService.UpdateProductTypeAsync(productType).ConfigureAwait(false);
                if (updatedProductType is not null)
                {
                    //// Clear cache for the list of system settings
                    //_ = _cacheService.RemoveAsync($"{MODULE_CACHE_KEY}:list");
                    return Results.Ok(updatedProductType);
                }
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product type with ID {ProductTypeId}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> DeleteProductTypeAsync(Guid id)
        {
            try
            {
                var deletedProductType = await _productTypeService.DeleteProductTypeAsync(id).ConfigureAwait(false);
                if (deletedProductType is not null)
                {
                    //// Clear cache for the list of system settings
                    //_ = _cacheService.RemoveAsync($"{MODULE_CACHE_KEY}:list");
                    return Results.Ok(deletedProductType);
                }
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product type with ID {ProductTypeId}", id);
                return Results.Problem(ex.Message);
            }
        }
    }
}
