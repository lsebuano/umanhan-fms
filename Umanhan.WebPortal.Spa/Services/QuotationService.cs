using Umanhan.Models.Dtos;
using Umanhan.Models.Models;

namespace Umanhan.WebPortal.Spa.Services
{
    public class QuotationService
    {
        private readonly ApiService _apiService;

        public QuotationService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Task<ApiResponse<QuotationDto>> GetQuotationByIdAsync(Guid id)
        {
            try
            {
                return _apiService.GetAsync<QuotationDto>("OperationsAPI", $"api/quotations/{id}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<IEnumerable<QuotationDto>>> GetQuotationsByFarmIdAsync(Guid farmId)
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<QuotationDto>>("OperationsAPI", $"api/quotations/farm-id/{farmId}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<IEnumerable<QuotationDto>>> GetTop3QuotationsByFarmIdAsync(Guid farmId)
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<QuotationDto>>("OperationsAPI", $"api/quotations/farm-id/{farmId}/top3");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<string> SendQuotationAsync(SendQuotationParamsModel paramsModel)
        {
            try
            {
                var response = await _apiService.PostAsync<SendQuotationParamsModel, string>("OperationsAPI", "api/quotations/send-quotation", paramsModel).ConfigureAwait(false);
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

        public async Task<string> CreateAndSendQuotationAsync(SendQuotationParamsModel paramsModel)
        {
            try
            {
                var response = await _apiService.PostAsync<SendQuotationParamsModel, string>("OperationsAPI", "api/quotations/create-send-quotation", paramsModel).ConfigureAwait(false);
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

        //public async Task<QuotationProductDto> CreateQuotationProductAsync(QuotationProductDto model)
        //{
        //    try
        //    {
        //        var response = await _apiService.PostAsync<QuotationProductDto, QuotationProductDto>("OperationsAPI", "api/quotations/product", model).ConfigureAwait(false);
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

        //public async Task<QuotationProductDto> UpdateQuotationProductAsync(QuotationProductDto model)
        //{
        //    try
        //    {
        //        var response = await _apiService.PutAsync<QuotationProductDto, QuotationProductDto>("OperationsAPI", "api/quotations/product", model).ConfigureAwait(false);
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

        //public async Task<string> DeleteQuotationProductAsync(Guid quotationProductId)
        //{
        //    try
        //    {
        //        var response = await _apiService.DeleteAsync<string>("OperationsAPI", $"api/quotations/product/{quotationProductId}").ConfigureAwait(false);
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
    }
}
