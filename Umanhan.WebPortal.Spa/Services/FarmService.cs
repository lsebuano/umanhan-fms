using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Models.Models;
using Umanhan.WebPortal.Spa.Pages;

namespace Umanhan.WebPortal.Spa.Services
{
    public class FarmService
    {
        private readonly ApiService _apiService;
        private readonly SecretService _secretService;

        public FarmService(ApiService apiService, SecretService secretService)
        {
            _apiService = apiService;
            _secretService = secretService;
        }

        public Task<ApiResponse<IEnumerable<FarmDto>>> GetAllFarmsAsync()
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<FarmDto>>("MasterdataAPI", "api/farms");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<FarmDto>> GetFarmByIdAsync(Guid id)
        {
            try
            {
                return _apiService.GetAsync<FarmDto>("MasterdataAPI", $"api/farms/{id}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<FarmDto> CreateFarmAsync(FarmDto farm)
        {
            try
            {
                var response = await _apiService.PostAsync<FarmDto, FarmDto>("MasterdataAPI", "api/farms", farm).ConfigureAwait(false);
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

        public async Task<FarmDto> UpdateFarmAsync(FarmDto farm)
        {
            try
            {
                var response = await _apiService.PutAsync<FarmDto, FarmDto>("MasterdataAPI", $"api/farms/{farm.FarmId}", farm).ConfigureAwait(false);
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

                var r2 = await _secretService.UpdateSecretAsync(new KeyValuePair<string, string>("FarmName", farm.FarmName)).ConfigureAwait(false);
                
                return response.Data;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public async Task<FarmDto> DeleteFarmAsync(Guid id)
        {
            throw new NotImplementedException("Delete Farm is not supported yet.");
            //try
            //{
            //    var response = await _apiService.DeleteAsync<FarmDto>("MasterdataAPI", $"api/farms/{id}").ConfigureAwait(false);
            //    if (!response.IsSuccess)
            //    {
            //        if (response.Errors != null && response.Errors.Any())
            //        {
            //            var errors = new Dictionary<string, List<string>>();
            //            foreach (var error in response.Errors)
            //            {
            //                errors.Add(error.Key, error.Value);
            //            }
            //            response.ErrorMessage = string.Join(", ", errors.SelectMany(x => x.Value));
            //        }
            //    }
            //    return response.Data;
            //}
            //catch (Exception ex)
            //{

            //}
            //return null;
        }

        public async Task<bool> CompleteFarmSetupAsync(FarmSetupDto farm)
        {
            try
            {
                //var response = await _apiService.PutAsync<Guid, bool>("MasterdataAPI", $"api/farms/complete-setup/{farmId}", farmId).ConfigureAwait(false);
                var response = await _apiService.PostAsJsonAsync<FarmSetupDto, FarmDto>("MasterdataAPI", $"api/farms/complete-setup", farm).ConfigureAwait(false);
                Console.WriteLine(response.IsSuccess);
                if (response.IsSuccess)
                {
                    //var r2 = await _secretService.UpdateSecretAsync(new KeyValuePair<string, string>("IsFarmSetupStarted", "true")).ConfigureAwait(false);
                    //var r3 = await _secretService.UpdateSecretAsync(new KeyValuePair<string, string>("IsFarmSetupComplete", "true")).ConfigureAwait(false);
                    return true;
                }
                else
                {
                    // Handle error
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                return false;
            }
        }

        public async Task<bool> IsFarmSetupCompleteAsync(Guid id)
        {
            try
            {
                var result = await _apiService.GetAsync<bool>("MasterdataAPI", $"api/farms/{id}/setup-complete").ConfigureAwait(false);
                if (result.IsSuccess)
                {
                    return result.Data;
                }
                else
                {
                    // Handle error
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                return false;
            }
        }
    }
}
