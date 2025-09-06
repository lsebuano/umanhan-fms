using Umanhan.Dtos;

namespace Umanhan.Services.Interfaces
{
    public interface IEmailService
    {
        Task<string> SendQuotationEmailAsync(QuotationDto model);
        Task<string> SendWelcomeMessageEmailAsync(string toEmail, string username);
    }
}
