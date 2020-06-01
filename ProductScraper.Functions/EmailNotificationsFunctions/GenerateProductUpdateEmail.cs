using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using ProductScraper.Common.Naming;
using ProductScraper.Models.EntityModels;
using ProductScraper.Models.ViewModels;
using SendGrid.Helpers.Mail;
using System;
using System.Linq;

namespace ProductScraper.Functions.EmailNotificationsFunctions
{
    public static class GenerateProductUpdateEmail
    {
        [FunctionName(FunctionName.GenerateProductUpdateEmail)]
        public static async void Run(
            [QueueTrigger(QueueName.ProductUpdateEmailNotifications, Connection = CommonName.Connection)]EmailMessage emailMessage,
            [Table(TableName.UserProfile)] CloudTable userProfileTable,
            [Queue(QueueName.EmailsToSend)] IAsyncCollector<SendGridMessage> emailMessageQueue,
            IBinder binder,
            ILogger log)
        {

            log.LogInformation($"C# Queue trigger function for email sending to user: {emailMessage.To}");

            CloudTable usersTable = await binder.BindAsync<CloudTable>(new TableAttribute(TableName.IdentityUsers)
            {
                Connection = CommonName.Connection
            });

            TableQuery<User> userEmailQuery = new TableQuery<User>().Where(TableQuery.GenerateFilterCondition("Id", QueryComparisons.Equal, emailMessage.UserId));
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

                //Update user info
                TableQuery<UserProfile> userProfileQuery = new TableQuery<UserProfile>().Where(TableQuery.GenerateFilterCondition("UserId", QueryComparisons.Equal, emailMessage.UserId));
                TableQuerySegment<UserProfile> userProfiles = await userProfileTable.ExecuteQuerySegmentedAsync(userProfileQuery, null);
                UserProfile userProfile = userProfiles.Results.FirstOrDefault();
                userProfile.LastNotificationEmailSendOn = DateTime.UtcNow;
                TableOperation operation = TableOperation.InsertOrReplace(user);

                await userProfileTable.ExecuteAsync(operation);
            }
        }
    }


    public class User : TableEntity
    {
        public string Id { get; set; }
        public string Email { get; set; }
    }
}
