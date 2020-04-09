using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;

namespace ProductScraper.Functions
{
    public static class TestEmail
    {
        [FunctionName("TestEmail")]
        public static void Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [SendGrid(ApiKey = "SendGridApiKey")] out SendGridMessage message,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            message = new SendGridMessage();
            message.AddTo("stevcooo@gmail.com");
            message.AddContent("text/html", "Test content");
            message.SetFrom(new EmailAddress("stevan@kostoski.com"));
            message.SetSubject("Some subject");
        }
    }
}
