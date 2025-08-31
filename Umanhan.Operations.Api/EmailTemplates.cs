using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;

namespace Umanhan.Operations.Api
{
    public static class EmailTemplates
    {
        public const string TEMPLATE_WELCOME = "WelcomeTemplate";
        public const string TEMPLATE_QUOTATION = "QuotationTemplate";

        public static async Task InitializeTemplates(IAmazonSimpleEmailService emailService)
        {
            var welcomeTemplate = new CreateTemplateRequest
            {
                Template = new Template
                {
                    TemplateName = "WelcomeTemplate",
                    SubjectPart = "Welcome to Umanhan FMS",
                    TextPart = """
                        Howdy, {{username}}!

                        We're thrilled to have you onboard. You can now manage your farm operations more efficiently.

                        Log in to your dashboard and start exploring the tools we've prepared for you.


                        Best regards,

                        The Farmer
                    """,
                    HtmlPart = """
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
                                <p><strong>The Farmer</strong></p>
                            </div>
                        </body>
                        </html>
                    """
                }
            };


            var quotationTemplate = new CreateTemplateRequest
            {
                Template = new Template
                {
                    TemplateName = "QuotationTemplate",
                    SubjectPart = "Quotation - {{RfqNumber}}",
                    HtmlPart = """
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
                                        <th>#</th>
                                        <th>Condition</th>
                                        <th>Value</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {{#each Breakdown}}
                                    <tr>
                                        <td>{{this.ProductName}}</td>
                                        <td>{{this.Quantity}} {{this.Unit}}</td>
                                        <td>{{this.UnitPrice}}</td>
                                        <td>{{this.Total}}</td>
                                    </tr>
                                    {{/each}}
                                    <tr>
                                        <td colspan="3" style="text-align:right;">Total</td>
                                        <td><strong>₱{{this.FinalPrice}}</strong></td>
                                    </tr>
                                </tbody>
                            </table>

                            <p>Thank you for your interest. Please reply to confirm or inquire further.</p>
                            <br/><p>Best regards,<br/><br/>The Sales Team</p>
                        </body>
                        </html>
                    """,
                    TextPart = """
                        Quotation - {{RfqNumber}}

                        Howdy, {{CustomerName}}!

                        Please find below your quotation from {{FarmName}}.
                        Valid until: {{ValidUntil}}

                        --- Breakdown ---
                        {{#each Breakdown}}
                            {{this.ProductName}}/{{this.Quantity}} {{this.Unit}}/{{this.UnitPrice}}/{{this.Total}}
                        {{/each}}
                        Total: ₱{{FinalPrice}}

                        Thank you for your interest.
                        Best regards,
                        The Farm Sales Team
                    """
                }
            };


            try
            {
                await emailService.DeleteTemplateAsync(new DeleteTemplateRequest { TemplateName = TEMPLATE_WELCOME });
            }
            catch { }

            try
            {
                await emailService.DeleteTemplateAsync(new DeleteTemplateRequest { TemplateName = TEMPLATE_QUOTATION });
            }
            catch { }

            await emailService.CreateTemplateAsync(welcomeTemplate);
            await emailService.CreateTemplateAsync(quotationTemplate);
        }
    }
}
