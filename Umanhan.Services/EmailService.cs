using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Umanhan.Dtos;
using Task = System.Threading.Tasks.Task;

namespace Umanhan.Services
{
    public class EmailService
    {
        private readonly IAmazonSimpleEmailService _ses;
        private readonly string _fromEmail;
        private readonly ILogger<EmailService> _logger;

        public const string TEMPLATE_WELCOME = "WelcomeTemplate";
        public const string TEMPLATE_QUOTATION = "QuotationTemplate";

        public EmailService(IAmazonSimpleEmailService ses, 
            ILogger<EmailService> logger)
        {
            _logger = logger;
            _ses = ses;
            _fromEmail = Environment.GetEnvironmentVariable("AWS_SES_FROM_EMAIL")
                          ?? throw new InvalidOperationException("Environment variable 'AWS_SES_FROM_EMAIL' is not set.");
        }

        public async Task SendEmailAsync(
            string toEmail,
            string subject,
            string? htmlBody = null,
            string? textBody = null,
            List<string>? cc = null,
            List<string>? bcc = null)
        {
            if (string.IsNullOrWhiteSpace(htmlBody) && string.IsNullOrWhiteSpace(textBody))
                throw new ArgumentException("Either HTML body or text body must be provided.");

            var destination = new Destination
            {
                ToAddresses = new List<string> { toEmail },
                CcAddresses = cc ?? new List<string>(),
                BccAddresses = bcc ?? new List<string>()
            };

            var body = new Body();
            if (!string.IsNullOrWhiteSpace(htmlBody))
                body.Html = new Content { Charset = "UTF-8", Data = htmlBody };
            if (!string.IsNullOrWhiteSpace(textBody))
                body.Text = new Content { Charset = "UTF-8", Data = textBody };

            var sendRequest = new SendEmailRequest
            {
                Source = _fromEmail,
                Destination = destination,
                Message = new Message
                {
                    Subject = new Content(subject),
                    Body = body
                }
            };

            // fallback to basic email send
            var response = await _ses.SendEmailAsync(sendRequest).ConfigureAwait(false);
            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                _logger.LogError($"Failed to send templated email. Status code: {response.HttpStatusCode}, Message ID: {response.MessageId}");
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
                    Template = TEMPLATE_QUOTATION,
                    TemplateData = JsonSerializer.Serialize(templateData)
                };

                var response = await _ses.SendTemplatedEmailAsync(sendTemplateRequest);
                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                    return response.MessageId;
                else
                    _logger.LogError($"Failed to send templated email. Status code: {response.HttpStatusCode}, Message ID: {response.MessageId}");

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed sending quotation to {model.RecipientEmail}. ERROR: {ex.Message}");
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public async Task<string> SendWelcomeMessageEmailAsync(string toEmail, string username)
        {
            try
            {
                var destination = new Destination { ToAddresses = new List<string> { toEmail } };

                var templateData = new
                {
                    username = username
                };

                var sendTemplateRequest = new SendTemplatedEmailRequest
                {
                    Source = _fromEmail,
                    Destination = destination,
                    Template = TEMPLATE_WELCOME,
                    TemplateData = JsonSerializer.Serialize(templateData)
                };

                var response = await _ses.SendTemplatedEmailAsync(sendTemplateRequest);
                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                    return response.MessageId;
                else
                    _logger.LogError($"Failed to send templated email. Status code: {response.HttpStatusCode}, Message ID: {response.MessageId}");

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed sending quotation to {toEmail}. ERROR: {ex.Message}");
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }
    }
}