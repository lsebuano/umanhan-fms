using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class FarmActivityExpenseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<FarmActivityExpenseService> _logger;

        private static List<FarmActivityExpenseDto> ToFarmActivityExpenseDto(IEnumerable<FarmActivityExpense> farmActivityExpenses)
        {
            return [.. farmActivityExpenses.Select(x => new FarmActivityExpenseDto
            {
                ActivityId = x.ActivityId,
                Amount = x.Amount,
                DateUtc = x.Date,
                Description = x.Description,
                ExpenseId = x.Id,
                ExpenseTypeId = x.ExpenseTypeId,
                ExpenseTypeName = x.ExpenseType?.ExpenseTypeName,
                Task = x.Activity?.Task.TaskName,
                ProductType = x.Activity?.ProductType?.ProductTypeName,
                Supervisor = x.Activity?.Supervisor?.Name,
                ActivityStartDateTime = x.Activity?.StartDate ?? DateTime.MinValue,
                ActivityEndDateTime = x.Activity?.EndDate ?? DateTime.MinValue,
            })];
        }

        private static FarmActivityExpenseDto ToFarmActivityExpenseDto(FarmActivityExpense farmActivityExpense)
        {
            return new FarmActivityExpenseDto
            {
                ActivityId = farmActivityExpense.ActivityId,
                Amount = farmActivityExpense.Amount,
                DateUtc = farmActivityExpense.Date,
                Description = farmActivityExpense.Description,
                ExpenseId = farmActivityExpense.Id,
                ExpenseTypeId = farmActivityExpense.ExpenseTypeId,
                ExpenseTypeName = farmActivityExpense.ExpenseType?.ExpenseTypeName,
                Task = farmActivityExpense.Activity?.Task.TaskName,
                ProductType = farmActivityExpense.Activity?.ProductType?.ProductTypeName,
                Supervisor = farmActivityExpense.Activity?.Supervisor?.Name,
                ActivityStartDateTime = farmActivityExpense.Activity?.StartDate ?? DateTime.MinValue,
                ActivityEndDateTime = farmActivityExpense.Activity?.EndDate ?? DateTime.MinValue,
            };
        }

        public FarmActivityExpenseService(IUnitOfWork unitOfWork, 
            IUserContextService userContext,
            ILogger<FarmActivityExpenseService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        public async Task<IEnumerable<FarmActivityExpenseDto>> GetAllFarmActivityExpensesAsync(params string[] includeEntities)
        {
            var list = await _unitOfWork.FarmActivityExpenses.GetAllAsync(includeEntities).ConfigureAwait(false);
            return ToFarmActivityExpenseDto(list);
        }

        public async Task<IEnumerable<FarmActivityExpenseDto>> GetFarmActivityExpensesAsync(Guid farmId, params string[] includeEntities)
        {
            var list = await _unitOfWork.FarmActivityExpenses.GetFarmActivityExpensesAsync(farmId, includeEntities).ConfigureAwait(false);
            return ToFarmActivityExpenseDto(list);
        }

        public async Task<IEnumerable<FarmActivityExpenseDto>> GetFarmActivityExpensesByActivityAsync(Guid activityId, params string[] includeEntities)
        {
            var list = await _unitOfWork.FarmActivityExpenses.GetFarmActivityExpensesByActivityAsync(activityId, includeEntities).ConfigureAwait(false);
            return ToFarmActivityExpenseDto(list);
        }

        public async Task<FarmActivityExpenseDto> GetFarmActivityExpenseByIdAsync(Guid id, params string[] includeEntities)
        {
            var obj = await _unitOfWork.FarmActivityExpenses.GetByIdAsync(id, includeEntities).ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToFarmActivityExpenseDto(obj);
        }

        public async Task<FarmActivityExpenseDto> CreateFarmActivityExpenseAsync(FarmActivityExpenseDto farmActivityExpense)
        {
            var newFarmActivityExpense = new FarmActivityExpense
            {
                Amount = farmActivityExpense.Amount,
                Date = farmActivityExpense.Date,
                Description = farmActivityExpense.Description,
                ActivityId = farmActivityExpense.ActivityId,
                ExpenseTypeId = farmActivityExpense.ExpenseTypeId,
            };

            try
            {
                var createdFarmActivityExpense = await _unitOfWork.FarmActivityExpenses.AddAsync(newFarmActivityExpense).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToFarmActivityExpenseDto(createdFarmActivityExpense);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating FarmActivityExpense: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<FarmActivityExpenseDto> UpdateFarmActivityExpenseAsync(FarmActivityExpenseDto farmActivityExpense)
        {
            var farmActivityExpenseEntity = await _unitOfWork.FarmActivityExpenses.GetByIdAsync(farmActivityExpense.ExpenseId).ConfigureAwait(false) ?? throw new KeyNotFoundException("Farm Activity not found.");
            //farmActivityExpenseEntity.Id = farmActivityExpense.ExpenseId;
            farmActivityExpenseEntity.ActivityId = farmActivityExpense.ActivityId;
            farmActivityExpenseEntity.Amount = farmActivityExpense.Amount;
            farmActivityExpenseEntity.Date = farmActivityExpense.Date;
            farmActivityExpenseEntity.Description = farmActivityExpense.Description;
            farmActivityExpenseEntity.ExpenseTypeId = farmActivityExpense.ExpenseTypeId;

            try
            {
                var updatedFarmActivityExpense = await _unitOfWork.FarmActivityExpenses.UpdateAsync(farmActivityExpenseEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToFarmActivityExpenseDto(updatedFarmActivityExpense);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating FarmActivityExpense: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<FarmActivityExpenseDto> DeleteFarmActivityExpenseAsync(Guid id)
        {
            var farmActivityExpenseEntity = await _unitOfWork.FarmActivityExpenses.GetByIdAsync(id).ConfigureAwait(false);
            if (farmActivityExpenseEntity == null)
                return null;

            try
            {
                var deletedFarmActivityExpense = await _unitOfWork.FarmActivityExpenses.DeleteAsync(id).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToFarmActivityExpenseDto(new FarmActivityExpense());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting FarmActivityExpense: {Message}", ex.Message);
                throw;
            }
        }
    }
}
