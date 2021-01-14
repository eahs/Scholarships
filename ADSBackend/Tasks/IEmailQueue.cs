namespace Scholarships.Tasks
{
    interface IEmailQueue
    {
        void SendMail(string to, string subject, string message);
        void SendMailUsingTemplate(string to, string subject, string template, object[] data);

    }
}
