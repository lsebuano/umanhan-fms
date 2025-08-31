using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Dtos;

namespace Umanhan.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync(params string[] includeEntities);
        Task<CategoryDto> GetCategoryByIdAsync(Guid id, params string[] includeEntities);
        Task<CategoryDto> CreateCategoryAsync(CategoryDto dto);
        Task<CategoryDto> UpdateCategoryAsync(CategoryDto dto);
        Task<CategoryDto> DeleteCategoryAsync(Guid id);
    }
}
