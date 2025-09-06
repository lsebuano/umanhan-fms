using Microsoft.Extensions.Logging;
using Umanhan.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class FarmTransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<FarmTransactionService> _logger;

        private async Task<List<FarmTransactionDto>> ToFarmTransactionDto(IEnumerable<FarmTransaction> farmTransactions)
        {
            var productLookup = await _unitOfWork.ProductLookup.BuildProductLookupAsync().ConfigureAwait(false);
            return [.. farmTransactions.Select(x => new FarmTransactionDto
            {
                TransactionId = x.Id,
                BuyerId = x.BuyerId,
                BuyerName = x.BuyerName,
                PaymentType = x.PaymentType,
                Date = x.Date.ToDateTime(TimeOnly.MinValue),
                ProduceInventoryId = x.ProduceInventoryId,
                Quantity = x.Quantity,
                Notes = x.Notes,
                ProductId = x.ProductId,
                ProductTypeId = x.ProductTypeId,
                TotalAmount = x.TotalAmount,
                TransactionTypeId = x.TransactionTypeId,
                UnitId = x.UnitId,
                UnitPrice = x.UnitPrice,
                ProductType = x.ProductType?.ProductTypeName,
                TransactionType = x.TransactionType?.TransactionTypeName,
                Unit = x.Unit?.UnitName,
                FarmId = x.FarmId,
                Product = productLookup[new ProductKey(x.ProductId, x.ProductTypeId)].ProductName,
                ProductVariety = productLookup[new ProductKey(x.ProductId, x.ProductTypeId)].Variety,
            })];
        }

        private async Task<FarmTransactionDto> ToFarmTransactionDto(FarmTransaction farmTransaction)
        {
            var productLookup = await _unitOfWork.ProductLookup.BuildProductLookupAsync().ConfigureAwait(false);
            return new FarmTransactionDto
            {
                TransactionId = farmTransaction.Id,
                BuyerId = farmTransaction.BuyerId,
                BuyerName = farmTransaction.BuyerName,
                PaymentType = farmTransaction.PaymentType,
                Date = farmTransaction.Date.ToDateTime(TimeOnly.MinValue),
                ProduceInventoryId = farmTransaction.ProduceInventoryId,
                Quantity = farmTransaction.Quantity,
                Notes = farmTransaction.Notes,
                ProductId = farmTransaction.ProductId,
                ProductTypeId = farmTransaction.ProductTypeId,
                TotalAmount = farmTransaction.TotalAmount,
                TransactionTypeId = farmTransaction.TransactionTypeId,
                UnitId = farmTransaction.UnitId,
                UnitPrice = farmTransaction.UnitPrice,
                Product = productLookup[new ProductKey(farmTransaction.ProductId, farmTransaction.ProductTypeId)].ProductName,
                ProductVariety = productLookup[new ProductKey(farmTransaction.ProductId, farmTransaction.ProductTypeId)].Variety,
                ProductType = farmTransaction.ProductType?.ProductTypeName,
                TransactionType = farmTransaction.TransactionType?.TransactionTypeName,
                Unit = farmTransaction.Unit?.UnitName,
                FarmId = farmTransaction.FarmId,
            };
        }

        public FarmTransactionService(IUnitOfWork unitOfWork, 
            IUserContextService userContext,
            ILogger<FarmTransactionService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        public async Task<IEnumerable<FarmTransactionDto>> GetFarmSalesAsync(Guid farmId, params string[] includeEntities)
        {
            var list = await _unitOfWork.FarmTransactions.GetFarmTransactionsAsync(farmId, includeEntities).ConfigureAwait(false);
            return await ToFarmTransactionDto(list).ConfigureAwait(false);
        }

        public async Task<IEnumerable<FarmTransactionDto>> GetRecentFarmTransactionsAsync(Guid farmId)
        {
            var list = await _unitOfWork.FarmTransactions.GetRecentFarmTransactionsAsync(farmId).ConfigureAwait(false);
            return await ToFarmTransactionDto(list).ConfigureAwait(false);
        }

        public async Task<FarmTransactionDto> GetFarmTransactionByIdAsync(Guid id, params string[] includeEntities)
        {
            var obj = await _unitOfWork.FarmTransactions.GetByIdAsync(id, includeEntities).ConfigureAwait(false);
            if (obj == null)
                return null;
            return await ToFarmTransactionDto(obj).ConfigureAwait(false);
        }

        public async Task<FarmTransactionDto> CreateFarmTransactionAsync(FarmTransactionDto farmTransaction)
        {
            var newFarmTransaction = new FarmTransaction
            {
                FarmId = farmTransaction.FarmId,
                BuyerId = farmTransaction.BuyerId,
                BuyerName = farmTransaction.BuyerName,
                Date = DateOnly.FromDateTime(farmTransaction.Date),
                ProduceInventoryId = farmTransaction.ProduceInventoryId,
                Quantity = farmTransaction.Quantity,
                Notes = farmTransaction.Notes,
                ProductId = farmTransaction.ProductId,
                ProductTypeId = farmTransaction.ProductTypeId,
                TotalAmount = farmTransaction.Quantity * farmTransaction.UnitPrice,
                TransactionTypeId = farmTransaction.TransactionTypeId,
                UnitId = farmTransaction.UnitId,
                UnitPrice = farmTransaction.UnitPrice,
            };

            try
            {
                var createdFarmTransaction = await _unitOfWork.FarmTransactions.AddAsync(newFarmTransaction).ConfigureAwait(false);
                if (createdFarmTransaction == null)
                    throw new InvalidOperationException("Failed to create farm transaction.");

                // update the inventory
                var fpiObj = await _unitOfWork.FarmProduceInventories.GetByIdAsync(farmTransaction.ProduceInventoryId).ConfigureAwait(false) ?? throw new InvalidOperationException("Missing inventory data.");

                // make sure the quantity is not negative
                if (fpiObj.CurrentQuantity < farmTransaction.Quantity)
                    throw new InvalidOperationException("Insufficient inventory quantity.");

                // update the inventory quantity
                fpiObj.CurrentQuantity -= farmTransaction.Quantity;
                await _unitOfWork.FarmProduceInventories.UpdateAsync(fpiObj).ConfigureAwait(false);

                // commit changes
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return await ToFarmTransactionDto(createdFarmTransaction).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Farm Transaction: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<FarmTransactionDto> UpdateFarmTransactionAsync(FarmTransactionDto farmTransaction)
        {
            var farmTransactionEntity = await _unitOfWork.FarmTransactions.GetByIdAsync(farmTransaction.TransactionId).ConfigureAwait(false) ?? throw new KeyNotFoundException("FarmTransaction not found.");
            farmTransactionEntity.BuyerId = farmTransaction.BuyerId;
            farmTransactionEntity.BuyerName = farmTransaction.BuyerName;
            farmTransactionEntity.Date = DateOnly.FromDateTime(farmTransaction.Date);
            farmTransactionEntity.ProduceInventoryId = farmTransaction.ProduceInventoryId;
            farmTransactionEntity.Quantity = farmTransaction.Quantity;
            farmTransactionEntity.Notes = farmTransaction.Notes;
            farmTransactionEntity.ProductId = farmTransaction.ProductId;
            farmTransactionEntity.ProductTypeId = farmTransaction.ProductTypeId;
            farmTransactionEntity.TotalAmount = farmTransaction.Quantity * farmTransaction.UnitPrice;
            farmTransactionEntity.TransactionTypeId = farmTransaction.TransactionTypeId;
            farmTransactionEntity.UnitId = farmTransaction.UnitId;
            farmTransactionEntity.UnitPrice = farmTransaction.UnitPrice;
            farmTransactionEntity.FarmId = farmTransaction.FarmId;

            try
            {
                var updatedFarmTransaction = await _unitOfWork.FarmTransactions.UpdateAsync(farmTransactionEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return await ToFarmTransactionDto(updatedFarmTransaction).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Farm Transaction: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<FarmTransactionDto> DeleteFarmTransactionAsync(Guid id)
        {
            var farmTransactionEntity = await _unitOfWork.FarmTransactions.GetByIdAsync(id).ConfigureAwait(false);
            if (farmTransactionEntity == null)
                return null;

            try
            {
                var deletedFarmTransaction = await _unitOfWork.FarmTransactions.DeleteAsync(id).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return await ToFarmTransactionDto(new FarmTransaction()).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Farm Transaction: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<FarmTransactionDto> CancelTransactionAsync(Guid id)
        {
            //// only NEW or PICKUP_CONFIRMED contracts can be cancelled
            //var farmTransactionEntity = await _unitOfWork.FarmContractDetails.GetByIdAsync(id).ConfigureAwait(false) ?? throw new KeyNotFoundException("Farm Contract Detail not found.");
            //farmTransactionEntity.Status = this.ChangeStatus(farmTransactionEntity, ContractStatus.CANCELLED);
            //farmTransactionEntity.PickupConfirmed = false;

            //try
            //{
            //    var updatedFarmContractDetail = await _unitOfWork.FarmContractDetails.UpdateAsync(farmTransactionEntity).ConfigureAwait(false);
            //    await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

            //    var farmContract = await _farmContractService.UpdateFarmContractStatusAsync(farmTransactionEntity.ContractId).ConfigureAwait(false);
            //    await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

            //    updatedFarmContractDetail.Contract = farmContract;
            //    return ToFarmContractDetailDto(updatedFarmContractDetail);
            //}
            //catch (Exception ex)
            //{
            //    throw;
            //}

            throw new NotImplementedException();
        }
    }
}
