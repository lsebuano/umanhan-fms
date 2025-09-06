using Umanhan.Dtos;
using Umanhan.Dtos.HelperModels;

namespace Umanhan.WebPortal.Spa.Services
{
    public class StaffService
    {
        private readonly ApiService _apiService;

        public StaffService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Task<ApiResponse<IEnumerable<StaffDto>>> GetAllStaffsAsync()
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<StaffDto>>("MasterdataAPI", "api/staffs");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<IEnumerable<StaffDto>>> GetStaffsByFarmAsync(Guid farmId)
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<StaffDto>>("MasterdataAPI", $"api/staffs/farm-id/{farmId}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<StaffDto>> GetStaffByIdAsync(Guid id)
        {
            try
            {
                return _apiService.GetAsync<StaffDto>("MasterdataAPI", $"api/staffs/{id}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<StaffDto> CreateStaffAsync(StaffDto staff)
        {
            try
            {
                var response = await _apiService.PostAsync<StaffDto, StaffDto>("MasterdataAPI", "api/staffs", staff).ConfigureAwait(false);
                if (!response.IsSuccess)
                {
                    if (response.Errors != null && response.Errors.Any())
                    {
                        var errors = new Dictionary<string, List<string>>();
                        foreach (var error in response.Errors)
                        {
                            errors.Add(error.Key, error.Value);
                        }
                        response.ErrorMessage = string.Join(", ", errors.SelectMany(x => x.Value));
                    }
                }
                return response.Data;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public async Task<StaffDto> UpdateStaffAsync(StaffDto staff)
        {
            try
            {
                var response = await _apiService.PutAsync<StaffDto, StaffDto>("MasterdataAPI", $"api/staffs/{staff.StaffId}", staff).ConfigureAwait(false);
                if (!response.IsSuccess)
                {
                    if (response.Errors != null && response.Errors.Any())
                    {
                        var errors = new Dictionary<string, List<string>>();
                        foreach (var error in response.Errors)
                        {
                            errors.Add(error.Key, error.Value);
                        }
                        response.ErrorMessage = string.Join(", ", errors.SelectMany(x => x.Value));
                    }
                }
                return response.Data;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public async Task<StaffDto> DeleteStaffAsync(Guid id)
        {
            try
            {
                var response = await _apiService.DeleteAsync<StaffDto>("MasterdataAPI", $"api/staffs/{id}").ConfigureAwait(false);
                if (!response.IsSuccess)
                {
                    if (response.Errors != null && response.Errors.Any())
                    {
                        var errors = new Dictionary<string, List<string>>();
                        foreach (var error in response.Errors)
                        {
                            errors.Add(error.Key, error.Value);
                        }
                        response.ErrorMessage = string.Join(", ", errors.SelectMany(x => x.Value));
                    }
                }
                return response.Data;
            }
            catch (Exception ex)
            {

            }
            return null;
        }
    }
}
