using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using ProductScraper.Common.Naming;
using SendGrid.Helpers.Mail;

namespace ProductScraper.Functions.Common
{
    public static class EmailToSendGrid
    {
        [FunctionName(FunctionName.EmailToSendGrid)]
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
