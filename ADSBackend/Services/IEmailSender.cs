using System.Net.Mail;
using System.Threading.Tasks;

namespace Scholarships.Services
{
    public interface IEmailSender
    {
        SmtpClient ConfigureClient();
        Task SendEmailAsync(string email, string subject, string message);
        void SendEmail(string email, string subject, string message);

    }
}
