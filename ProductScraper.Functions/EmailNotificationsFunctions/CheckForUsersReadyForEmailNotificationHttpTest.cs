using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using ProductScraper.Common.Naming;
using ProductScraper.Models.EntityModels;
using System;

namespace ProductScraper.Functions.EmailNotificationsFunctions
{
    public static class CheckForUsersReadyForEmailNotificationHttpTest
    {
        [FunctionName("CheckForUsersReadyForEmailNotificationHttpTest")]
        //"0 0 */2 * * *"	once every two hours
        public static async void Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req,
            [Table(TableName.UserProfile)] CloudTable userProfileTable,
            [Queue(QueueName.UsersReadyForNotifications)] IAsyncCollector<UserProfile> usersReadyForNotificationsQueue,
            ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            TableQuery<UserProfile> userProfilesQuery = new TableQuery<UserProfile>();
            TableQuerySegment<UserProfile> users = await userProfileTable.ExecuteQuerySegmentedAsync(userProfilesQuery, null);
            foreach (UserProfile user in users)
            {
                if (!user.EnableEmailNotifications)
                {
                    continue;
                }

                TimeSpan timeSpan = DateTime.UtcNow - user.LastNotificationEmailSendOn;
                if (timeSpan.Days >= user.DaysBetweenEmailNotifications)
                {
                    log.LogInformation($"User {user.Id} is ready for notifications.");
                    //Send notification to queue
                    await usersReadyForNotificationsQueue.AddAsync(user);
                }
            }
        }
    }
}