using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using ProductScraper.Common.Naming;
using ProductScraper.Models.EntityModels;
using System;

namespace ProductScraper.Functions.EmailNotificationsFunctions
{
    public static class CheckForUsersReadyForEmailNotification
    {
        [FunctionName(FunctionName.CheckForUsersReadyForEmailNotification)]
        //"0 0 */2 * * *"	once every two hours
        public static async void Run([TimerTrigger("0 0 */2 * * *")]TimerInfo timerInfo,
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

                    //Update user info
                    user.LastNotificationEmailSendOn = DateTime.UtcNow;
                    TableOperation operation = TableOperation.InsertOrReplace(user);
                    await userProfileTable.ExecuteAsync(operation);

                    //Send notification to queue
                    await usersReadyForNotificationsQueue.AddAsync(user);
                }
            }
        }
    }
}