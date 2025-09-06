using Umanhan.Dtos;
using Umanhan.Dtos.HelperModels;

namespace Umanhan.WebPortal.Spa.Services
{
    public class ProductTypeService
    {
        private readonly ApiService _apiService;

        public ProductTypeService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Task<ApiResponse<IEnumerable<ProductTypeDto>>> GetAllProductTypesAsync()
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<ProductTypeDto>>("MasterdataAPI", "api/product-types");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<ProductTypeDto>> GetProductTypeByIdAsync(Guid id)
        {
            try
            {
                return _apiService.GetAsync<ProductTypeDto>("MasterdataAPI", $"api/product-types/{id}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ProductTypeDto> CreateProductTypeAsync(ProductTypeDto productType)
        {
            try
            {
                var response = await _apiService.PostAsync<ProductTypeDto, ProductTypeDto>("MasterdataAPI", "api/product-types", productType).ConfigureAwait(false);
                if (!response.IsSuccess)
                {
                    if (response.Errors != null && response.Errors.Any())
                    {
                        var errors = new Dictionary<string, List<string>>();
                        foreach (var error in response.Errors)
                        {
                            errors.Add(error.Key, error.Value);
                        }
                        response.ErrorMessage = string.Join(", ", errors.SelectMany(x => x.Value));
                    }
                }
                return response.Data;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public async Task<ProductTypeDto> UpdateProductTypeAsync(ProductTypeDto productType)
        {
            try
            {
                var response = await _apiService.PutAsync<ProductTypeDto, ProductTypeDto>("MasterdataAPI", $"api/product-types/{productType.TypeId}", productType).ConfigureAwait(false);
                if (!response.IsSuccess)
                {
                    if (response.Errors != null && response.Errors.Any())
                    {
                        var errors = new Dictionary<string, List<string>>();
                        foreach (var error in response.Errors)
                        {
                            errors.Add(error.Key, error.Value);
                        }
                        response.ErrorMessage = string.Join(", ", errors.SelectMany(x => x.Value));
                    }
                }
                return response.Data;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public async Task<ProductTypeDto> DeleteProductTypeAsync(Guid id)
        {
            try
            {
                var response = await _apiService.DeleteAsync<ProductTypeDto>("MasterdataAPI", $"api/product-types/{id}").ConfigureAwait(false);
                if (!response.IsSuccess)
                {
                    if (response.Errors != null && response.Errors.Any())
                    {
                        var errors = new Dictionary<string, List<string>>();
                        foreach (var error in response.Errors)
                        {
                            errors.Add(error.Key, error.Value);
                        }
                        response.ErrorMessage = string.Join(", ", errors.SelectMany(x => x.Value));
                    }
                }
                return response.Data;
            }
            catch (Exception ex)
            {

            }
            return null;
        }
    }
}
