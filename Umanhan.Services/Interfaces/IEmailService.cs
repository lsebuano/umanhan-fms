using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Dtos;

namespace Umanhan.Services.Interfaces
{
    public interface IEmailService
    {
        Task<string> SendQuotationEmailAsync(QuotationDto model);
        Task<string> SendWelcomeMessageEmailAsync(string toEmail, string username);
    }
}
