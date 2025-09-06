using Microsoft.Extensions.Logging;
using Umanhan.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class StaffService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<StaffService> _logger;

        private static List<StaffDto> ToStaffDto(IEnumerable<Staff> staffs)
        {
            return [.. staffs.Select(x => new StaffDto
            {
                StaffId = x.Id,
                ContactInfo = x.ContactInfo,
                Name = x.Name,
                Status = x.Status,
                FarmId = x.FarmId,
                HireDateUtc = x.HireDate,
            })
            .OrderBy(x => x.Name)];
        }

        private static StaffDto ToStaffDto(Staff staff)
        {
            return new StaffDto
            {
                StaffId = staff.Id,
                ContactInfo = staff.ContactInfo,
                Name = staff.Name,
                Status = staff.Status,
                FarmId = staff.FarmId,
                HireDateUtc = staff.HireDate,
            };
        }

        public StaffService(IUnitOfWork unitOfWork, 
            IUserContextService userContext,
            ILogger<StaffService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        public async Task<IEnumerable<StaffDto>> GetAllStaffsAsync(params string[] includeEntities)
        {
            var list = await _unitOfWork.Staffs.GetAllAsync(includeEntities).ConfigureAwait(false);
            return ToStaffDto(list);
        }

        public async Task<IEnumerable<StaffDto>> GetStaffsByFarmAsync(Guid farmId)
        {
            var list = await _unitOfWork.Staffs.GetStaffsByFarmAsync(farmId).ConfigureAwait(false);
            return ToStaffDto(list);
        }

        public async Task<StaffDto> GetStaffByIdAsync(Guid id, params string[] includeEntities)
        {
            var obj = await _unitOfWork.Staffs.GetByIdAsync(id, includeEntities).ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToStaffDto(obj);
        }

        public async Task<StaffDto> CreateStaffAsync(StaffDto staff)
        {
            var newStaff = new Staff
            {
                ContactInfo = staff.ContactInfo,
                Name = staff.Name,
                Status = staff.Status,
                FarmId = staff.FarmId,
                HireDate = staff.HireDate,
            };

            try
            {
                var createdStaff = await _unitOfWork.Staffs.AddAsync(newStaff).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToStaffDto(createdStaff);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating staff");
                throw;
            }
        }

        public async Task<StaffDto> UpdateStaffAsync(StaffDto staff)
        {
            var staffEntity = await _unitOfWork.Staffs.GetByIdAsync(staff.StaffId).ConfigureAwait(false) ?? throw new KeyNotFoundException("Staff not found.");
            staffEntity.ContactInfo = staff.ContactInfo;
            staffEntity.Name = staff.Name;
            staffEntity.Status = staff.Status;
            staffEntity.FarmId = staff.FarmId;
            staffEntity.HireDate = staff.HireDate;

            try
            {
                var updatedStaff = await _unitOfWork.Staffs.UpdateAsync(staffEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToStaffDto(updatedStaff);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating staff: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<StaffDto> DeleteStaffAsync(Guid id)
        {
            var staffEntity = await _unitOfWork.Staffs.GetByIdAsync(id).ConfigureAwait(false);
            if (staffEntity == null)
                return null;

            try
            {
                var deletedStaff = await _unitOfWork.Staffs.DeleteAsync(id).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToStaffDto(new Staff());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting staff: {Message}", ex.Message);
                throw;
            }
        }
    }
}
