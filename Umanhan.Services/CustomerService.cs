using Microsoft.Extensions.Logging;
using Umanhan.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class CustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<CustomerService> _logger;

        private static List<CustomerDto> ToCustomerDto(IEnumerable<Customer> customers)
        {
            return [.. customers.Select(x => new CustomerDto
            {
                CustomerId = x.Id,
                CustomerName = x.CustomerName,
                Address = x.Address,
                ContactInfo = x.ContactInfo,
                CustomerTypeId = x.CustomerTypeId,
                CustomerType = x.CustomerType?.CustomerTypeName,
                ContractEligible = x.ContractEligible,
                EmailAddress = x.EmailAddress,
                FarmContracts = x.FarmContracts.Select(a => new FarmContractDto{
                    ContractDate = a.ContractDate.ToDateTime(TimeOnly.MinValue),
                    ContractId = a.Id,
                    FarmName = a.Farm?.FarmName,
                    FarmLocation = a.Farm?.Location,
                    Status = a.Status
                })
            })
            .OrderBy(x => x.CustomerName)];
        }

        private static CustomerDto ToCustomerDto(Customer customer)
        {
            return new CustomerDto
            {
                CustomerId = customer.Id,
                CustomerName = customer.CustomerName,
                Address = customer.Address,
                ContactInfo = customer.ContactInfo,
                CustomerTypeId = customer.CustomerTypeId,
                CustomerType = customer.CustomerType?.CustomerTypeName,
                ContractEligible = customer.ContractEligible,
                EmailAddress = customer.EmailAddress,
                FarmContracts = customer.FarmContracts.Select(a => new FarmContractDto
                {
                    ContractDate = a.ContractDate.ToDateTime(TimeOnly.MinValue),
                    ContractId = a.Id,
                    FarmName = a.Farm?.FarmName,
                    FarmLocation = a.Farm?.Location,
                    Status = a.Status
                })
            };
        }

        public CustomerService(IUnitOfWork unitOfWork, 
            IUserContextService userContext,
            ILogger<CustomerService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync(params string[] includeEntities)
        {
            var list = await _unitOfWork.Customers.GetAllAsync(includeEntities).ConfigureAwait(false);
            return ToCustomerDto(list);
        }

        public async Task<IEnumerable<CustomerDto>> GetCustomersContractEligibleAsync(params string[] includeEntities)
        {
            var list = await _unitOfWork.Customers.GetCustomersContractEligibleAsync(includeEntities).ConfigureAwait(false);
            return ToCustomerDto(list);
        }

        public async Task<IEnumerable<CustomerDto>> GetCustomersByTypeAsync(Guid customerTypeId, params string[] includeEntities)
        {
            var list = await _unitOfWork.Customers.GetCustomersByTypeAsync(customerTypeId, includeEntities).ConfigureAwait(false);
            return ToCustomerDto(list);
        }

        public async Task<CustomerDto> GetCustomerByIdAsync(Guid id, params string[] includeEntities)
        {
            var obj = await _unitOfWork.Customers.GetByIdAsync(id, includeEntities).ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToCustomerDto(obj);
        }

        public async Task<CustomerDto> CreateCustomerAsync(CustomerDto customer)
        {
            var newCustomer = new Customer
            {
                CustomerName = customer.CustomerName,
                Address = customer.Address,
                ContactInfo = customer.ContactInfo,
                CustomerTypeId = customer.CustomerTypeId,
                ContractEligible = customer.ContractEligible,
                EmailAddress = customer.EmailAddress
            };

            try
            {
                var createdCustomer = await _unitOfWork.Customers.AddAsync(newCustomer).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToCustomerDto(createdCustomer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<CustomerDto> UpdateCustomerAsync(CustomerDto customer)
        {
            var customerEntity = await _unitOfWork.Customers.GetByIdAsync(customer.CustomerId).ConfigureAwait(false) ?? throw new KeyNotFoundException("Customer not found.");
            customerEntity.CustomerName = customer.CustomerName;
            customerEntity.Address = customer.Address;
            customerEntity.ContactInfo = customer.ContactInfo;
            customerEntity.CustomerTypeId = customer.CustomerTypeId;
            customerEntity.ContractEligible = customer.ContractEligible;
            customerEntity.EmailAddress = customer.EmailAddress;

            try
            {
                var updatedCustomer = await _unitOfWork.Customers.UpdateAsync(customerEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToCustomerDto(updatedCustomer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating customer: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<CustomerDto> DeleteCustomerAsync(Guid id)
        {
            var customerEntity = await _unitOfWork.Customers.GetByIdAsync(id).ConfigureAwait(false);
            if (customerEntity == null)
                return null;

            try
            {
                var deletedCustomer = await _unitOfWork.Customers.DeleteAsync(id).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToCustomerDto(new Customer());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer: {Message}", ex.Message);
                throw;
            }
        }
    }
}
