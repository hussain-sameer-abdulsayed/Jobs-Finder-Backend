using MB_Project.Models;
using MB_Project.Models.DTOS;

namespace MB_Project.IRepos
{
    public interface IEmailService
    {
        /*
        Task SendEmailAsync(string toEmail, string subject, string body, bool isBodyHTML);
        Task SendConfirmationEmail(EmailDto request);
        */
        void SendValidationEmail(EmailDto request);
    }
}
