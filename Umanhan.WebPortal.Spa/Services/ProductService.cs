using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Models.Models;

namespace Umanhan.WebPortal.Spa.Services
{
    public class ProductService
    {
        private readonly ApiService _apiService;

        public ProductService(ApiService apiService)
        {
            _apiService = apiService;
        }

        //public Task<ApiResponse<IEnumerable<ProductDto>>> GetAllProductsAsync()
        //{
        //    try
        //    {
        //        return _apiService.GetAsync<IEnumerable<ProductDto>>("MasterdataAPI", "api/products");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        public Task<ApiResponse<IEnumerable<ProductDto>>> GetProductsByFarmAsync(Guid farmId)
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<ProductDto>>("MasterdataAPI", $"api/products/farm-id/{farmId}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<IEnumerable<ProductDto>>> GetProductsByFarmByTypeAsync(Guid farmId, Guid typeId)
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<ProductDto>>("MasterdataAPI", $"api/products/farm-id/{farmId}/type-id/{typeId}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<ProductDto>> GetProductByIdAsync(Guid typeId, Guid id)
        {
            try
            {
                return _apiService.GetAsync<ProductDto>("MasterdataAPI", $"api/products/{id}/type-id/{typeId}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<IEnumerable<FarmProduceInventoryDto>>> GetProductsFromProduceInventoryAsync(Guid farmId)
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<FarmProduceInventoryDto>>("OperationsAPI", $"api/produce/{farmId}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<IEnumerable<FarmProduceInventoryDto>>> GetProductsFromProduceInventoryAsync(Guid farmId, Guid typeId)
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<FarmProduceInventoryDto>>("OperationsAPI", $"api/produce/{farmId}/{typeId}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //public async Task<ProductDto> CreateProductAsync(ProductDto product)
        //{
        //    try
        //    {
        //        var response = await _apiService.PostAsync<ProductDto, ProductDto>("MasterdataAPI", "api/products", product).ConfigureAwait(false);
        //        if (!response.IsSuccess)
        //        {
        //            if (response.Errors != null && response.Errors.Any())
        //            {
        //                var errors = new Dictionary<string, List<string>>();
        //                foreach (var error in response.Errors)
        //                {
        //                    errors.Add(error.Key, error.Value);
        //                }
        //                response.ErrorMessage = string.Join(", ", errors.SelectMany(x => x.Value));
        //            }
        //        }
        //        return response.Data;
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return null;
        //}

        //public async Task<ProductDto> UpdateProductAsync(ProductDto product)
        //{
        //    try
        //    {
        //        var response = await _apiService.PutAsync<ProductDto, ProductDto>("MasterdataAPI", $"api/products/{product.ProductId}", product).ConfigureAwait(false);
        //        if (!response.IsSuccess)
        //        {
        //            if (response.Errors != null && response.Errors.Any())
        //            {
        //                var errors = new Dictionary<string, List<string>>();
        //                foreach (var error in response.Errors)
        //                {
        //                    errors.Add(error.Key, error.Value);
        //                }
        //                response.ErrorMessage = string.Join(", ", errors.SelectMany(x => x.Value));
        //            }
        //        }
        //        return response.Data;
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return null;
        //}

        //public async Task<ProductDto> DeleteProductAsync(Guid id)
        //{
        //    try
        //    {
        //        var response = await _apiService.DeleteAsync<ProductDto>("MasterdataAPI", $"api/products/{id}").ConfigureAwait(false);
        //        if (!response.IsSuccess)
        //        {
        //            if (response.Errors != null && response.Errors.Any())
        //            {
        //                var errors = new Dictionary<string, List<string>>();
        //                foreach (var error in response.Errors)
        //                {
        //                    errors.Add(error.Key, error.Value);
        //                }
        //                response.ErrorMessage = string.Join(", ", errors.SelectMany(x => x.Value));
        //            }
        //        }
        //        return response.Data;
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return null;
        //}
    }
}
