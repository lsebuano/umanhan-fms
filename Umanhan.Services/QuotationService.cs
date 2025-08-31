using Amazon.SimpleEmail;
using Microsoft.Extensions.Logging;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Models.Models;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace Umanhan.Services
{
    public class QuotationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly IEmailService _emailService;
        private readonly PricingService _pricingService;
        private readonly PricingProfileService _pricingProfileService;
        //private readonly IAmazonSimpleEmailService _awsEmailService;
        private readonly ILogger<QuotationService> _logger;

        private static List<QuotationDto> ToQuotationDto(IEnumerable<Quotation> quotations)
        {
            return [.. quotations.Select(x => new QuotationDto
            {
                QuotationId = x.Id,
                FarmId = x.FarmId,
                FarmName = x.Farm?.FarmName,
                RecipientEmail = x.RecipientEmail,
                RecipientName = x.RecipientName,
                BasePrice = x.BasePrice,
                FinalPrice = x.FinalPrice,
                ValidUntil = x.ValidUntil,
                DateSent = x.DateSent,
                Status = x.Status,
                PricingProfileId = x.PricingProfileId,
                QuotationNumber = x.QuotationNumber,
                AwsSesMessageId = x.AwsSesMessageId,
                //QuotationDetails = x.QuotationDetails.Select(qd => new QuotationDetailDto
                //{
                //    After = qd.After,
                //    Before = qd.Before,
                //    Delta = qd.Delta,
                //    IsDeduction = qd.IsDeduction,
                //    ConditionType = qd.ConditionType,
                //    ApplyType = qd.ApplyType,
                //    Value = qd.Value,
                //    Sequence = qd.Sequence,
                //    QuotationId = qd.QuotationId,
                //    DetailId = qd.Id,
                //}).ToList()
                QuotationProducts = x.QuotationProducts.Select(c => new QuotationProductDto {
                    ProductId = c.ProductId,
                    ProductName = c.ProductName,
                    ProductTypeId = c.ProductTypeId,
                    Quantity = c.Quantity,
                    UnitPrice = c.UnitPrice,
                }).ToList()
            })
            .OrderBy(x => x.RecipientName)];
        }

        private static QuotationDto ToQuotationDto(Quotation quotation)
        {
            return new QuotationDto
            {
                QuotationId = quotation.Id,
                FarmId = quotation.FarmId,
                FarmName = quotation.Farm?.FarmName,
                RecipientEmail = quotation.RecipientEmail,
                RecipientName = quotation.RecipientName,
                BasePrice = quotation.BasePrice,
                FinalPrice = quotation.FinalPrice,
                ValidUntil = quotation.ValidUntil,
                DateSent = quotation.DateSent,
                Status = quotation.Status,
                PricingProfileId = quotation.PricingProfileId,
                QuotationNumber = quotation.QuotationNumber,
                AwsSesMessageId = quotation.AwsSesMessageId,
                //QuotationDetails = quotation.QuotationDetails.Select(qd => new QuotationDetailDto
                //{
                //    After = qd.After,
                //    Before = qd.Before,
                //    Delta = qd.Delta,
                //    IsDeduction = qd.IsDeduction,
                //    ConditionType = qd.ConditionType,
                //    ApplyType = qd.ApplyType,
                //    Value = qd.Value,
                //    Sequence = qd.Sequence,
                //    QuotationId = qd.QuotationId,
                //    DetailId = qd.Id
                //}).ToList()
                QuotationProducts = quotation.QuotationProducts.Select(c => new QuotationProductDto
                {
                    ProductId = c.ProductId,
                    ProductName = c.ProductName,
                    ProductTypeId = c.ProductTypeId,
                    Quantity = c.Quantity,
                    UnitPrice = c.UnitPrice,
                }).ToList()
            };
        }

        public QuotationService(IUnitOfWork unitOfWork, 
            IUserContextService userContext,
            IEmailService emailService, 
            PricingService pricingService,
            //IAmazonSimpleEmailService awsEmailService,
            PricingProfileService pricingProfileService,
            ILogger<QuotationService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._emailService = emailService;
            this._pricingService = pricingService;
            //this._awsEmailService = awsEmailService;
            this._pricingProfileService = pricingProfileService;
            this._logger = logger;
        }

        public async Task<IEnumerable<QuotationDto>> GetQuotationsByFarmIdAsync(Guid farmId)
        {
            var list = await _unitOfWork.Quotations.GetQuotationsByFarmIdAsync(farmId, "Farm", "QuotationProducts").ConfigureAwait(false);
            return ToQuotationDto(list);
        }

        public async Task<IEnumerable<QuotationDto>> GetQuotationsByFarmIdAsync(Guid farmId, int topN)
        {
            var list = await _unitOfWork.Quotations.GetQuotationsByFarmIdAsync(farmId, topN, "Farm", "QuotationProducts").ConfigureAwait(false);
            return ToQuotationDto(list);
        }

        public async Task<QuotationDto> GetQuotationByIdAsync(Guid id)
        {
            var obj = await _unitOfWork.Quotations.GetByIdAsync(id, "Farm", "QuotationProducts").ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToQuotationDto(obj);
        }

        public async Task<Quotation> CreateQuotationAsync(QuotationDto quotation)
        {
            var newQuotation = new Quotation
            {
                QuotationNumber = $"RFQ-{DateTime.Now.ToLocalTime().Ticks}",
                ValidUntil = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)), // Default to 30 days from now
                Status = "PENDING",

                AwsSesMessageId = string.Empty, // This will be set after sending the email
                DateSent = DateTime.Now.AddYears(100).ToLocalTime(),   // This will be set after sending the email

                FinalPrice = quotation.FinalPrice,
                FarmId = quotation.FarmId,
                RecipientEmail = quotation.RecipientEmail,
                RecipientName = quotation.RecipientName,
                BasePrice = quotation.BasePrice,
                PricingProfileId = quotation.PricingProfileId,
                //QuotationDetails = quotation.QuotationDetails.Select(qd => new QuotationDetail
                //{
                //    After = qd.After,
                //    Before = qd.Before,
                //    Delta = qd.Delta,
                //    IsDeduction = qd.IsDeduction,
                //    ConditionType = qd.ConditionType,
                //    ApplyType = qd.ApplyType,
                //    Value = qd.Value,
                //    Sequence = qd.Sequence,
                //}).ToList()
                QuotationProducts = quotation.QuotationProducts.Select(x => new QuotationProduct
                {
                    ProductId = x.ProductId,
                    ProductName = x.ProductName,
                    ProductTypeId = x.ProductTypeId,
                    Quantity = x.Quantity,
                    QuotationId = x.QuotationId,
                    Total = x.Total,
                    UnitId = x.UnitId,
                    UnitPrice = x.UnitPrice
                }).ToList()
            };

            try
            {
                var createdQuotation = await _unitOfWork.Quotations.AddAsync(newQuotation).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return createdQuotation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public async Task<string> SendQuotationEmailAsync(SendQuotationParamsModel paramsModel)
        {
            if (paramsModel.BasePrice <= 0)
                throw new ArgumentException("Base price must be greater than zero.", nameof(paramsModel.BasePrice));

            //if (paramsModel.PricingProfileId == Guid.Empty)
            //    throw new ArgumentException("Pricing profile ID cannot be empty.", nameof(paramsModel.PricingProfileId));

            if (paramsModel.CustomerId == Guid.Empty)
                throw new ArgumentException("Customer ID cannot be empty.", nameof(paramsModel.CustomerId));

            // Ensure the customer exists
            var customer = await _unitOfWork.Customers.GetByIdAsync(paramsModel.CustomerId).ConfigureAwait(false);
            if (customer == null)
                throw new KeyNotFoundException("Customer not found.");

            if (string.IsNullOrWhiteSpace(customer.EmailAddress))
                throw new ArgumentException("Customer email address is not valid.", nameof(paramsModel.CustomerId));

            //// Fetch the pricing profile and ensure it exists
            //var pricing = await _unitOfWork.PricingProfiles.GetByIdAsync(paramsModel.PricingProfileId, "Farm", "PricingConditions").ConfigureAwait(false);
            //if (pricing == null)
            //    throw new KeyNotFoundException("Pricing profile not found.");

            //var finalPriceResult = await _pricingService.CalculateFinalPriceAsync(paramsModel.PricingProfileId, paramsModel.BasePrice).ConfigureAwait(false);

            var model = new QuotationDto
            {
                //FinalPrice = finalPriceResult.FinalPrice,
                //FarmName = pricing.Farm.FarmName,
                //FarmId = pricing.FarmId,
                //PricingProfileId = pricing.Id,

                FinalPrice = paramsModel.FinalPrice,
                FarmName = paramsModel.FarmName,
                FarmId = paramsModel.FarmId,
                QuotationNumber = $"RFQ-{DateTime.Now.ToLocalTime().Ticks}",
                RecipientName = customer.CustomerName,
                ValidUntil = DateOnly.FromDateTime(DateTime.Now.ToLocalTime().AddDays(30)),
                BasePrice = paramsModel.BasePrice,
                RecipientEmail = paramsModel.CustomerEmail,
                Status = "NEW",
                QuotationProducts = paramsModel.Products.ToList()
            };

            return await _emailService.SendQuotationEmailAsync(model).ConfigureAwait(false);
        }

        public async Task CreateAndSendQuotationEmailAsync(SendQuotationParamsModel paramsModel)
        {
            //var pricing = await _pricingProfileService.GetPricingProfileByIdAsync(paramsModel.PricingProfileId).ConfigureAwait(false);
            //if (pricing == null)
            //{
            //    throw new KeyNotFoundException();
            //}

            var quotation = new QuotationDto
            {
                AwsSesMessageId = null,
                //DateSent = 

                QuotationNumber = $"RFQ-{DateTime.Now.ToLocalTime().Ticks}",
                Status = "NEW",

                ValidUntil = DateOnly.FromDateTime(DateTime.Now.ToLocalTime().AddDays(30)),

                RecipientEmail = paramsModel.CustomerEmail,
                RecipientName = paramsModel.CustomerName,
                FinalPrice = paramsModel.FinalPrice,

                BasePrice = paramsModel.BasePrice,
                FarmId = paramsModel.FarmId,
                FarmName = paramsModel.FarmName,
                //FarmId = pricing.FarmId,
                //PricingProfileId = pricing.ProfileId,

                QuotationProducts = paramsModel.Products.ToList()
            };

            var newQuotation = await CreateQuotationAsync(quotation).ConfigureAwait(false);
            if (newQuotation != null)
            {
                string messageId = await _emailService.SendQuotationEmailAsync(quotation).ConfigureAwait(false);
                if (messageId != null)
                {
                    newQuotation.DateSent = DateTime.Now.ToLocalTime();
                    newQuotation.AwsSesMessageId = messageId;
                    await UpdateQuotationMessageIdAsync(newQuotation).ConfigureAwait(false);
                }
            }
            else
                _logger.LogWarning($"Unable to create quotation.");
        }

        public async Task UpdateQuotationMessageIdAsync(Quotation quotation)
        {
            try
            {
                await _unitOfWork.Quotations.UpdateQuotationMessageIdAsync(quotation).ConfigureAwait(false);
                await _unitOfWork.CommitAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}
