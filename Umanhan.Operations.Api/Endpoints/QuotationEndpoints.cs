using FluentValidation;
using Umanhan.Dtos;
using Umanhan.Dtos.HelperModels;
using Umanhan.Services;

namespace Umanhan.Operations.Api.Endpoints
{
    public class QuotationEndpoints
    {
        private readonly QuotationService _quotationService;
        private readonly IValidator<QuotationDto> _validator;
        private readonly IValidator<QuotationDetailDto> _validatorDetail;
        private readonly ILogger<QuotationEndpoints> _logger;

        public QuotationEndpoints(QuotationService quotationService, IValidator<QuotationDto> validator,
            IValidator<QuotationDetailDto> validatorDetail, ILogger<QuotationEndpoints> logger)
        {
            _quotationService = quotationService;
            _validator = validator;
            _validatorDetail = validatorDetail;
            _logger = logger;
        }

        public async Task<IResult> GetQuotationsByFarmIdAsync(Guid farmId)
        {
            try
            {
                var list = await _quotationService.GetQuotationsByFarmIdAsync(farmId).ConfigureAwait(false);
                return Results.Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving quotations for farm {FarmId}", farmId);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetQuotationsByFarmIdAsync(Guid farmId, int topN)
        {
            try
            {
                var list = await _quotationService.GetQuotationsByFarmIdAsync(farmId, topN).ConfigureAwait(false);
                return Results.Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving quotations for farm {FarmId}", farmId);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetTop3QuotationsByFarmIdAsync(Guid farmId)
        {
            try
            {
                var list = await _quotationService.GetQuotationsByFarmIdAsync(farmId, 3).ConfigureAwait(false);
                return Results.Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving quotations for farm {FarmId}", farmId);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetQuotationByIdAsync(Guid id)
        {
            try
            {
                var obj = await _quotationService.GetQuotationByIdAsync(id).ConfigureAwait(false);
                return Results.Ok(obj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving quotation by ID {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> SendQuotationEmailAsync(SendQuotationParamsModel paramsModel)
        {
            try
            {
                await _quotationService.SendQuotationEmailAsync(paramsModel).ConfigureAwait(false);
                return Results.Ok("SUCCESS");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new quotation.");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> CreateAndSendQuotationEmailAsync(SendQuotationParamsModel paramsModel)
        {
            try
            {
                await _quotationService.CreateAndSendQuotationEmailAsync(paramsModel).ConfigureAwait(false);
                return Results.Ok("SUCCESS");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new quotation.");
                return Results.Problem(ex.Message);
            }
        }

        //public async Task<IResult> RenderQuotationMessagePreviewAsync(SendQuotationParamsModel paramsModel)
        //{
        //    try
        //    {
        //        var message = await _quotationService.RenderQuotationMessagePreviewAsync(paramsModel).ConfigureAwait(false);
        //        return Results.Ok(message);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error creating new quotation.");
        //        return Results.Problem(ex.Message);
        //    }
        //}

        //public async Task<IResult> CreateQuotationProductAsync(QuotationProductDto model)
        //{
        //    try
        //    {
        //        var result = await _quotationService.CreateQuotationProductAsync(model).ConfigureAwait(false);
        //        return Results.Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error creating new quotation product item.");
        //        return Results.Problem(ex.Message);
        //    }
        //}

        //public async Task<IResult> UpdateQuotationProductAsync(QuotationProductDto model)
        //{
        //    try
        //    {
        //        var result = await _quotationService.UpdateQuotationProductAsync(model).ConfigureAwait(false);
        //        return Results.Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error updating new quotation product item.");
        //        return Results.Problem(ex.Message);
        //    }
        //}

        //public async Task<IResult> DeleteQuotationProductAsync(Guid quotationProductId)
        //{
        //    try
        //    {
        //        var result = await _quotationService.DeleteQuotationProductAsync(quotationProductId).ConfigureAwait(false);
        //        return Results.Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error deleting quotation product item.");
        //        return Results.Problem(ex.Message);
        //    }
        //}
    }
}
