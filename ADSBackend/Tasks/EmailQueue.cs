using Scholarships.Data;
using Scholarships.Services;
using System;

namespace Scholarships.Tasks
{
    public class EmailQueue : IEmailQueue
    {
        private readonly ApplicationDbContext _context;
        private readonly Services.Configuration Configuration;
        private readonly IEmailSender Email;

        public EmailQueue(ApplicationDbContext context, Services.Configuration configurationService, IEmailSender email)
        {
            _context = context;
            Configuration = configurationService;
            Email = email;
        }

        public void SendMail(string to, string subject, string message)
        {
            try
            {
                Email.SendEmail(to, subject, message);
            }
            catch (Exception e)
            {
                // Error
                Serilog.Log.Logger.Error(e, "Unable to send email message to {0}", to);
            }

        }

        public void SendMailUsingTemplate(string to, string subject, string template, object[] data)
        {
            throw new NotImplementedException();
        }

    }
}
