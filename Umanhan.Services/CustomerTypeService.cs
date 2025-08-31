using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class CustomerTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<CustomerTypeService> _logger;

        private static List<CustomerTypeDto> ToCustomerTypeDto(IEnumerable<CustomerType> customerTypes)
        {
            return [.. customerTypes.Select(x => new CustomerTypeDto
            {
                TypeId = x.Id,
                CustomerTypeName = x.CustomerTypeName,
                Customers = x.Customers.Select(y => new CustomerDto{
                    Address = y.Address,
                    ContactInfo = y.ContactInfo,
                    CustomerId = y.Id,
                    CustomerName = y.CustomerName
                })
                .OrderBy(xx => xx.CustomerName)
            })
            .OrderBy(x => x.CustomerTypeName)];
        }

        private static CustomerTypeDto ToCustomerTypeDto(CustomerType customerType)
        {
            return new CustomerTypeDto
            {
                TypeId = customerType.Id,
                CustomerTypeName = customerType.CustomerTypeName,
                Customers = customerType.Customers.Select(y => new CustomerDto
                {
                    Address = y.Address,
                    ContactInfo = y.ContactInfo,
                    CustomerId = y.Id,
                    CustomerName = y.CustomerName
                })
                .OrderBy(xx => xx.CustomerName)
            };
        }

        public CustomerTypeService(IUnitOfWork unitOfWork, 
            IUserContextService userContext,
            ILogger<CustomerTypeService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        public async Task<IEnumerable<CustomerTypeDto>> GetAllCustomerTypesAsync(params string[] includeEntities)
        {
            var list = await _unitOfWork.CustomerTypes.GetAllAsync(includeEntities).ConfigureAwait(false);
            return ToCustomerTypeDto(list);
        }

        public async Task<CustomerTypeDto> GetCustomerTypeByIdAsync(Guid id, params string[] includeEntities)
        {
            var obj = await _unitOfWork.CustomerTypes.GetByIdAsync(id, includeEntities).ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToCustomerTypeDto(obj);
        }

        public async Task<CustomerTypeDto> CreateCustomerTypeAsync(CustomerTypeDto customerType)
        {
            var newCustomerType = new CustomerType
            {
                CustomerTypeName = customerType.CustomerTypeName,
            };

            try
            {
                var createdCustomerType = await _unitOfWork.CustomerTypes.AddAsync(newCustomerType).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToCustomerTypeDto(createdCustomerType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer type: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<CustomerTypeDto> UpdateCustomerTypeAsync(CustomerTypeDto customerType)
        {
            var customerTypeEntity = await _unitOfWork.CustomerTypes.GetByIdAsync(customerType.TypeId).ConfigureAwait(false) ?? throw new KeyNotFoundException("CustomerType not found.");
            customerTypeEntity.CustomerTypeName = customerType.CustomerTypeName;

            try
            {
                var updatedCustomerType = await _unitOfWork.CustomerTypes.UpdateAsync(customerTypeEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToCustomerTypeDto(updatedCustomerType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating customer type: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<CustomerTypeDto> DeleteCustomerTypeAsync(Guid id)
        {
            var customerTypeEntity = await _unitOfWork.CustomerTypes.GetByIdAsync(id).ConfigureAwait(false);
            if (customerTypeEntity == null)
                return null;

            try
            {
                var deletedCustomerType = await _unitOfWork.CustomerTypes.DeleteAsync(id).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToCustomerTypeDto(new CustomerType());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer type: {Message}", ex.Message);
                throw;
            }
        }
    }
}
