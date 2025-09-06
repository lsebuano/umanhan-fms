using FluentValidation;
using Umanhan.Dtos;
using Umanhan.Services.Interfaces;

namespace Umanhan.Masterdata.Api.Endpoints
{
    public class CategoryEndpoints
    {
        private readonly ICategoryService _categoryService;
        private readonly IValidator<CategoryDto> _validator;
        private readonly ILogger<CategoryEndpoints> _logger;
        //private readonly ICacheService _cacheService;

        private const string MODULE_CACHE_KEY = "category";

        public CategoryEndpoints(ICategoryService categoryService, 
            IValidator<CategoryDto> validator, 
            ILogger<CategoryEndpoints> logger
            )
        {
            _categoryService = categoryService;
            _validator = validator;
            _logger = logger;
            //_cacheService = cacheService;
        }

        public async Task<IResult> GetAllCategoriesAsync()
        {
            try
            {
                string key = $"{MODULE_CACHE_KEY}:list";
                //var result = await _cacheService.GetOrSetAsync(key, async () =>
                //{
                    var result = await _categoryService.GetAllCategoriesAsync("Tasks", "Inventories.Unit").ConfigureAwait(false);
                //    return result;
                //});
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving categories");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetCategoryByIdAsync(Guid id)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id, "Tasks", "Inventories.Unit").ConfigureAwait(false);
                return category is not null ? Results.Ok(category) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving category with ID {CategoryId}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> CreateCategoryAsync(CategoryDto category)
        {
            var validationResult = await _validator.ValidateAsync(category).ConfigureAwait(false);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            try
            {
                var newCategory = await _categoryService.CreateCategoryAsync(category).ConfigureAwait(false);

                //string key = $"{MODULE_CACHE_KEY}:list";
                //_ = _cacheService.RemoveAsync(key);

                return Results.Created($"/api/categories/{newCategory.CategoryId}", newCategory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> UpdateCategoryAsync(Guid id, CategoryDto category)
        {
            if (id != category.CategoryId)
                return Results.BadRequest("Category ID mismatch");

            var validationResult = await _validator.ValidateAsync(category).ConfigureAwait(false);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            try
            {
                var updatedCategory = await _categoryService.UpdateCategoryAsync(category).ConfigureAwait(false);
                if (updatedCategory is not null)
                {
                    //// Clear cache for the list of system settings
                    //_ = _cacheService.RemoveAsync($"{MODULE_CACHE_KEY}:list");
                    return Results.Ok(updatedCategory);
                }
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category with ID {CategoryId}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> DeleteCategoryAsync(Guid id)
        {
            try
            {
                var deletedCategory = await _categoryService.DeleteCategoryAsync(id).ConfigureAwait(false);
                if (deletedCategory is not null)
                {
                    //// Clear cache for the list of system settings
                    //_ = _cacheService.RemoveAsync($"{MODULE_CACHE_KEY}:list");
                    return Results.Ok(deletedCategory);
                }
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category with ID {CategoryId}", id);
                return Results.Problem(ex.Message);
            }
        }
    }
}