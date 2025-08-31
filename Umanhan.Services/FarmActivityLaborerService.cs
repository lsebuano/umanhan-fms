using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class FarmActivityLaborerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<FarmActivityLaborerService> _logger;

        private static List<FarmActivityLaborerDto> ToFarmActivityLaborerDto(IEnumerable<FarmActivityLaborer> farmActivityLaborers)
        {
            return [.. farmActivityLaborers.Select(x => new FarmActivityLaborerDto
            {
                LaborActivityId = x.Id,
                Rate = x.Rate,
                LaborerId = x.LaborerId,
                PaymentTypeId = x.PaymentTypeId,
                QuantityWorked = x.QuantityWorked,
                TotalPayment = x.TotalPayment,
                ActivityId = x.ActivityId,
                PaymentType = x.PaymentType?.PaymentTypeName,
                LaborName = x.Laborer?.Name,
                Timestamp = x.Timestamp
            })];
        }

        private static FarmActivityLaborerDto ToFarmActivityLaborerDto(FarmActivityLaborer expenseType)
        {
            return new FarmActivityLaborerDto
            {
                LaborActivityId = expenseType.Id,
                Rate = expenseType.Rate,
                LaborerId = expenseType.LaborerId,
                PaymentTypeId = expenseType.PaymentTypeId,
                QuantityWorked = expenseType.QuantityWorked,
                TotalPayment = expenseType.TotalPayment,
                ActivityId = expenseType.ActivityId,
                PaymentType = expenseType.PaymentType?.PaymentTypeName,
                LaborName = expenseType.Laborer?.Name,
                Timestamp = expenseType.Timestamp
            };
        }

        public FarmActivityLaborerService(IUnitOfWork unitOfWork, 
            IUserContextService userContext,
            ILogger<FarmActivityLaborerService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        public async Task<IEnumerable<FarmActivityLaborerDto>> GetFarmActivityLaborersAsync(params string[] includeEntities)
        {
            var list = await _unitOfWork.FarmActivityLaborers.GetAllAsync(includeEntities).ConfigureAwait(false);
            return ToFarmActivityLaborerDto(list);
        }

        public async Task<IEnumerable<FarmActivityLaborerDto>> GetFarmActivityLaborersAsync(Guid farmId, params string[] includeEntities)
        {
            var list = await _unitOfWork.FarmActivityLaborers.GetFarmActivityLaborersAsync(farmId, includeEntities).ConfigureAwait(false);
            return ToFarmActivityLaborerDto(list);
        }

        public async Task<IEnumerable<FarmActivityLaborerDto>> GetFarmActivityLaborersByActivityAsync(Guid activityId, params string[] includeEntities)
        {
            var list = await _unitOfWork.FarmActivityLaborers.GetFarmActivityLaborersByActivityAsync(activityId, includeEntities).ConfigureAwait(false);
            return ToFarmActivityLaborerDto(list);
        }

        public async Task<FarmActivityLaborerDto> GetFarmActivityLaborerByIdAsync(Guid id, params string[] includeEntities)
        {
            var obj = await _unitOfWork.FarmActivityLaborers.GetByIdAsync(id, includeEntities).ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToFarmActivityLaborerDto(obj);
        }

        public async Task<FarmActivityLaborerDto> CreateFarmActivityLaborerAsync(FarmActivityLaborerDto expenseType)
        {
            expenseType.Recompute();
            var newFarmActivityLaborer = new FarmActivityLaborer
            {
                Rate = expenseType.Rate,
                ActivityId = expenseType.ActivityId,
                LaborerId = expenseType.LaborerId,
                PaymentTypeId = expenseType.PaymentTypeId,
                QuantityWorked = expenseType.QuantityWorked,
                TotalPayment = expenseType.TotalPayment,
            };

            try
            {
                var createdFarmActivityLaborer = await _unitOfWork.FarmActivityLaborers.AddAsync(newFarmActivityLaborer).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToFarmActivityLaborerDto(createdFarmActivityLaborer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating FarmActivityLaborer: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<FarmActivityLaborerDto> UpdateFarmActivityLaborerAsync(FarmActivityLaborerDto expenseType)
        {
            var expenseTypeEntity = await _unitOfWork.FarmActivityLaborers.GetByIdAsync(expenseType.LaborActivityId).ConfigureAwait(false) ?? throw new KeyNotFoundException("FarmActivityLaborer not found.");
            expenseTypeEntity.Rate = expenseType.Rate;
            expenseTypeEntity.ActivityId = expenseType.ActivityId;
            expenseTypeEntity.LaborerId = expenseType.LaborerId;
            expenseTypeEntity.PaymentTypeId = expenseType.PaymentTypeId;
            expenseTypeEntity.QuantityWorked = expenseType.QuantityWorked;

            expenseType.Recompute();
            expenseTypeEntity.TotalPayment = expenseType.TotalPayment;

            try
            {
                var updatedFarmActivityLaborer = await _unitOfWork.FarmActivityLaborers.UpdateAsync(expenseTypeEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToFarmActivityLaborerDto(updatedFarmActivityLaborer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating FarmActivityLaborer: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<FarmActivityLaborerDto> DeleteFarmActivityLaborerAsync(Guid id)
        {
            var expenseTypeEntity = await _unitOfWork.FarmActivityLaborers.GetByIdAsync(id).ConfigureAwait(false);
            if (expenseTypeEntity == null)
                return null;

            try
            {
                var deletedFarmActivityLaborer = await _unitOfWork.FarmActivityLaborers.DeleteAsync(id).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToFarmActivityLaborerDto(new FarmActivityLaborer());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting FarmActivityLaborer: {Message}", ex.Message);
                throw;
            }
        }
    }
}
