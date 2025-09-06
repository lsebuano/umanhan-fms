using Microsoft.Extensions.Logging;
using Umanhan.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class ProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<ProductService> _logger;

        private static List<ProductDto> ToProductDto(IEnumerable<ProductDto> products)
        {
            return [.. products.Select(x => new ProductDto
            {
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                Variety = x.Variety,
                ProductTypeId = x.ProductTypeId,
                ProductTypeName = x.ProductTypeName,
            })
            .OrderBy(x => x.ProductName)];
        }

        private static ProductDto ToProductDto(ProductDto product)
        {
            return new ProductDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Variety = product.Variety,
                ProductTypeId = product.ProductTypeId,
                ProductTypeName = product.ProductTypeName,
            };
        }

        private static List<ProductDto> ToProductDto2(IEnumerable<VwProductLookup> products)
        {
            return [.. products.Select(x => new ProductDto
            {
                ProductId = x.ProductId.Value,
                ProductTypeId = x.ProductTypeId.Value,
                FarmId = x.FarmId.Value,
                UnitId = x.UnitId.Value,
                ZoneId = x.ZoneId.Value,
                FarmName = x.FarmName,
                UnitName = x.UnitName,
                ZoneName = x.ZoneName,
                StartDate = x.StartDate?.ToDateTime(TimeOnly.MinValue),
                EstimatedHarvestDate = x.EstimatedHarvestDate?.ToDateTime(TimeOnly.MinValue),
                DefaultRate = x.DefaultRate ?? 1,
                ProductName = x.ProductName,
                Variety = x.Variety,
                ProductTypeName = x.ProductTypeName,
            })
            .OrderBy(x => x.ProductName)];
        }

        private static ProductDto ToProductDto2(VwProductLookup product)
        {
            return new ProductDto
            {
                ProductId = product.ProductId.Value,
                ProductTypeId = product.ProductTypeId.Value,
                FarmId = product.FarmId.Value,
                UnitId = product.UnitId.Value,
                ZoneId = product.ZoneId.Value,
                FarmName = product.FarmName,
                UnitName = product.UnitName,
                ZoneName = product.ZoneName,
                StartDate = product.StartDate?.ToDateTime(TimeOnly.MinValue),
                EstimatedHarvestDate = product.EstimatedHarvestDate?.ToDateTime(TimeOnly.MinValue),
                DefaultRate = product.DefaultRate ?? 1,
                ProductName = product.ProductName,
                Variety = product.Variety,
                ProductTypeName = product.ProductTypeName,
            };
        }

        public ProductService(IUnitOfWork unitOfWork, 
            IUserContextService userContext,
            ILogger<ProductService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByFarmAsync(Guid farmId)
        {
            var list = await _unitOfWork.ProductLookup.GetProductsByFarmAsync(farmId).ConfigureAwait(false);
            return ToProductDto2(list);
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByFarmByTypeAsync(Guid farmId, Guid typeId)
        {
            var list = await _unitOfWork.ProductLookup.GetProductsByFarmByTypeAsync(farmId, typeId).ConfigureAwait(false);
            return ToProductDto2(list);
        }

        public async Task<ProductDto> GetProductByIdAsync(Guid typeId, Guid id)
        {
            var product = await _unitOfWork.ProductLookup.GetProductByIdAsync(typeId, id).ConfigureAwait(false);
            if (product == null)
                return null;
            return ToProductDto2(product);
        }
    }
}
