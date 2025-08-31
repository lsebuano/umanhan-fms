using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class PaymentTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<PaymentTypeService> _logger;

        private static List<PaymentTypeDto> ToPaymentTypeDto(IEnumerable<PaymentType> paymentTypes)
        {
            return [.. paymentTypes.Select(x => new PaymentTypeDto
            {
                PaymentTypeId = x.Id,
                PaymentTypeName = x.PaymentTypeName,
            })
            .OrderBy(x => x.PaymentTypeName)];
        }

        private static PaymentTypeDto ToPaymentTypeDto(PaymentType paymentType)
        {
            return new PaymentTypeDto
            {
                PaymentTypeId = paymentType.Id,
                PaymentTypeName = paymentType.PaymentTypeName,
            };
        }

        public PaymentTypeService(IUnitOfWork unitOfWork, 
            IUserContextService userContext,
            ILogger<PaymentTypeService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        public async Task<IEnumerable<PaymentTypeDto>> GetAllPaymentTypesAsync(params string[] includeEntities)
        {
            var list = await _unitOfWork.PaymentTypes.GetAllAsync(includeEntities).ConfigureAwait(false);
            return ToPaymentTypeDto(list);
        }

        public async Task<PaymentTypeDto> GetPaymentTypeByIdAsync(Guid id, params string[] includeEntities)
        {
            var obj = await _unitOfWork.PaymentTypes.GetByIdAsync(id, includeEntities).ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToPaymentTypeDto(obj);
        }

        public async Task<PaymentTypeDto> CreatePaymentTypeAsync(PaymentTypeDto paymentType)
        {
            var newPaymentType = new PaymentType
            {
                PaymentTypeName = paymentType.PaymentTypeName,
            };

            try
            {
                var createdPaymentType = await _unitOfWork.PaymentTypes.AddAsync(newPaymentType).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToPaymentTypeDto(createdPaymentType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment type: {PaymentTypeName}", paymentType.PaymentTypeName);
                throw;
            }
        }

        public async Task<PaymentTypeDto> UpdatePaymentTypeAsync(PaymentTypeDto paymentType)
        {
            var paymentTypeEntity = await _unitOfWork.PaymentTypes.GetByIdAsync(paymentType.PaymentTypeId).ConfigureAwait(false) ?? throw new KeyNotFoundException("PaymentType not found.");
            paymentTypeEntity.PaymentTypeName = paymentType.PaymentTypeName;

            try
            {
                var updatedPaymentType = await _unitOfWork.PaymentTypes.UpdateAsync(paymentTypeEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToPaymentTypeDto(updatedPaymentType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating payment type: {PaymentTypeName}", paymentType.PaymentTypeName);
                throw;
            }
        }

        public async Task<PaymentTypeDto> DeletePaymentTypeAsync(Guid id)
        {
            var paymentTypeEntity = await _unitOfWork.PaymentTypes.GetByIdAsync(id).ConfigureAwait(false);
            if (paymentTypeEntity == null)
                return null;

            try
            {
                var deletedPaymentType = await _unitOfWork.PaymentTypes.DeleteAsync(id).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToPaymentTypeDto(new PaymentType());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting payment type: {PaymentTypeId}", id);
                throw;
            }
        }
    }
}
