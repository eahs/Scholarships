using System.Threading.Tasks;

namespace Scholarships.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
