using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace RentCarSystem.Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to,string subject,string body);
        Task SendVerificationEmailAsync(string to, string token);
        Task SendPasswordResetEmailAsync(string to, string token);
      
        Task SendWelcomeEmailAsync(string to, string userName);
    }
}
