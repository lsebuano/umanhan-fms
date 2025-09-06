using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using Umanhan.Dtos;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class SesEmailService : IEmailService
    {
        private readonly IAmazonSimpleEmailService _ses;
        private readonly ILogger<SesEmailService> _logger;
        private readonly string _fromEmail;

        public SesEmailService(IAmazonSimpleEmailService ses, 
            ILogger<SesEmailService> logger, 
            IConfiguration config)
        {
            _ses = ses;
            _logger = logger;
            _fromEmail = config["Email:From"];
        }

        public async Task<string> SendQuotationEmailAsync(QuotationDto model)
        {
            try
            {
                var destination = new Destination { ToAddresses = new List<string> { model.RecipientEmail } };
                var templateData = new
                {
                    RfqNumber = model.QuotationNumber,
                    CustomerName = model.RecipientName,
                    FarmName = model.FarmName,
                    ValidUntil = model.ValidUntil.ToString("MMMM dd, yyyy"),
                    BasePrice = model.BasePrice.ToString("N2"),
                    FinalPrice = model.FinalPrice.ToString("N2"),
                    Breakdown = model.QuotationProducts.Select(x => new
                    {
                        ProductName = x.ProductName,
                        Quantity = x.Quantity.ToString("n2"),
                        Unit = x.Unit,
                        UnitPrice = x.UnitPrice.ToString("n2"),
                        Total = x.Total.ToString("n2")
                    })
                };

                var sendTemplateRequest = new SendTemplatedEmailRequest
                {
                    Source = _fromEmail,
                    Destination = destination,
                    Template = "QuotationTemplate",
                    TemplateData = JsonSerializer.Serialize(templateData)
                };

                var response = await _ses.SendTemplatedEmailAsync(sendTemplateRequest);

                return response.HttpStatusCode == HttpStatusCode.OK ? response.MessageId : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending quotation to {model.RecipientEmail}");
                return null;
            }
        }

        public async Task<string> SendWelcomeMessageEmailAsync(string toEmail, string username)
        {
            try
            {
                var destination = new Destination { ToAddresses = new List<string> { toEmail } };
                var templateData = new { username };

                var sendTemplateRequest = new SendTemplatedEmailRequest
                {
                    Source = _fromEmail,
                    Destination = destination,
                    Template = "WelcomeTemplate",
                    TemplateData = JsonSerializer.Serialize(templateData)
                };

                var response = await _ses.SendTemplatedEmailAsync(sendTemplateRequest);
                return response.HttpStatusCode == HttpStatusCode.OK ? response.MessageId : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending welcome email to {toEmail}");
                return null;
            }
        }
    }

}
