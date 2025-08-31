using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class TransactionTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<TransactionTypeService> _logger;

        private static List<TransactionTypeDto> ToTransactionTypeDto(IEnumerable<TransactionType> transactionTypes)
        {
            return [.. transactionTypes.Select(x => new TransactionTypeDto
            {
                TypeId = x.Id,
                TransactionTypeName = x.TransactionTypeName,
            })
            .OrderBy(x => x.TransactionTypeName)];
        }

        private static TransactionTypeDto ToTransactionTypeDto(TransactionType transactionType)
        {
            return new TransactionTypeDto
            {
                TypeId = transactionType.Id,
                TransactionTypeName = transactionType.TransactionTypeName,
            };
        }

        public TransactionTypeService(IUnitOfWork unitOfWork, 
            IUserContextService userContext,
            ILogger<TransactionTypeService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        public async Task<IEnumerable<TransactionTypeDto>> GetAllTransactionTypesAsync(params string[] includeEntities)
        {
            var list = await _unitOfWork.TransactionTypes.GetAllAsync(includeEntities).ConfigureAwait(false);
            return ToTransactionTypeDto(list);
        }

        public async Task<TransactionTypeDto> GetTransactionTypeByIdAsync(Guid id, params string[] includeEntities)
        {
            var obj = await _unitOfWork.TransactionTypes.GetByIdAsync(id, includeEntities).ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToTransactionTypeDto(obj);
        }

        public async Task<TransactionTypeDto> CreateTransactionTypeAsync(TransactionTypeDto transactionType)
        {
            var newTransactionType = new TransactionType
            {
                TransactionTypeName = transactionType.TransactionTypeName,
            };

            try
            {
                var createdTransactionType = await _unitOfWork.TransactionTypes.AddAsync(newTransactionType).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToTransactionTypeDto(createdTransactionType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating transaction type: {TransactionTypeName}", transactionType.TransactionTypeName);
                throw;
            }
        }

        public async Task<TransactionTypeDto> UpdateTransactionTypeAsync(TransactionTypeDto transactionType)
        {
            var transactionTypeEntity = await _unitOfWork.TransactionTypes.GetByIdAsync(transactionType.TypeId).ConfigureAwait(false) ?? throw new KeyNotFoundException("TransactionType not found.");
            transactionTypeEntity.TransactionTypeName = transactionType.TransactionTypeName;

            try
            {
                var updatedTransactionType = await _unitOfWork.TransactionTypes.UpdateAsync(transactionTypeEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToTransactionTypeDto(updatedTransactionType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating transaction type: {TransactionTypeName}", transactionType.TransactionTypeName);
                throw;
            }
        }

        public async Task<TransactionTypeDto> DeleteTransactionTypeAsync(Guid id)
        {
            var transactionTypeEntity = await _unitOfWork.TransactionTypes.GetByIdAsync(id).ConfigureAwait(false);
            if (transactionTypeEntity == null)
                return null;

            try
            {
                var deletedTransactionType = await _unitOfWork.TransactionTypes.DeleteAsync(id).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToTransactionTypeDto(new TransactionType());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting transaction type: {TransactionTypeId}", id);
                throw;
            }
        }
    }
}
