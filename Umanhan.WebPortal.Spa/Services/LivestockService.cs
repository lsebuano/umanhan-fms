using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Models.Models;

namespace Umanhan.WebPortal.Spa.Services
{
    public class LivestockService
    {
        private readonly ApiService _apiService;

        public LivestockService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Task<ApiResponse<IEnumerable<LivestockDto>>> GetAllLivestocksAsync()
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<LivestockDto>>("MasterdataAPI", "api/livestocks");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<LivestockDto>> GetLivestockByIdAsync(Guid id)
        {
            try
            {
                return _apiService.GetAsync<LivestockDto>("MasterdataAPI", $"api/livestocks/{id}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<LivestockDto> CreateLivestockAsync(LivestockDto livestock)
        {
            try
            {
                var response = await _apiService.PostAsync<LivestockDto, LivestockDto>("MasterdataAPI", "api/livestocks", livestock).ConfigureAwait(false);
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

        public async Task<LivestockDto> UpdateLivestockAsync(LivestockDto livestock)
        {
            try
            {
                var response = await _apiService.PutAsync<LivestockDto, LivestockDto>("MasterdataAPI", $"api/livestocks/{livestock.LivestockId}", livestock).ConfigureAwait(false);
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

        public async Task<LivestockDto> DeleteLivestockAsync(Guid id)
        {
            try
            {
                var response = await _apiService.DeleteAsync<LivestockDto>("MasterdataAPI", $"api/livestocks/{id}").ConfigureAwait(false);
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
