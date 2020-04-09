using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using ProductScraper.Models.EntityModels;
using System;

namespace ProductScraper.Functions
{
    public static class CheckForUsersReadyForEmailNotification
    {
        [FunctionName("CheckForUsersReadyForEmailNotification")]
        //"0 0 */2 * * *"	once every two hours
        public static async void Run([TimerTrigger("0 0 */2 * * *")]TimerInfo myTimer,
            [Table("UserProfile")] CloudTable userProfileTable,
            [Queue("usersReadyForNotifications")] IAsyncCollector<UserProfile> usersReadyForNotificationsQueue,
            ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var userProfilesQuery = new TableQuery<UserProfile>();
            var users = await userProfileTable.ExecuteQuerySegmentedAsync(userProfilesQuery, null);            
            foreach (var user in users)
            {
                var timeSpan = DateTime.UtcNow - user.LastNotificationEmailSendOn;
                if (timeSpan.Days >= user.DaysBetweenEmailNotifications)
                {
                    log.LogInformation($"User {user.Id} is ready for notifications.");

                    //Update user info
                    user.LastNotificationEmailSendOn = DateTime.UtcNow;                    
                    var operation = TableOperation.InsertOrReplace(user);
                    await userProfileTable.ExecuteAsync(operation);

                    //Send notification to queue
                    await usersReadyForNotificationsQueue.AddAsync(user);                    
                }
            }
        }
    }
}