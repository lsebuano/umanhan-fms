using System.Diagnostics;
using Umanhan.Models.Dtos;
using Umanhan.Models.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Umanhan.WebPortal.Spa.Services
{
    public class FarmActivityService
    {
        private readonly ApiService _apiService;

        public FarmActivityService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Task<ApiResponse<IEnumerable<FarmActivityDto>>> GetFarmActivitiesAsync(Guid farmId)
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<FarmActivityDto>>("OperationsAPI", $"api/farm-activities/farm-id/{farmId}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<IEnumerable<FarmActivityDto>>> GetFarmActivitiesAsync(Guid farmId, DateTime date)
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<FarmActivityDto>>("OperationsAPI", $"api/farm-activities/farm-id/{farmId}/start-date/{date.ToString("O")}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<FarmActivityDto>> GetFarmActivityByIdAsync(Guid id)
        {
            try
            {
                return _apiService.GetAsync<FarmActivityDto>("OperationsAPI", $"api/farm-activities/{id}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ApiResponse<FarmActivityDto>> CreateFarmActivitiesAsync(FarmActivityDto farmActivity)
        {
            try
            {
                var response = await _apiService.PostAsync<FarmActivityDto, FarmActivityDto>("OperationsAPI", $"api/farm-activities", farmActivity).ConfigureAwait(false);
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
                return response;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public async Task<ApiResponse<FarmActivityDto>> CreateHarvestActivityFromContractDetailsAsync(FarmContractDetailDto contractDetail, Guid farmId, Guid taskId, Guid supervisorId)
        {
            try
            {
                var farmActivity = new FarmActivityDto
                {
                    FarmId = farmId,
                    ProductId = contractDetail.ProductId,
                    ProductTypeId = contractDetail.ProductTypeId,
                    ContractId = contractDetail.ContractId,
                    StartDateTime = contractDetail.HarvestDate.Value,
                    EndDateTime = contractDetail.HarvestDate.Value.AddDays(1).AddMinutes(-1),
                    Notes = $"Harvest activity for {contractDetail.Product} on {contractDetail.HarvestDate?.ToString("MM/dd/yyyy")}",
                    TaskId = taskId,
                    SupervisorId = supervisorId
                };

                var response = await _apiService.PostAsync<FarmActivityDto, FarmActivityDto>("OperationsAPI", $"api/farm-activities", farmActivity).ConfigureAwait(false);
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
                return response;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public async Task<FarmActivityDto> UpdateFarmActivitiesAsync(FarmActivityDto farmActivity)
        {
            try
            {
                var response = await _apiService.PutAsync<FarmActivityDto, FarmActivityDto>("OperationsAPI", $"api/farm-activities/{farmActivity.ActivityId}", farmActivity).ConfigureAwait(false);
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

        public async Task<FarmActivityDto> DeleteFarmActivitiesAsync(FarmActivityDto farmActivity)
        {
            try
            {
                var response = await _apiService.PutAsync<Guid, FarmActivityDto>("OperationsAPI", $"api/farm-activities", farmActivity.ActivityId).ConfigureAwait(false);
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
