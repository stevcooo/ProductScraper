using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;

namespace ProductScraper.Functions
{
    public static class EmailSender
    {
        [FunctionName("EmailSender")]
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
