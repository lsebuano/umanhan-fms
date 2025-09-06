using Umanhan.Dtos;
using Umanhan.Dtos.HelperModels;

namespace Umanhan.WebPortal.Spa.Services
{
    public class FarmZoneService
    {
        private readonly ApiService _apiService;

        public FarmZoneService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Task<ApiResponse<IEnumerable<FarmZoneDto>>> GetFarmZonesAsync()
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<FarmZoneDto>>("OperationsAPI", $"api/farm-zones");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<IEnumerable<FarmZoneDto>>> GetFarmZonesAsync(Guid farmId)
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<FarmZoneDto>>("OperationsAPI", $"api/farm-zones/farm-id/{farmId}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<FarmZoneDto>> GetFarmZoneByIdAsync(Guid id)
        {
            try
            {
                return _apiService.GetAsync<FarmZoneDto>("OperationsAPI", $"api/farm-zones/{id}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<FarmZoneDto> CreateFarmZoneAsync(FarmZoneDto farmZone)
        {
            try
            {
                var response = await _apiService.PostAsync<FarmZoneDto, FarmZoneDto>("OperationsAPI", $"api/farm-zones", farmZone).ConfigureAwait(false);
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

        public async Task<FarmZoneDto> UpdateFarmZoneAsync(FarmZoneDto farmZone)
        {
            try
            {
                var response = await _apiService.PutAsync<FarmZoneDto, FarmZoneDto>("OperationsAPI", $"api/farm-zones", farmZone).ConfigureAwait(false);
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

        public async Task<FarmZoneDto> CreateFarmZoneBoundaryAsync(FarmZoneDto farmZone)
        {
            try
            {
                var response = await _apiService.PostAsync<FarmZoneDto, FarmZoneDto>("OperationsAPI", $"api/farm-zones/boundary", farmZone).ConfigureAwait(false);
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

        //public async Task<FarmZoneDto> UpdateFarmZoneBoundaryAsync(FarmZoneDto farmZone)
        //{
        //    try
        //    {
        //        var response = await _apiService.PutAsync<FarmZoneDto, FarmZoneDto>("OperationsAPI", $"api/farm-zones/boundary/{farmZone.ZoneId}", farmZone).ConfigureAwait(false);
        //        if (!response.IsSuccess)
        //        {
        //            if (response.Errors != null && response.Errors.Any())
        //            {
        //                var errors = new Dictionary<string, List<string>>();
        //                foreach (var error in response.Errors)
        //                {
        //                    errors.Add(error.Key, error.Value);
        //                }
        //                response.ErrorMessage = string.Join(", ", errors.SelectMany(x => x.Value));
        //            }
        //        }
        //        return response.Data;
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return null;
        //}

        public async Task<FarmZoneDto> CreateUpdateFarmZoneBoundaryAsync(FarmZoneDto farmZone)
        {
            try
            {
                var response = await _apiService.PostAsync<FarmZoneDto, FarmZoneDto>("OperationsAPI", $"api/farm-zones/boundary", farmZone).ConfigureAwait(false);
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
