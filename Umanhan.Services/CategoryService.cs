using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<CategoryService> _logger;

        private static List<CategoryDto> ToCategoryDto(IEnumerable<Category> categories)
        {
            return [.. categories.Select(x => new CategoryDto
            {
                CategoryId = x.Id,
                CategoryName = x.CategoryName,
                Group = x.Group,
                Group2 = x.Group2,
                ConsumptionBehavior = x.ConsumptionBehavior,
                Inventories = x.Inventories.Select(y => new InventoryDto
                {
                    InventoryId = y.Id,
                    ItemName = y.ItemName,
                    CategoryId = y.CategoryId,
                    UnitId = y.UnitId,
                    Unit = y.Unit?.UnitName
                })
                .OrderBy(xx => xx.ItemName),
                Tasks = x.Tasks.Select(y => new TaskDto
                {
                    TaskId = y.Id,
                    TaskName = y.TaskName,
                    CategoryId = y.CategoryId
                })
                .OrderBy(xx => xx.TaskName)
            })
            .OrderBy(x => x.CategoryName)];
        }

        private static CategoryDto ToCategoryDto(Category category)
        {
            return new CategoryDto
            {
                CategoryId = category.Id,
                CategoryName = category.CategoryName,
                Group = category.Group,
                Group2 = category.Group2,
                ConsumptionBehavior = category.ConsumptionBehavior,
                Inventories = category.Inventories.Select(x => new InventoryDto
                {
                    InventoryId = x.Id,
                    ItemName = x.ItemName,
                    CategoryId = x.CategoryId,
                    UnitId = x.UnitId,
                    Unit = x.Unit?.UnitName
                })
                .OrderBy(xx => xx.ItemName),
                Tasks = category.Tasks.Select(x => new TaskDto
                {
                    TaskId = x.Id,
                    TaskName = x.TaskName,
                    CategoryId = x.CategoryId
                })
                .OrderBy(xx => xx.TaskName)
            };
        }

        public CategoryService(IUnitOfWork unitOfWork, 
            ILogger<CategoryService> logger, 
            IUserContextService userContext)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync(params string[] includeEntities)
        {
            try
            {
                var list = await _unitOfWork.Categories.GetAllAsync(includeEntities).ConfigureAwait(false);
                return ToCategoryDto(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Enumerable.Empty<CategoryDto>();
            }
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(Guid id, params string[] includeEntities)
        {
            try
            {
                var obj = await _unitOfWork.Categories.GetByIdAsync(id, includeEntities).ConfigureAwait(false);
                if (obj == null)
                    return null;
                return ToCategoryDto(obj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public async Task<CategoryDto> CreateCategoryAsync(CategoryDto dto)
        {
            var newCategory = new Category
            {
                CategoryName = dto.CategoryName,
                Group = dto.Group,
                Group2 = dto.Group2,
                ConsumptionBehavior = dto.ConsumptionBehavior,
            };

            try
            {
                var createdCategory = await _unitOfWork.Categories.AddAsync(newCategory).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToCategoryDto(createdCategory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public async Task<CategoryDto> UpdateCategoryAsync(CategoryDto dto)
        {
            var categoryEntity = await _unitOfWork.Categories.GetByIdAsync(dto.CategoryId).ConfigureAwait(false) ?? throw new KeyNotFoundException("Category not found.");
            categoryEntity.CategoryName = dto.CategoryName;
            categoryEntity.Group = dto.Group;
            categoryEntity.Group2 = dto.Group2;
            categoryEntity.ConsumptionBehavior = dto.ConsumptionBehavior;

            try
            {
                var updatedCategory = await _unitOfWork.Categories.UpdateAsync(categoryEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToCategoryDto(updatedCategory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public async Task<CategoryDto> DeleteCategoryAsync(Guid id)
        {
            var categoryEntity = await _unitOfWork.Categories.GetByIdAsync(id).ConfigureAwait(false);
            if (categoryEntity == null)
                return null;

            try
            {
                var deletedCategory = await _unitOfWork.Categories.DeleteAsync(id).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToCategoryDto(new Category());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }
    }
}
