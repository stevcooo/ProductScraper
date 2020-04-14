using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ProductScraper.Common.Naming;
using ProductScraper.Models.ViewModels;
using SendGrid.Helpers.Mail;
using System.IO;
using System.Threading.Tasks;

namespace ProductScraper.Functions.Common
{
    public static class EmailSender
    {
        [FunctionName(FunctionName.EmailSender)]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [Queue(QueueName.EmailsToSend)] IAsyncCollector<SendGridMessage> emailMessageQueue,
            ILogger log)
        {
            log.LogInformation("EmailSender trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            EmailMessage emailMessage = JsonConvert.DeserializeObject<EmailMessage>(requestBody);

            if (emailMessage == null)
            {
                return new NoContentResult();
            }

            SendGridMessage message = new SendGridMessage();
            message.AddTo(emailMessage.To);
            message.AddContent("text/html", emailMessage.Content);
            message.SetFrom(new EmailAddress("stevan@kostoski.com"));
            message.SetSubject(emailMessage.Subject);
            await emailMessageQueue.AddAsync(message);
            log.LogInformation($"Enqueued email message to : {emailMessage.To}");
            return new OkResult();
        }
    }
}
