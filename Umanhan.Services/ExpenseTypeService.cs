using Microsoft.Extensions.Logging;
using Umanhan.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class ExpenseTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<ExpenseTypeService> _logger;

        private static List<ExpenseTypeDto> ToExpenseTypeDto(IEnumerable<ExpenseType> expenseTypes)
        {
            return [.. expenseTypes.Select(x => new ExpenseTypeDto
            {
                TypeId = x.Id,
                ExpenseTypeName = x.ExpenseTypeName,
            })
            .OrderBy(x => x.ExpenseTypeName)];
        }

        private static ExpenseTypeDto ToExpenseTypeDto(ExpenseType expenseType)
        {
            return new ExpenseTypeDto
            {
                TypeId = expenseType.Id,
                ExpenseTypeName = expenseType.ExpenseTypeName,
            };
        }

        public ExpenseTypeService(IUnitOfWork unitOfWork, 
            IUserContextService userContext,
            ILogger<ExpenseTypeService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        public async Task<IEnumerable<ExpenseTypeDto>> GetAllExpenseTypesAsync(params string[] includeEntities)
        {
            var list = await _unitOfWork.ExpenseTypes.GetAllAsync(includeEntities).ConfigureAwait(false);
            return ToExpenseTypeDto(list);
        }

        public async Task<ExpenseTypeDto> GetExpenseTypeByIdAsync(Guid id, params string[] includeEntities)
        {
            var obj = await _unitOfWork.ExpenseTypes.GetByIdAsync(id, includeEntities).ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToExpenseTypeDto(obj);
        }

        public async Task<ExpenseTypeDto> CreateExpenseTypeAsync(ExpenseTypeDto expenseType)
        {
            var newExpenseType = new ExpenseType
            {
                ExpenseTypeName = expenseType.ExpenseTypeName,
            };

            try
            {
                var createdExpenseType = await _unitOfWork.ExpenseTypes.AddAsync(newExpenseType).ConfigureAwait(false);
                return ToExpenseTypeDto(createdExpenseType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating ExpenseType: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<ExpenseTypeDto> UpdateExpenseTypeAsync(ExpenseTypeDto expenseType)
        {
            var expenseTypeEntity = await _unitOfWork.ExpenseTypes.GetByIdAsync(expenseType.TypeId).ConfigureAwait(false) ?? throw new KeyNotFoundException("ExpenseType not found.");
            expenseTypeEntity.ExpenseTypeName = expenseType.ExpenseTypeName;

            try
            {
                var updatedExpenseType = await _unitOfWork.ExpenseTypes.UpdateAsync(expenseTypeEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToExpenseTypeDto(updatedExpenseType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating ExpenseType: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<ExpenseTypeDto> DeleteExpenseTypeAsync(Guid id)
        {
            var expenseTypeEntity = await _unitOfWork.ExpenseTypes.GetByIdAsync(id).ConfigureAwait(false);
            if (expenseTypeEntity == null)
                return null;

            try
            {
                var deletedExpenseType = await _unitOfWork.ExpenseTypes.DeleteAsync(id).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToExpenseTypeDto(new ExpenseType());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting ExpenseType: {Message}", ex.Message);
                throw;
            }
        }
    }
}
