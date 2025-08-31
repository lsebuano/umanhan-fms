using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Models.Models;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class FarmContractService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<FarmContractService> _logger;

        private async Task<List<FarmContractDto>> ToFarmContractDto(IEnumerable<FarmContract> farmContracts)
        {
            var productLookup = await _unitOfWork.ProductLookup.BuildProductLookupAsync().ConfigureAwait(false);

            return [.. farmContracts.Select(x => new FarmContractDto
            {
                ContractDate = x.ContractDate.ToDateTime(TimeOnly.MinValue),
                ContractId = x.Id,
                CustomerId = x.CustomerId,
                FarmId = x.FarmId,
                Status = x.Status,
                CustomerAddress = x.Customer?.Address,
                CustomerContactInfo = x.Customer?.ContactInfo,
                CustomerName = x.Customer?.CustomerName,
                CustomerTin = x.Customer?.Tin,
                FarmName = x.Farm?.FarmName,
                FarmLocation = x.Farm?.Location,
                FarmFullAddress = x.Farm?.FullAddress,
                FarmContractDetails = x.FarmContractDetails.Select(z=> new FarmContractDetailDto {
                    ContractDetailId = z.Id,
                    ContractId = z.ContractId,
                    ContractedQuantity = z.ContractedQuantity,
                    DeliveredQuantity = z.DeliveredQuantity,
                    ContractedUnitPrice = z.ContractedUnitPrice,
                    ProductId = z.ProductId,
                    Product = productLookup[new ProductKey(z.ProductId, z.ProductTypeId)].ProductName,
                    ProductTypeId = z.ProductTypeId,
                    ProductType = z.ProductType?.ProductTypeName,
                    Unit = z.Unit?.UnitName,
                    UnitId = z.UnitId,
                    Status = z.Status,
                    HarvestDate = z.HarvestDate?.ToDateTime(TimeOnly.MinValue),
                    PickupDate = z.PickupDate?.ToDateTime(TimeOnly.MinValue),
                    PickupConfirmed = z.PickupConfirmed,
                    HarvestActivityId = x.FarmActivities.FirstOrDefault(xx=>xx.ProductId == z.ProductId)?.Id ?? Guid.Empty,
                    HasHarvestActivity = x.FarmActivities.Any(xx=>xx.ProductId == z.ProductId),
                    PaidDate = z.PaidDate?.ToDateTime(TimeOnly.MinValue),
                    IsRecovered = z.IsRecovered,
                    PricingProfileId = z.PricingProfileId,
                })
                .OrderBy(x => x.HarvestDate ?? x.PickupDate)
            })
                .OrderBy(x => x.ContractDate)];
        }

        private async Task<FarmContractDto> ToFarmContractDto(FarmContract farmContract)
        {
            var productLookup = await _unitOfWork.ProductLookup.BuildProductLookupAsync().ConfigureAwait(false);

            return new FarmContractDto
            {
                ContractDate = farmContract.ContractDate.ToDateTime(TimeOnly.MinValue),
                ContractId = farmContract.Id,
                CustomerId = farmContract.CustomerId,
                FarmId = farmContract.FarmId,
                Status = farmContract.Status,
                CustomerAddress = farmContract.Customer?.Address,
                CustomerContactInfo = farmContract.Customer?.ContactInfo,
                CustomerName = farmContract.Customer?.CustomerName,
                CustomerTin = farmContract.Customer?.Tin,
                FarmName = farmContract.Farm?.FarmName,
                FarmLocation = farmContract.Farm?.Location,
                FarmFullAddress = farmContract.Farm?.FullAddress,
                FarmContractDetails = farmContract.FarmContractDetails.Select(z => new FarmContractDetailDto
                {
                    ContractDetailId = z.Id,
                    ContractId = z.ContractId,
                    ContractedQuantity = z.ContractedQuantity,
                    DeliveredQuantity = z.DeliveredQuantity,
                    ContractedUnitPrice = z.ContractedUnitPrice,
                    ProductId = z.ProductId,
                    Product = productLookup[new ProductKey(z.ProductId, z.ProductTypeId)].ProductName,
                    ProductTypeId = z.ProductTypeId,
                    ProductType = z.ProductType?.ProductTypeName,
                    Unit = z.Unit?.UnitName,
                    UnitId = z.UnitId,
                    Status = z.Status,
                    HarvestDate = z.HarvestDate?.ToDateTime(TimeOnly.MinValue),
                    PickupDate = z.PickupDate?.ToDateTime(TimeOnly.MinValue),
                    PickupConfirmed = z.PickupConfirmed,
                    HarvestActivityId = farmContract.FarmActivities.FirstOrDefault(xx => xx.ProductId == z.ProductId)?.Id ?? Guid.Empty,
                    HasHarvestActivity = farmContract.FarmActivities.Any(xx => xx.ProductId == z.ProductId),
                    PaidDate = z.PaidDate?.ToDateTime(TimeOnly.MinValue),
                    IsRecovered = z.IsRecovered,
                    PricingProfileId = z.PricingProfileId,
                })
            };
        }

        public FarmContractService(IUnitOfWork unitOfWork, 
            IUserContextService userContext,
            ILogger<FarmContractService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        public async Task<List<FarmContractDto>> GetFarmContractsAsync(Guid farmId, params string[] includeEntities)
        {
            var list = await _unitOfWork.FarmContracts.GetFarmContractsAsync(farmId, includeEntities).ConfigureAwait(false);
            return await ToFarmContractDto(list);
        }

        public async Task<List<FarmContractDto>> GetFarmContractsAsync(Guid farmId, DateTime startDate, DateTime endDate, params string[] includeEntities)
        {
            var list = await _unitOfWork.FarmContracts.GetFarmContractsAsync(farmId, startDate, endDate, includeEntities).ConfigureAwait(false);
            return await ToFarmContractDto(list);
        }

        public async Task<FarmContractDto> GetFarmContractByIdAsync(Guid id, params string[] includeEntities)
        {
            var obj = await _unitOfWork.FarmContracts.GetByIdAsync(id, includeEntities).ConfigureAwait(false);
            if (obj == null)
                return null;
            return await ToFarmContractDto(obj).ConfigureAwait(false);
        }

        public async Task<FarmContractDto> CreateFarmContractAsync(FarmContractDto farmContract)
        {
            var newFarmContract = new FarmContract
            {
                ContractDate = DateOnly.FromDateTime(farmContract.ContractDate),
                CustomerId = farmContract.CustomerId,
                FarmId = farmContract.FarmId,
                Status = ContractStatus.NEW.ToString()
            };

            try
            {
                var createdFarmContract = await _unitOfWork.FarmContracts.AddAsync(newFarmContract).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return await ToFarmContractDto(createdFarmContract).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating FarmContract");
                throw;
            }
        }

        public async Task<FarmContractDto> UpdateFarmContractAsync(FarmContractDto farmContract)
        {
            var farmContractEntity = await _unitOfWork.FarmContracts.GetByIdAsync(farmContract.ContractId).ConfigureAwait(false) ?? throw new KeyNotFoundException("FarmContract not found.");
            farmContractEntity.ContractDate = DateOnly.FromDateTime(farmContract.ContractDate);
            farmContractEntity.CustomerId = farmContract.CustomerId;
            farmContractEntity.FarmId = farmContract.FarmId;
            farmContractEntity.Status = farmContract.Status;

            try
            {
                var updatedFarmContract = await _unitOfWork.FarmContracts.UpdateAsync(farmContractEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return await ToFarmContractDto(updatedFarmContract).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating FarmContract: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<FarmContractDto> DeleteFarmContractAsync(Guid id)
        {
            var farmContractEntity = await _unitOfWork.FarmContracts.GetByIdAsync(id, "FarmContractDetails").ConfigureAwait(false);
            if (farmContractEntity == null)
                return null;

            try
            {
                // check the details first. only new details can be deleted
                var details = farmContractEntity.FarmContractDetails;
                if (details.All(d => d.Status.ToUpper() != ContractStatus.NEW.ToString()))
                {
                    throw new InvalidOperationException("Only contracts with NEW details can be deleted.");
                }

                var deletedFarmContract = await _unitOfWork.FarmContracts.DeleteAsync(id).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return await ToFarmContractDto(new FarmContract()).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting FarmContract: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<FarmContract?> UpdateFarmContractStatusAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid contract ID.", nameof(id));

            var contract = await _unitOfWork.FarmContracts.GetByIdAsync(id, "FarmContractDetails").ConfigureAwait(false);
            if (contract is null)
                return null;

            var allDetails = contract.FarmContractDetails;
            if (!allDetails.Any())
                return contract; // No details, keep current status

            // Prevent overriding terminal contract statuses
            if (contract.Status == ContractStatus.CANCELLED.ToString() ||
                contract.Status == ContractStatus.PICKUP_CONFIRMED.ToString())
            {
                return contract;
            }

            // Parse detail statuses into enum
            var detailStatuses = allDetails
                .Select(d => Enum.TryParse<ContractStatus>(d.Status, true, out var status) ? status : (ContractStatus?)null)
                .Where(s => s.HasValue)
                .Select(s => s.Value)
                .Distinct()
                .ToList();

            ContractStatus newStatus;

            if (detailStatuses.Count == 1)
            {
                newStatus = detailStatuses[0];
            }
            else if (detailStatuses.All(s => s == ContractStatus.PAID || s == ContractStatus.PICKUP_CONFIRMED))
            {
                newStatus = ContractStatus.PARTIALLY_PAID;
            }
            else if (detailStatuses.Contains(ContractStatus.PAID) || detailStatuses.Contains(ContractStatus.PICKUP_CONFIRMED))
            {
                newStatus = ContractStatus.PARTIALLY_PAID;
            }
            else if (detailStatuses.All(s => s == ContractStatus.CANCELLED))
            {
                newStatus = ContractStatus.CANCELLED;
            }
            else if (detailStatuses.All(s => s == ContractStatus.NEW))
            {
                newStatus = ContractStatus.NEW;
            }
            else
            {
                newStatus = ContractStatus.PARTIALLY_PAID;
            }

            // Only update if different
            if (!string.Equals(contract.Status, newStatus.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                contract.Status = newStatus.ToString();
                contract = await _unitOfWork.FarmContracts.UpdateAsync(contract).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);
            }

            return contract;
        }

    }
}
