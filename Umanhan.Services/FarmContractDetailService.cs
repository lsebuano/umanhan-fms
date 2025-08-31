using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Net.NetworkInformation;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Models.Models;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class FarmContractDetailService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<FarmContractDetailService> _logger;
        private readonly FarmContractService _farmContractService;
        private readonly FarmContractSaleService _farmContractSaleService;

        private static readonly IReadOnlyDictionary<ContractStatus, ContractStatus[]> _allowedTransitions =
        new Dictionary<ContractStatus, ContractStatus[]>
        {
            [ContractStatus.NEW] = [ContractStatus.PICKUP_CONFIRMED, ContractStatus.CANCELLED, ContractStatus.PAID],
            [ContractStatus.PARTIALLY_PAID] = [ContractStatus.PAID, ContractStatus.CANCELLED],
            [ContractStatus.PAID] = [],
            [ContractStatus.PICKUP_CONFIRMED] = [ContractStatus.PAID, ContractStatus.CANCELLED],
            [ContractStatus.CANCELLED] = [],
        };

        private static List<FarmContractDetailDto> ToFarmContractDetailDto(IEnumerable<FarmContractDetail> farmContractDetails)
        {
            return [.. farmContractDetails.Select(x => new FarmContractDetailDto
            {
                ContractStatus2 = x.Contract?.Status,
                ContractDetailId = x.Id,
                ContractId = x.ContractId,
                ProductId = x.ProductId,
                ProductTypeId = x.ProductTypeId,
                ContractedQuantity = x.ContractedQuantity,
                DeliveredQuantity = x.DeliveredQuantity,
                ContractedUnitPrice = x.ContractedUnitPrice,
                UnitId = x.UnitId,
                Status = x.Status,
                ProductType = x.ProductType?.ProductTypeName,
                Unit = x.Unit?.UnitName,
                HarvestDate = x.HarvestDate?.ToDateTime(TimeOnly.MinValue),
                PickupDate = x.PickupDate?.ToDateTime(TimeOnly.MinValue),
                PickupConfirmed = x.PickupConfirmed,
                PaidDate = x.PaidDate?.ToDateTime(TimeOnly.MinValue),
                IsRecovered = x.IsRecovered,
                PricingProfileId = x.PricingProfileId,
            })];
        }

        private static FarmContractDetailDto ToFarmContractDetailDto(FarmContractDetail farmContractDetail)
        {
            return new FarmContractDetailDto
            {
                ContractStatus2 = farmContractDetail.Contract?.Status,
                ContractDetailId = farmContractDetail.Id,
                ContractId = farmContractDetail.ContractId,
                ProductId = farmContractDetail.ProductId,
                ProductTypeId = farmContractDetail.ProductTypeId,
                ContractedQuantity = farmContractDetail.ContractedQuantity,
                DeliveredQuantity = farmContractDetail.DeliveredQuantity,
                ContractedUnitPrice = farmContractDetail.ContractedUnitPrice,
                UnitId = farmContractDetail.UnitId,
                Status = farmContractDetail.Status,
                ProductType = farmContractDetail.ProductType?.ProductTypeName,
                Unit = farmContractDetail.Unit?.UnitName,
                HarvestDate = farmContractDetail.HarvestDate?.ToDateTime(TimeOnly.MinValue),
                PickupDate = farmContractDetail.PickupDate?.ToDateTime(TimeOnly.MinValue),
                PickupConfirmed = farmContractDetail.PickupConfirmed,
                PaidDate = farmContractDetail.PaidDate?.ToDateTime(TimeOnly.MinValue),
                IsRecovered = farmContractDetail.IsRecovered,
                PricingProfileId = farmContractDetail.PricingProfileId,
            };
        }

        private string ChangeStatus(FarmContractDetail entity, ContractStatus newStatus)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null.");

            _ = Enum.TryParse(entity.Status, out ContractStatus current);
            if (current == newStatus)
                return current.ToString();

            if (!_allowedTransitions.TryGetValue(current, out var allowed))
                throw new InvalidOperationException($"No transitions defined from status {current}.");

            if (!allowed.Contains(newStatus))
                throw new InvalidOperationException($"Cannot change status from {current} to {newStatus}.");

            return newStatus.ToString();
        }

        public FarmContractDetailService(IUnitOfWork unitOfWork, 
            IUserContextService userContext, 
            FarmContractService farmContractService, 
            FarmContractSaleService farmContractSaleService,
            ILogger<FarmContractDetailService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._farmContractService = farmContractService;
            this._farmContractSaleService = farmContractSaleService;
            this._logger = logger;
        }

        public async Task<object?> GetFarmContractDetailsAsync(Guid contractId, params string[] includeEntities)
        {
            var list = await _unitOfWork.FarmContractDetails.GetFarmContractDetailsAsync(contractId, includeEntities).ConfigureAwait(false);
            return ToFarmContractDetailDto(list);
        }

        public async Task<FarmContractDetailDto> GetFarmContractDetailByIdAsync(Guid id, params string[] includeEntities)
        {
            var obj = await _unitOfWork.FarmContractDetails.GetByIdAsync(id, includeEntities).ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToFarmContractDetailDto(obj);
        }

        public async Task<FarmContractDetailDto> CreateFarmContractDetailAsync(FarmContractDetailDto farmContractDetail)
        {
            var newFarmContractDetail = new FarmContractDetail
            {
                ContractId = farmContractDetail.ContractId,
                ProductId = farmContractDetail.ProductId,
                ProductTypeId = farmContractDetail.ProductTypeId,
                ContractedQuantity = farmContractDetail.ContractedQuantity,
                DeliveredQuantity = farmContractDetail.DeliveredQuantity,
                ContractedUnitPrice = farmContractDetail.ContractedUnitPrice,
                UnitId = farmContractDetail.UnitId,
                HarvestDate = farmContractDetail.HarvestDate == null ? null : DateOnly.FromDateTime(farmContractDetail.HarvestDate.Value),
                PickupDate = farmContractDetail.PickupDate == null ? null : DateOnly.FromDateTime(farmContractDetail.PickupDate.Value),
                PickupConfirmed = farmContractDetail.PickupConfirmed,
                TotalAmount = farmContractDetail.TotalAmount,
                Status = ContractStatus.NEW.ToString(),
                PricingProfileId = farmContractDetail.PricingProfileId,
            };

            try
            {
                var createdFarmContractDetail = await _unitOfWork.FarmContractDetails.AddAsync(newFarmContractDetail).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                var farmContract = await _farmContractService.UpdateFarmContractStatusAsync(createdFarmContractDetail.ContractId).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToFarmContractDetailDto(createdFarmContractDetail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Farm Contract Detail");
                throw;
            }
        }

        public async Task<FarmContractDetailDto> UpdateFarmContractDetailAsync(FarmContractDetailDto farmContractDetail)
        {
            try
            {
                var farmContractDetailEntity = await _unitOfWork.FarmContractDetails.GetByIdAsync(farmContractDetail.ContractDetailId).ConfigureAwait(false) ?? throw new KeyNotFoundException("Farm Contract Detail not found.");
                farmContractDetailEntity.ProductId = farmContractDetail.ProductId;
                farmContractDetailEntity.ProductTypeId = farmContractDetail.ProductTypeId;
                farmContractDetailEntity.ContractedQuantity = farmContractDetail.ContractedQuantity;
                farmContractDetailEntity.DeliveredQuantity = farmContractDetail.DeliveredQuantity;
                farmContractDetailEntity.ContractedUnitPrice = farmContractDetail.ContractedUnitPrice;
                farmContractDetailEntity.UnitId = farmContractDetail.UnitId;
                farmContractDetailEntity.ContractId = farmContractDetail.ContractId;
                farmContractDetailEntity.HarvestDate = farmContractDetail.HarvestDate == null ? null : DateOnly.FromDateTime(farmContractDetail.HarvestDate.Value);
                farmContractDetailEntity.PickupDate = farmContractDetail.HarvestDate == null ? null : DateOnly.FromDateTime(farmContractDetail.PickupDate.Value);
                farmContractDetailEntity.PickupConfirmed = farmContractDetail.PickupConfirmed;
                farmContractDetailEntity.TotalAmount = farmContractDetail.TotalAmount;
                farmContractDetailEntity.PricingProfileId = farmContractDetail.PricingProfileId;

                // status should not be updated directly
                // farmContractDetailEntity.Status = farmContractDetail.Status;

                var updatedFarmContractDetail = await _unitOfWork.FarmContractDetails.UpdateAsync(farmContractDetailEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToFarmContractDetailDto(updatedFarmContractDetail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Farm Contract Detail");
                throw;
            }
        }

        public async Task<FarmContractDetailDto> DeleteFarmContractDetailAsync(Guid id)
        {
            // only NEW contracts can be deleted
            var farmContractDetailEntity = await _unitOfWork.FarmContractDetails.GetByIdAsync(id).ConfigureAwait(false);
            if (farmContractDetailEntity == null)
                return null;

            if (farmContractDetailEntity.Status != ContractStatus.NEW.ToString())
                throw new InvalidOperationException("Only NEW contracts can be deleted.");

            try
            {
                var deletedFarmContractDetail = await _unitOfWork.FarmContractDetails.DeleteAsync(id).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                var farmContract = await _farmContractService.UpdateFarmContractStatusAsync(farmContractDetailEntity.ContractId).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToFarmContractDetailDto(new FarmContractDetail());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Farm Contract Detail");
                throw;
            }
        }

        public async Task<FarmContractDetailDto> ConfirmPickupAsync(Guid id)
        {
            // only NEW contracts can be confirmed for pickup
            var farmContractDetailEntity = await _unitOfWork.FarmContractDetails.GetByIdAsync(id).ConfigureAwait(false) ?? throw new KeyNotFoundException("Farm Contract Detail not found.");
            farmContractDetailEntity.PickupConfirmed = true;
            farmContractDetailEntity.Status = this.ChangeStatus(farmContractDetailEntity, ContractStatus.PICKUP_CONFIRMED);

            try
            {
                var updatedFarmContractDetail = await _unitOfWork.FarmContractDetails.UpdateAsync(farmContractDetailEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                var farmContract = await _farmContractService.UpdateFarmContractStatusAsync(farmContractDetailEntity.ContractId).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                updatedFarmContractDetail.Contract = farmContract;
                return ToFarmContractDetailDto(updatedFarmContractDetail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming pickup for Farm Contract Detail");
                throw;
            }
        }

        public async Task<FarmContractDetailDto> CancelTransactionAsync(Guid id)
        {
            // only NEW or PICKUP_CONFIRMED contracts can be cancelled
            var farmContractDetailEntity = await _unitOfWork.FarmContractDetails.GetByIdAsync(id).ConfigureAwait(false) ?? throw new KeyNotFoundException("Farm Contract Detail not found.");
            farmContractDetailEntity.Status = this.ChangeStatus(farmContractDetailEntity, ContractStatus.CANCELLED);
            farmContractDetailEntity.PickupConfirmed = false;

            try
            {
                var updatedFarmContractDetail = await _unitOfWork.FarmContractDetails.UpdateAsync(farmContractDetailEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                var farmContract = await _farmContractService.UpdateFarmContractStatusAsync(farmContractDetailEntity.ContractId).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                updatedFarmContractDetail.Contract = farmContract;
                return ToFarmContractDetailDto(updatedFarmContractDetail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling transaction for Farm Contract Detail");
                throw;
            }
        }

        public async Task<FarmContractDetailDto> MarkTransactionAsPaidAsync(Guid id)
        {
            var farmContractDetailEntity = await _unitOfWork.FarmContractDetails.GetByIdAsync(id, "Contract.Customer", "Contract.Farm", "Unit", "ProductType").ConfigureAwait(false) ?? throw new KeyNotFoundException("Farm Contract Detail not found.");
            farmContractDetailEntity.Status = this.ChangeStatus(farmContractDetailEntity, ContractStatus.PAID);
            farmContractDetailEntity.PickupConfirmed = true;

            var paidDate = DateOnly.FromDateTime(DateTime.Now.ToLocalTime());
            farmContractDetailEntity.PaidDate = paidDate;

            try
            {
                var updatedFarmContractDetail = await _unitOfWork.FarmContractDetails.UpdateAsync(farmContractDetailEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                var farmContract = await _farmContractService.UpdateFarmContractStatusAsync(farmContractDetailEntity.ContractId).ConfigureAwait(false);

                var productLookup = await _unitOfWork.ProductLookup.BuildProductLookupAsync().ConfigureAwait(false);
                var saleEntity = new FarmContractSale
                {
                    ContractDetailId = farmContractDetailEntity.Id,
                    Date = paidDate,
                    UnitPrice = farmContractDetailEntity.ContractedUnitPrice,
                    Quantity = farmContractDetailEntity.ContractedQuantity,
                    TotalAmount = farmContractDetailEntity.TotalAmount,
                    UnitId = farmContractDetailEntity.UnitId,
                    ProductId = farmContractDetailEntity.ProductId,
                    ProductTypeId = farmContractDetailEntity.ProductTypeId,
                    CustomerId = farmContractDetailEntity.Contract.CustomerId,
                    ProductName = productLookup[new ProductKey(farmContractDetailEntity.ProductId, farmContractDetailEntity.ProductTypeId)].ProductName,
                    ProductVariety = productLookup[new ProductKey(farmContractDetailEntity.ProductId, farmContractDetailEntity.ProductTypeId)].Variety,
                    ProductTypeName = farmContractDetailEntity.ProductType.ProductTypeName,
                    CustomerName = farmContractDetailEntity.Contract.Customer.CustomerName,
                    UnitName = farmContractDetailEntity.Unit.UnitName,
                    Notes = $"Sales from contract settlement. Contract ID: {farmContract?.Id}",
                };
                await _unitOfWork.FarmContractSales.AddAsync(saleEntity).ConfigureAwait(false);

                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                updatedFarmContractDetail.Contract = farmContract;
                return ToFarmContractDetailDto(updatedFarmContractDetail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking transaction as paid for Farm Contract Detail");
                throw;
            }
        }

        public async Task<FarmContractDetailDto> SetHarvestDateAsync(Guid id, DateTime date)
        {
            var farmContractDetailEntity = await _unitOfWork.FarmContractDetails.GetByIdAsync(id).ConfigureAwait(false) ?? throw new KeyNotFoundException("Farm Contract Detail not found.");

            if (date > farmContractDetailEntity.PickupDate?.ToDateTime(TimeOnly.MinValue))
                throw new InvalidOperationException("Harvest date must be equal or earlier than the pickup date.");

            farmContractDetailEntity.HarvestDate = DateOnly.FromDateTime(date);

            try
            {
                var updatedFarmContractDetail = await _unitOfWork.FarmContractDetails.UpdateAsync(farmContractDetailEntity).ConfigureAwait(false);

                // update the yield table as well                
                var productDetail = await _unitOfWork.ProductLookup.GetProductByIdAsync(farmContractDetailEntity.ProductTypeId, farmContractDetailEntity.ProductId).ConfigureAwait(false);
                if(productDetail == null)
                    throw new KeyNotFoundException("Product not found.");

                var zoneId = productDetail.ZoneId;
                if(zoneId == null)
                    throw new InvalidOperationException("Product does not have an associated zone.");

                await _unitOfWork.FarmZoneYields.AddAsync(new FarmZoneYield
                {
                    ProductId = farmContractDetailEntity.ProductId,
                    ProductTypeId = farmContractDetailEntity.ProductTypeId,
                    UnitId = farmContractDetailEntity.UnitId,
                    ExpectedYield = farmContractDetailEntity.ContractedQuantity,
                    ActualYield = farmContractDetailEntity.ContractedQuantity, // assuming actual yield is the same as contracted quantity
                    ContractDetailId = farmContractDetailEntity.Id,
                    ZoneId = zoneId.Value,
                    HarvestDate = farmContractDetailEntity.HarvestDate.Value,
                    ForecastedYield = 0,
                    FarmId = farmContractDetailEntity.Contract.FarmId,
                }).ConfigureAwait(false);

                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);
                return ToFarmContractDetailDto(updatedFarmContractDetail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting harvest date for Farm Contract Detail");
                throw;
            }
        }

        public async Task<FarmContractDetailDto> SetPickupDateAsync(Guid id, DateTime date)
        {
            var farmContractDetailEntity = await _unitOfWork.FarmContractDetails.GetByIdAsync(id).ConfigureAwait(false) ?? throw new KeyNotFoundException("Farm Contract Detail not found.");

            var puDate = farmContractDetailEntity.PickupDate;
            if (puDate.HasValue && DateOnly.FromDateTime(date) == puDate.Value)
                throw new InvalidOperationException("Select a different pickup date.");

            if (date < farmContractDetailEntity.HarvestDate?.ToDateTime(TimeOnly.MinValue))
                throw new InvalidOperationException("Pickup date must equal or later than the harvest date.");

            farmContractDetailEntity.PickupDate = DateOnly.FromDateTime(date);

            try
            {
                var updatedFarmContractDetail = await _unitOfWork.FarmContractDetails.UpdateAsync(farmContractDetailEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);
                return ToFarmContractDetailDto(updatedFarmContractDetail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting pickup date for Farm Contract Detail");
                throw;
            }
        }

        public async Task<FarmContractDetailDto> RecoverHarvestAsync(Guid id)
        {
            var farmContractDetailEntity = await _unitOfWork.FarmContractDetails.GetByIdAsync(id, "Contract").ConfigureAwait(false) ?? throw new KeyNotFoundException("Farm Contract Detail not found.");            

            try
            {
                // only CANCELLED contracts can be recovered
                if (farmContractDetailEntity.Status != ContractStatus.CANCELLED.ToString() &&
                    farmContractDetailEntity.HarvestDate.HasValue)
                    throw new InvalidOperationException("Only CANCELLED contracts can be recovered.");

                farmContractDetailEntity.IsRecovered = true;
                farmContractDetailEntity.Status = ContractStatus.RECOVERED.ToString();

                var updatedFarmContractDetail = await _unitOfWork.FarmContractDetails.UpdateAsync(farmContractDetailEntity).ConfigureAwait(false);

                //decimal initialQuantity = 0;
                //var list = await _unitOfWork.FarmProduceInventories.GetFarmProduceInventoriesAsync(farmContractDetailEntity.Contract.FarmId).ConfigureAwait(false);
                //initialQuantity = 
                await _unitOfWork.FarmProduceInventories.AddAsync(new FarmProduceInventory
                {
                    ProductId = farmContractDetailEntity.ProductId,
                    ProductTypeId = farmContractDetailEntity.ProductTypeId,
                    UnitId = farmContractDetailEntity.UnitId,
                    UnitPrice = farmContractDetailEntity.ContractedUnitPrice,
                    Date = DateOnly.FromDateTime(DateTime.Now.ToLocalTime()),
                    Notes = $"Recovered from contract detail ID: {farmContractDetailEntity.Id}",
                    FarmId = farmContractDetailEntity.Contract.FarmId,
                    CurrentQuantity = farmContractDetailEntity.ContractedQuantity,
                    InitialQuantity = farmContractDetailEntity.ContractedQuantity,
                }).ConfigureAwait(false);

                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToFarmContractDetailDto(updatedFarmContractDetail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recovering harvest for Farm Contract Detail");
                throw;
            }
        }
    }
}
