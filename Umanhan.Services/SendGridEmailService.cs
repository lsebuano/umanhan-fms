using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using Umanhan.Dtos;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class SendGridEmailService : IEmailService
    {
        private readonly ISendGridClient _sendGridClient;
        private readonly ILogger<SendGridEmailService> _logger;
        private readonly string _fromEmail;

        public SendGridEmailService(ISendGridClient sendGridClient, 
            ILogger<SendGridEmailService> logger)
        {
            _sendGridClient = sendGridClient;
            _logger = logger;
            _fromEmail = Environment.GetEnvironmentVariable("AWS_SES_FROM_EMAIL");
        }

        public async Task<string> SendQuotationEmailAsync(QuotationDto model)
        {
            try
            {
                var htmlContent = """
                        <html>
                        <head>
                            <style>
                                body { font-family: Arial, sans-serif; }
                                table { width: 100%; border-collapse: collapse; margin-top: 10px; }
                                th, td { border: 1px solid #ccc; padding: 8px; text-align: left; }
                                th { background-color: #f2f2f2; }
                            </style>
                        </head>
                        <body>
                            <p>Howdy, {{CustomerName}}!</p>

                            <p>Please find below your quotation from <strong>{{FarmName}}</strong>. 
                            This quotation is valid until <strong>{{ValidUntil}}</strong>.</p>

                            <table style="width:550px;">
                                <thead>
                                    <tr>
                                        <th>Product</th>
                                        <th>Quantity</th>
                                        <th>Total</th>
                                    </tr>
                                </thead>
                                <tbody>
                """;
                foreach (var item in model.QuotationProducts)
                {
                    htmlContent += """
                                    <tr>
                                        <td>{{ProductName}}</td>
                                        <td>{{Quantity}}{{this.Unit}} x {{UnitPrice}}</td>
                                        <td>{{Total}}</td>
                                    </tr>
                    """
                    .Replace("{{ProductName}}", item.ProductName)
                    .Replace("{{Quantity}}", item.Quantity.ToString("n1"))
                    .Replace("{{this.Unit}}", item.Unit)
                    .Replace("{{UnitPrice}}", item.UnitPrice.ToString("n2"))
                    .Replace("{{Total}}", item.Total.ToString("n2"));
                }

                htmlContent += """
                                    <tr>
                                        <td colspan="2" style="text-align:right;">Total</td>
                                        <td><strong>₱{{FinalPrice}}</strong></td>
                                    </tr>
                                </tbody>
                            </table>

                            <p>Thank you for your interest. Please reply to confirm or inquire further.</p>
                            <br/><p>Best regards,<br/><br/>The Sales Team</p>
                        </body>
                        </html>
                    """;

                htmlContent = htmlContent
                    .Replace("{{CustomerName}}", model.RecipientName)
                    .Replace("{{FarmName}}", model.FarmName)
                    .Replace("{{ValidUntil}}", model.ValidUntil.ToString("MMMM dd, yyyy"))
                    .Replace("{{FinalPrice}}", model.FinalPrice.ToString("N2"));

                var plainTextContent = """
                        Quotation - {{RfqNumber}}

                        Howdy, {{CustomerName}}!

                        Please find below your quotation from {{FarmName}}.
                        Valid until: {{ValidUntil}}

                        --- Breakdown ---

                """;
                foreach (var item in model.QuotationProducts)
                {
                    plainTextContent += """
                            {{ProductName}} {{Quantity}}{{Unit}}x{{UnitPrice}}={{Total}}

                    """
                    .Replace("{{ProductName}}", item.ProductName)
                    .Replace("{{Quantity}}", item.Quantity.ToString("n1"))
                    .Replace("{{Unit}}", item.Unit)
                    .Replace("{{UnitPrice}}", item.UnitPrice.ToString("n2"))
                    .Replace("{{Total}}", item.Total.ToString("n2"));
                }
                plainTextContent += """                    
                            Total: ₱{{FinalPrice}}

                        Thank you for your interest.

                        Best regards,
                        The Farm Sales Team
                    """;
                plainTextContent = plainTextContent
                    .Replace("{{RfqNumber}}", model.QuotationNumber)
                    .Replace("{{CustomerName}}", model.RecipientName)
                    .Replace("{{FarmName}}", model.FarmName)
                    .Replace("{{ValidUntil}}", model.ValidUntil.ToString("MMMM dd, yyyy"))
                    .Replace("{{FinalPrice}}", model.FinalPrice.ToString("N2"));

                string subject = $"Quotation #{model.QuotationNumber} for {model.RecipientName} - {model.FarmName}";
                var fromEmailObj = new EmailAddress(_fromEmail, "Umanhan FMS");
                var toEmailObj = new EmailAddress(model.RecipientEmail, model.RecipientName);
                var msg = MailHelper.CreateSingleEmail(fromEmailObj, toEmailObj, subject, plainTextContent, htmlContent);
                var response = await _sendGridClient.SendEmailAsync(msg);
                if (response.IsSuccessStatusCode)
                    return Guid.NewGuid().ToString(); // SendGrid doesn’t return a message ID
                else
                    _logger.LogError($"Failed to send quotation via SendGrid. StatusCode: {response.StatusCode}");

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending quotation to {model.RecipientEmail} via SendGrid.");
                return null;
            }
        }

        public async Task<string> SendWelcomeMessageEmailAsync(string toEmail, string username)
        {
            try
            {
                var subject = "Welcome to Umanhan FMS";
                var plainTextContent = """
                        Howdy, {{username}}!

                        We're thrilled to have you onboard. You can now manage your farm operations more efficiently.

                        Log in to your dashboard and start exploring the tools we've prepared for you.


                        Best regards,

                        The Farm
                    """
                    .Replace("{{username}}", username);

                var htmlContent = """
                        <html>
                        <head>
                            <style>
                                body { font-family: Arial, sans-serif; }
                                .content { max-width: 600px; margin: auto; padding: 20px; }
                            </style>
                        </head>
                        <body>
                            <div class="content">
                                <h2>Howdy, {{username}}!</h2>
                                <p>We're thrilled to have you onboard.</p>
                                <p>You can now manage your farm operations more efficiently.</p>
                                <p>Log in to your dashboard and start exploring the tools we've prepared for you.</p>
                                <p>Best regards,</p>
                                <p><strong>The Farm</strong></p>
                            </div>
                        </body>
                        </html>
                    """
                   .Replace("{{username}}", username);

                var fromEmailObj = new EmailAddress(_fromEmail, "Umanhan FMS");
                var toEmailObj = new EmailAddress(toEmail, username);
                var msg = MailHelper.CreateSingleEmail(fromEmailObj, toEmailObj, subject, plainTextContent, htmlContent);
                var response = await _sendGridClient.SendEmailAsync(msg);
                return response.IsSuccessStatusCode ? Guid.NewGuid().ToString() : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending welcome email to {toEmail} via SendGrid.");
                return null;
            }
        }
    }

}
