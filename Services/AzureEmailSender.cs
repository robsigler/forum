using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;
using Azure;
using Azure.Communication.Email;
using Microsoft.Extensions.Configuration;

namespace Forum.Services
{
    public class AzureEmailSender : IEmailSender
    {
        private readonly string _connectionString;
        
        public AzureEmailSender(string connectionString)
        {
            this._connectionString = connectionString;
        }
        
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new EmailClient(_connectionString);
            
            var sender = "DoNotReply@robsigler.com";

            var emailContent = new EmailContent(subject)
            {
                PlainText = htmlMessage
            };

            var toRecipients = new List<EmailAddress>()
            {
                new EmailAddress(email)
            };

            var emailRecipients = new EmailRecipients(toRecipients);

            var emailMessage = new EmailMessage(sender, emailRecipients, emailContent);

            return client.SendAsync(WaitUntil.Completed, emailMessage);
        }
    }
}