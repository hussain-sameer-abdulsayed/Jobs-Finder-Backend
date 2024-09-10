using MB_Project.IRepos;
using Microsoft.AspNetCore.Identity;
using MimeKit;
using MB_Project.Models.DTOS;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace MB_Project.Repos
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager;

        public EmailService(IConfiguration configuration, UserManager<IdentityUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        

        public void SendValidationEmail(EmailDto request)
        {
            // Create a new MIME message
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_configuration.GetSection("EmailSettings:SenderName").Value!, _configuration.GetSection("EmailSettings:FromEmail").Value!)); // Sender
            message.To.Add(MailboxAddress.Parse(request.To)); // Recipient
            message.Subject = request.Subject;

            // Create the body part of the message (HTML)
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = request.Body;

            // Set the body of the message
            message.Body = bodyBuilder.ToMessageBody();

            // Connect to the SMTP server
            using var smtp = new SmtpClient();
            smtp.Connect(_configuration.GetSection("EmailSettings:MailServer").Value!, int.Parse(_configuration.GetSection("EmailSettings:MailPort").Value!), SecureSocketOptions.StartTls); // SMTP server, port, secure connection (false for TLS)
            smtp.Authenticate(_configuration.GetSection("EmailSettings:FromEmail").Value!, _configuration.GetSection("EmailSettings:Password").Value!); // Your email credentials

            // Send the message
            smtp.Send(message);

            // Disconnect from the SMTP server
            smtp.Disconnect(true);
        }



        /*
        public Task SendEmailAsync(string toEmail, string subject, string body, bool isBodyHTML)
        {
            string MailServer = _configuration["EmailSettings:MailServer"];
            string FromEmail = _configuration["EmailSettings:FromEmail"];
            string Password = _configuration["EmailSettings:Password"];
            int Port = int.Parse(_configuration["EmailSettings:MailPort"]);
            var client = new SmtpClient(MailServer, Port)
            {
                Credentials = new NetworkCredential(FromEmail, Password),
                EnableSsl = true
            };
            MailMessage mailMessage = new MailMessage(FromEmail, toEmail, subject, body)
            {
                IsBodyHtml = isBodyHTML
            };
            return client.SendMailAsync(mailMessage);
        }

        public async Task SendConfirmationEmail(string? email, User? user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = $"http://localhost:3000/confirm-email?UserId={user.Id}&Token={token}";
            await SendEmailAsync(email, "Confirm Your Email", $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>;.", true);
        }
        */
    }
}
