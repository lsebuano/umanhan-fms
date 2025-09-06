using Microsoft.Extensions.Logging;
using Umanhan.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class FarmContractSaleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<FarmContractSaleService> _logger;

        private async Task<List<FarmContractSaleDto>> ToFarmContractSaleDto(IEnumerable<FarmContractSale> farmContractSales)
        {
            return [.. farmContractSales.Select(x => new FarmContractSaleDto
            {
                FarmId = x.FarmId,
                ContractSaleId = x.Id,
                Date = x.Date.ToDateTime(TimeOnly.MinValue),
                Notes = x.Notes,
                Quantity = x.Quantity,
                TotalAmount = x.TotalAmount,
                Unit = x.UnitName,
                UnitId = x.UnitId,
                UnitPrice = x.UnitPrice,
                ContractDetailId = x.ContractDetailId,
                ProductId = x.ProductId,
                ProductTypeId = x.ProductTypeId,
                ProductType = x.ProductTypeName,
                Product = x.ProductName,
                ProductVariety = x.ProductVariety,
                CustomerId = x.CustomerId,
                Customer = x.CustomerName,
            })];
        }

        private async Task<FarmContractSaleDto> ToFarmContractSaleDto(FarmContractSale farmContractSale)
        {
            return new FarmContractSaleDto
            {
                FarmId = farmContractSale.FarmId,
                ContractSaleId = farmContractSale.Id,
                Date = farmContractSale.Date.ToDateTime(TimeOnly.MinValue),
                Notes = farmContractSale.Notes,
                Quantity = farmContractSale.Quantity,
                TotalAmount = farmContractSale.TotalAmount,
                Unit = farmContractSale.UnitName,
                UnitId = farmContractSale.UnitId,
                UnitPrice = farmContractSale.UnitPrice,
                ContractDetailId = farmContractSale.ContractDetailId,
                ProductId = farmContractSale.ProductId,
                ProductTypeId = farmContractSale.ProductTypeId,
                ProductType = farmContractSale.ProductTypeName,
                Product = farmContractSale.ProductName,
                ProductVariety = farmContractSale.ProductVariety,
                CustomerId = farmContractSale.CustomerId,
                Customer = farmContractSale.CustomerName,
            };
        }

        public FarmContractSaleService(IUnitOfWork unitOfWork, 
            IUserContextService userContext,
            ILogger<FarmContractSaleService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        public async Task<IEnumerable<FarmContractSaleDto>> GetFarmContractSalesAsync(Guid farmId, params string[] includeEntities)
        {
            var list = await _unitOfWork.FarmContractSales.GetFarmContractSalesAsync(farmId, includeEntities).ConfigureAwait(false);
            return await ToFarmContractSaleDto(list).ConfigureAwait(false);
        }

        public async Task<FarmContractSaleDto> GetFarmContractSaleByIdAsync(Guid id, params string[] includeEntities)
        {
            var obj = await _unitOfWork.FarmContractSales.GetByIdAsync(id, includeEntities).ConfigureAwait(false);
            if (obj == null)
                return null;
            return await ToFarmContractSaleDto(obj).ConfigureAwait(false);
        }

        public async Task<FarmContractSaleDto> CreateFarmContractSaleAsync(FarmContractSaleDto farmContractSale)
        {
            var newFarmContractSale = new FarmContractSale
            {
                FarmId = farmContractSale.FarmId,
                Date = DateOnly.FromDateTime(farmContractSale.Date),
                Notes = farmContractSale.Notes,
                Quantity = farmContractSale.Quantity,
                TotalAmount = farmContractSale.TotalAmount,
                UnitId = farmContractSale.UnitId,
                UnitPrice = farmContractSale.UnitPrice,
                ContractDetailId = farmContractSale.ContractDetailId,
                ProductId = farmContractSale.ProductId,
                ProductTypeId = farmContractSale.ProductTypeId,
                ProductName = farmContractSale.Product,
                ProductVariety = farmContractSale.ProductVariety,
                ProductTypeName = farmContractSale.ProductType,
                CustomerId = farmContractSale.CustomerId,
                CustomerName = farmContractSale.Customer,
                UnitName = farmContractSale.Unit
            };

            try
            {
                var createdFarmContractSale = await _unitOfWork.FarmContractSales.AddAsync(newFarmContractSale).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return await ToFarmContractSaleDto(createdFarmContractSale).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Farm Contract Sale: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<FarmContractSaleDto> UpdateFarmContractSaleAsync(FarmContractSaleDto farmContractSale)
        {
            var farmContractSaleEntity = await _unitOfWork.FarmContractSales.GetByIdAsync(farmContractSale.ContractSaleId).ConfigureAwait(false) ?? throw new KeyNotFoundException("FarmContractSale not found.");
            farmContractSaleEntity.TotalAmount = farmContractSale.TotalAmount;
            farmContractSaleEntity.Quantity = farmContractSale.Quantity;
            farmContractSaleEntity.UnitPrice = farmContractSale.UnitPrice;
            farmContractSaleEntity.Date = DateOnly.FromDateTime(farmContractSale.Date);
            farmContractSaleEntity.UnitId = farmContractSale.UnitId;
            farmContractSaleEntity.Notes = farmContractSale.Notes;
            farmContractSaleEntity.ContractDetailId = farmContractSale.ContractDetailId;
            farmContractSaleEntity.ProductId = farmContractSale.ProductId;
            farmContractSaleEntity.ProductTypeId = farmContractSale.ProductTypeId;
            farmContractSaleEntity.ProductName = farmContractSale.Product;
            farmContractSaleEntity.ProductVariety = farmContractSale.ProductVariety;
            farmContractSaleEntity.ProductTypeName = farmContractSale.ProductType;
            farmContractSaleEntity.CustomerId = farmContractSale.CustomerId;
            farmContractSaleEntity.CustomerName = farmContractSale.Customer;
            farmContractSaleEntity.UnitName = farmContractSale.Unit;
            farmContractSaleEntity.FarmId = farmContractSale.FarmId;

            try
            {
                var updatedFarmContractSale = await _unitOfWork.FarmContractSales.UpdateAsync(farmContractSaleEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return await ToFarmContractSaleDto(updatedFarmContractSale).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Farm Contract Sale: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<FarmContractSaleDto> DeleteFarmContractSaleAsync(Guid id)
        {
            var farmContractSaleEntity = await _unitOfWork.FarmContractSales.GetByIdAsync(id).ConfigureAwait(false);
            if (farmContractSaleEntity == null)
                return null;

            try
            {
                var deletedFarmContractSale = await _unitOfWork.FarmContractSales.DeleteAsync(id).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return await ToFarmContractSaleDto(new FarmContractSale()).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Farm Contract Sale: {Message}", ex.Message);
                throw;
            }
        }
    }
}
