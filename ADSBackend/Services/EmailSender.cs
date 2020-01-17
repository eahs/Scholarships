using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Scholarships.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713

    public class EmailSender : IEmailSender
    {
        private Services.Configuration Configuration { get; }

        public EmailSender(Services.Configuration configuration)
        {
            Configuration = configuration;
        }

        public SmtpClient ConfigureClient ()
        {
            // https://support.google.com/a/answer/176600?hl=en
            System.Net.NetworkCredential basicauthenticationinfo = new System.Net.NetworkCredential(Configuration.Get("ApplicationEmail"), Configuration.Get("ApplicationEmailPassword"));

            // https://myaccount.google.com/u/3/lesssecureapps?pli=1&pageId=none
            SmtpClient client = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = basicauthenticationinfo
            };

            return client;
        }

        private static MailMessage ConfigureMessage (string email, string subject, string message )
        {
            MailMessage mailMessage = new MailMessage
            {
                IsBodyHtml = true,
                From = new MailAddress("scholarship@eastonsd.org", "Scholarships"),
                Body = message,
                Subject = subject,
            };
            mailMessage.To.Add(email);

            return mailMessage;
        }

        public void SendEmail(string email, string subject, string message)
        {
            SmtpClient client = ConfigureClient();
            MailMessage mailMessage = ConfigureMessage(email, subject, message);

            client.Send(mailMessage);
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            SmtpClient client = ConfigureClient();
            MailMessage mailMessage = ConfigureMessage(email, subject, message);

            return client.SendMailAsync(mailMessage);
        }
    }
}
