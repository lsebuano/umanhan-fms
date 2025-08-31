using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class FarmGeneralExpenseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<FarmGeneralExpenseService> _logger;

        private static List<FarmGeneralExpenseDto> ToFarmGeneralExpenseDto(IEnumerable<FarmGeneralExpense> farmGeneralExpenses)
        {
            return [.. farmGeneralExpenses.Select(x => new FarmGeneralExpenseDto
            {
                Amount = x.Amount,
                Date = x.Date,
                ExpenseId = x.Id,
                ExpenseTypeId = x.ExpenseTypeId,
                FarmId = x.FarmId,
                Notes = x.Notes,
                Payee = x.Payee,
                ExpenseTypeName = x.ExpenseType?.ExpenseTypeName,
                FarmName = x.Farm?.FarmName                
            })];
        }

        private static FarmGeneralExpenseDto ToFarmGeneralExpenseDto(FarmGeneralExpense farmGeneralExpense)
        {
            return new FarmGeneralExpenseDto
            {
                Amount = farmGeneralExpense.Amount,
                Date = farmGeneralExpense.Date,
                ExpenseId = farmGeneralExpense.Id,
                ExpenseTypeId = farmGeneralExpense.ExpenseTypeId,
                FarmId = farmGeneralExpense.FarmId,
                Notes = farmGeneralExpense.Notes,
                Payee = farmGeneralExpense.Payee,
                ExpenseTypeName = farmGeneralExpense.ExpenseType?.ExpenseTypeName,
                FarmName = farmGeneralExpense.Farm?.FarmName
            };
        }

        public FarmGeneralExpenseService(IUnitOfWork unitOfWork, 
            IUserContextService userContext,
            ILogger<FarmGeneralExpenseService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        public async Task<IEnumerable<FarmGeneralExpenseDto>> GetFarmGeneralExpensesAsync(Guid farmId, params string[] includeEntities)
        {
            var list = await _unitOfWork.FarmGeneralExpenses.GetFarmGeneralExpensesAsync(farmId, includeEntities).ConfigureAwait(false);
            return ToFarmGeneralExpenseDto(list);
        }

        public async Task<IEnumerable<FarmGeneralExpenseDto>> GetFarmGeneralExpensesAsync(Guid farmId, DateTime startDate, DateTime endDate, params string[] includeEntities)
        {
            var list = await _unitOfWork.FarmGeneralExpenses.GetFarmGeneralExpensesAsync(farmId, startDate, endDate, includeEntities).ConfigureAwait(false);
            return ToFarmGeneralExpenseDto(list);
        }

        public async Task<FarmGeneralExpenseDto> GetFarmGeneralExpenseByIdAsync(Guid id, params string[] includeEntities)
        {
            var obj = await _unitOfWork.FarmGeneralExpenses.GetByIdAsync(id, includeEntities).ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToFarmGeneralExpenseDto(obj);
        }

        public async Task<FarmGeneralExpenseDto> CreateFarmGeneralExpenseAsync(FarmGeneralExpenseDto farmGeneralExpense)
        {
            var newFarmGeneralExpense = new FarmGeneralExpense
            {
                Payee = farmGeneralExpense.Payee,
                Amount = farmGeneralExpense.Amount,
                Date = farmGeneralExpense.Date,
                FarmId = farmGeneralExpense.FarmId,
                ExpenseTypeId = farmGeneralExpense.ExpenseTypeId,
                Notes = farmGeneralExpense.Notes,
            };

            try
            {
                var createdFarmGeneralExpense = await _unitOfWork.FarmGeneralExpenses.AddAsync(newFarmGeneralExpense).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToFarmGeneralExpenseDto(createdFarmGeneralExpense);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating FarmGeneralExpense");
                throw;
            }
        }

        public async Task<FarmGeneralExpenseDto> UpdateFarmGeneralExpenseAsync(FarmGeneralExpenseDto farmGeneralExpense)
        {
            var farmGeneralExpenseEntity = await _unitOfWork.FarmGeneralExpenses.GetByIdAsync(farmGeneralExpense.ExpenseId).ConfigureAwait(false) ?? throw new KeyNotFoundException("FarmGeneralExpense not found.");
            farmGeneralExpenseEntity.Payee = farmGeneralExpense.Payee;
            farmGeneralExpenseEntity.Amount = farmGeneralExpense.Amount;
            farmGeneralExpenseEntity.Date = farmGeneralExpense.Date;
            farmGeneralExpenseEntity.FarmId = farmGeneralExpense.FarmId;
            farmGeneralExpenseEntity.ExpenseTypeId = farmGeneralExpense.ExpenseTypeId;
            farmGeneralExpenseEntity.Notes = farmGeneralExpense.Notes;

            try
            {
                var updatedFarmGeneralExpense = await _unitOfWork.FarmGeneralExpenses.UpdateAsync(farmGeneralExpenseEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToFarmGeneralExpenseDto(updatedFarmGeneralExpense);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating FarmGeneralExpense: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<FarmGeneralExpenseDto> DeleteFarmGeneralExpenseAsync(Guid id)
        {
            var farmGeneralExpenseEntity = await _unitOfWork.FarmGeneralExpenses.GetByIdAsync(id).ConfigureAwait(false);
            if (farmGeneralExpenseEntity == null)
                return null;

            try
            {
                var deletedFarmGeneralExpense = await _unitOfWork.FarmGeneralExpenses.DeleteAsync(id).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToFarmGeneralExpenseDto(new FarmGeneralExpense());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting FarmGeneralExpense: {Message}", ex.Message);
                throw;
            }
        }
    }
}
