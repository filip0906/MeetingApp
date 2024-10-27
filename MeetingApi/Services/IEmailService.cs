using MeetingApi.Models;

namespace MeetingApi.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task SendEmailAsync(string to, string subject, string body, Meeting meeting); // Nova metoda
    }

}
