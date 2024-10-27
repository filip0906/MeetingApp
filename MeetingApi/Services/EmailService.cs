using System.Globalization;
using System.Net.Mail;
using System.Threading.Tasks;
using MeetingApi.Models;

namespace MeetingApi.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpClient _smtpClient;

        public EmailService(SmtpClient smtpClient)
        {
            _smtpClient = smtpClient;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                var mailMessage = new MailMessage("filip0906@gmail.com", to, subject, body);
                await _smtpClient.SendMailAsync(mailMessage);
                Console.WriteLine($"Email successfully sent to {to}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email to {to}: {ex.Message}");
                throw;
            }
        }

        public async Task SendEmailAsync(string to, string subject, string body, Meeting meeting)
        {
            try
            {
                var fullBody = $"{body}<br/><br/>";

                var mailMessage = new MailMessage("filip0906@gmail.com", to, subject, fullBody)
                {
                    IsBodyHtml = true // Omogućava HTML sadržaj u e-mailu
                };

                await _smtpClient.SendMailAsync(mailMessage);
                Console.WriteLine($"Email successfully sent to {to}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email to {to}: {ex.Message}");
                throw;
            }
        }
    }

}
