using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using ProductScraper.Common.Naming;
using SendGrid.Helpers.Mail;

namespace ProductScraper.Functions.EmailNotificationsFunctions
{
    public static class EmailSender
    {
        [FunctionName(FunctionName.EmailSender)]
        public static void Run(
            [QueueTrigger(QueueName.EmailsToSend, Connection = CommonName.Connection)]SendGridMessage queuedMessage,
            [SendGrid(ApiKey = "SendGridApiKey")] out SendGridMessage message,
            ILogger log)
        {
            log.LogInformation($"Started sending message");
            message = queuedMessage;
        }
    }
}
