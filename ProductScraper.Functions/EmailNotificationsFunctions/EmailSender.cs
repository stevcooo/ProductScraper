using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using ProductScraper.Common;
using SendGrid.Helpers.Mail;

namespace ProductScraper.Functions.EmailNotificationsFunctions
{
    public static class EmailSender
    {
        [FunctionName(FunctionsNames.EmailSender)]
        public static void Run(
            [QueueTrigger("EmailsToSend", Connection = "AzureWebJobsStorage")]SendGridMessage queuedMessage,
            [SendGrid(ApiKey = "SendGridApiKey")] out SendGridMessage message,
            ILogger log)
        {
            log.LogInformation($"Started sending message");
            message = queuedMessage;
        }
    }
}
