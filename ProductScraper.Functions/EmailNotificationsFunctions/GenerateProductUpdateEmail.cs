using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using ProductScraper.Common.Naming;
using ProductScraper.Models.ViewModels;
using SendGrid.Helpers.Mail;
using System.Linq;

namespace ProductScraper.Functions.EmailNotificationsFunctions
{
    public static class GenerateProductUpdateEmail
    {
        [FunctionName(FunctionName.GenerateProductUpdateEmail)]
        public static async void Run(
            [QueueTrigger(QueueName.ProductUpdateEmailNotifications, Connection = CommonName.Connection)]EmailMessage emailMessage,
            [Queue(QueueName.EmailsToSend)] IAsyncCollector<SendGridMessage> emailMessageQueue,
            IBinder binder,
            ILogger log)
        {

            log.LogInformation($"C# Queue trigger function for email sending to user: {emailMessage.To}");

            CloudTable usersTable = await binder.BindAsync<CloudTable>(new TableAttribute("v3AspNetUsers")
            {
                Connection = CommonName.Connection
            });

            TableQuery<User> userEmailQuery = new TableQuery<User>().Where(
                TableQuery.GenerateFilterCondition("Id", QueryComparisons.Equal, emailMessage.To));
            TableQuerySegment<User> users = await usersTable.ExecuteQuerySegmentedAsync(userEmailQuery, null);
            User user = users.Results.FirstOrDefault();
            if (user != null)
            {

                SendGridMessage message = new SendGridMessage();
                message.AddTo(user.Email);
                message.AddContent("text/html", emailMessage.Content);
                message.SetFrom(new EmailAddress("stevan@kostoski.com"));
                message.SetSubject(emailMessage.Subject);
                await emailMessageQueue.AddAsync(message);
                log.LogInformation($"Enqueued email message to : {user.Email}");
            }
        }
    }


    public class User : TableEntity
    {
        public string Id { get; set; }
        public string Email { get; set; }
    }
}
