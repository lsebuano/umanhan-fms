using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Models.Models;

namespace Umanhan.WebPortal.Spa.Services
{
    public class CategoryService
    {
        private readonly ApiService _apiService;

        public CategoryService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Task<ApiResponse<IEnumerable<CategoryDto>>> GetAllCategoriesAsync()
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<CategoryDto>>("MasterdataAPI", "api/categories");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<CategoryDto>> GetCategoryByIdAsync(Guid id)
        {
            try
            {
                return _apiService.GetAsync<CategoryDto>("MasterdataAPI", $"api/categories/{id}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<CategoryDto> CreateCategoryAsync(CategoryDto category)
        {
            try
            {
                var response = await _apiService.PostAsync<CategoryDto, CategoryDto>("MasterdataAPI", "api/categories", category).ConfigureAwait(false);
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

        public async Task<CategoryDto> UpdateCategoryAsync(CategoryDto category)
        {
            try
            {
                var response = await _apiService.PutAsync<CategoryDto, CategoryDto>("MasterdataAPI", $"api/categories/{category.CategoryId}", category).ConfigureAwait(false);
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

        public async Task<CategoryDto> DeleteCategoryAsync(Guid id)
        {
            try
            {
                var response = await _apiService.DeleteAsync<CategoryDto>("MasterdataAPI", $"api/categories/{id}").ConfigureAwait(false);
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
