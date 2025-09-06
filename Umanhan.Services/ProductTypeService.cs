using Microsoft.Extensions.Logging;
using Umanhan.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class ProductTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<ProductTypeService> _logger;

        private static List<ProductTypeDto> ToProductTypeDto(IEnumerable<ProductType> productTypes)
        {
            return [.. productTypes.Select(x => new ProductTypeDto
            {
                TypeId = x.Id,
                ProductTypeName = x.ProductTypeName,
                TableName = x.TableName,
            })
            .OrderBy(x => x.ProductTypeName)];
        }

        private static ProductTypeDto ToProductTypeDto(ProductType productType)
        {
            return new ProductTypeDto
            {
                TypeId = productType.Id,
                ProductTypeName = productType.ProductTypeName,
                TableName = productType.TableName,
            };
        }

        public ProductTypeService(IUnitOfWork unitOfWork, 
            IUserContextService userContext,
            ILogger<ProductTypeService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        public async Task<IEnumerable<ProductTypeDto>> GetAllProductTypesAsync(params string[] includeEntities)
        {
            var list = await _unitOfWork.ProductTypes.GetAllAsync(includeEntities).ConfigureAwait(false);
            return ToProductTypeDto(list);
        }

        public async Task<ProductTypeDto> GetProductTypeByIdAsync(Guid id, params string[] includeEntities)
        {
            var obj = await _unitOfWork.ProductTypes.GetByIdAsync(id, includeEntities).ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToProductTypeDto(obj);
        }

        public async Task<ProductTypeDto> CreateProductTypeAsync(ProductTypeDto productType)
        {
            var newProductType = new ProductType
            {
                ProductTypeName = productType.ProductTypeName,
                TableName = productType.TableName,
            };

            try
            {
                var createdProductType = await _unitOfWork.ProductTypes.AddAsync(newProductType).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToProductTypeDto(createdProductType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating ProductType: {ProductTypeName}", productType.ProductTypeName);
                throw;
            }
        }

        public async Task<ProductTypeDto> UpdateProductTypeAsync(ProductTypeDto productType)
        {
            var productTypeEntity = await _unitOfWork.ProductTypes.GetByIdAsync(productType.TypeId).ConfigureAwait(false) ?? throw new KeyNotFoundException("ProductType not found.");
            productTypeEntity.ProductTypeName = productType.ProductTypeName;
            productTypeEntity.TableName = productType.TableName;

            try
            {
                var updatedProductType = await _unitOfWork.ProductTypes.UpdateAsync(productTypeEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToProductTypeDto(updatedProductType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating ProductType: {ProductTypeName}", productType.ProductTypeName);
                throw;
            }
        }

        public async Task<ProductTypeDto> DeleteProductTypeAsync(Guid id)
        {
            var productTypeEntity = await _unitOfWork.ProductTypes.GetByIdAsync(id).ConfigureAwait(false);
            if (productTypeEntity == null)
                return null;

            try
            {
                var deletedProductType = await _unitOfWork.ProductTypes.DeleteAsync(id).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToProductTypeDto(new ProductType());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting ProductType with ID: {Id}", id);
                throw;
            }
        }
    }
}
