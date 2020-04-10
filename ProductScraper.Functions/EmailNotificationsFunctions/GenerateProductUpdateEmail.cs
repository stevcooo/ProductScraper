using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using ProductScraper.Models.ViewModels;
using SendGrid.Helpers.Mail;
using System.Linq;

namespace ProductScraper.Functions.EmailNotificationsFunctions
{
    public static class GenerateProductUpdateEmail
    {
        //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
        [FunctionName("GenerateProductUpdateEmail")]
        public static async void Run(
            [QueueTrigger("ProductUpdateEmailNotifications", Connection = "AzureWebJobsStorage")]EmailMessage emailMessage,
            [Queue("EmailsToSend")] IAsyncCollector<SendGridMessage> emailMessageQueue,
            IBinder binder,
            ILogger log)
        {
            
            log.LogInformation($"C# Queue trigger function for email sending to user: {emailMessage.To}");

            var usersTable = await binder.BindAsync<CloudTable>(new TableAttribute("v3AspNetUsers")
            {
                Connection = "AzureWebJobsStorage"
            });

            var userEmailQuery = new TableQuery<User>().Where(
                TableQuery.GenerateFilterCondition("Id", QueryComparisons.Equal, emailMessage.To));
            var users = await usersTable.ExecuteQuerySegmentedAsync(userEmailQuery, null);
            var user = users.Results.FirstOrDefault();
            if (user != null)
            {
                
                var message = new SendGridMessage();
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
