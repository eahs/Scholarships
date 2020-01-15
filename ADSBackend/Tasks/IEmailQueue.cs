using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scholarships.Tasks
{
    interface IEmailQueue
    {
        void SendMail(string to, string subject, string message);
        void SendMailUsingTemplate(string to, string subject, string template, object[] data);
        
    }
}
