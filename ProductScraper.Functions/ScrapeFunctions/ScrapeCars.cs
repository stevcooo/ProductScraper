using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using ProductScraper.Common.Naming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.Storage.Table;
using ProductScraper.Models.EntityModels;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using SendGrid.Helpers.Mail;

namespace ProductScraper.Functions.EmailNotificationsFunctions
{
    public static class ScrapeCars
    {
        [FunctionName(FunctionName.ScrapeCars)]
        public static async void Run([TimerTrigger("0 */2 * * * ")]TimerInfo timerInfo,
            [Table(TableName.Ads)] CloudTable adsTable,
            [Queue(QueueName.EmailsToSend)] IAsyncCollector<SendGridMessage> emailMessageQueue,
            ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            
            var browser = new ScrapingBrowser();
            var homePage = browser.NavigateToPage(new Uri("https://www.pazar3.mk/mk/TOYOTAAVTOCENTARSKOPJE"));
            var adsFromWebNodes = homePage.Html.CssSelect(".search-result > .store-ad-item-description").ToArray();
            var adsFromWeb = new Dictionary<string, string>();
            // generate links
            foreach (var ad in adsFromWebNodes)
            {
                var titleNode  = ad.CssSelect(".Link_vis").FirstOrDefault();
                if (titleNode != null)
                {
                    adsFromWeb.Add(titleNode.Attributes.FirstOrDefault(t => t.Name == "href")?.Value, titleNode.InnerText);
                }
            }
            // load links from DB
            var adsTableQuery = new TableQuery<Ad>();
            var existingAds = await adsTable.ExecuteQuerySegmentedAsync(adsTableQuery, null);
            foreach (var ad in existingAds)
            {
                // remove existing ads
                if (adsFromWeb.ContainsKey(ad.Link))
                    adsFromWeb.Remove(ad.Link);
            }
            // compare if any is new
            if (adsFromWeb.Any())
            {
                // Save new to db
                var emailBodyBuilder = new StringBuilder();
                emailBodyBuilder.AppendLine("These are the new ads from Toyota:");
                emailBodyBuilder.AppendLine("<br>");
                emailBodyBuilder.AppendLine("<br>");
                
                foreach (var ad in adsFromWeb)
                {
                    emailBodyBuilder.AppendLine($"<a href='https://www.pazar3.mk/${ad.Key}'>{ad.Value}</a>");
                    emailBodyBuilder.AppendLine("<br>");
                    var newAd = new Ad
                    {
                        Id = DateTime.Now.Ticks,
                        RowKey = DateTime.Now.Ticks.ToString(),
                        PartitionKey = DateTime.Now.Ticks.ToString(),
                        Title = ad.Value,
                        Link = ad.Key
                    };
                    await adsTable.ExecuteAsync(TableOperation.Insert(newAd));
                }
                // send email with new links
                var message = new SendGridMessage();
                message.AddTo("stevcooo@gmail.com");
                message.AddContent("text/html", emailBodyBuilder.ToString());
                message.SetFrom(new EmailAddress("stevan@kostoski.com"));
                message.SetSubject("Toyota ads");
                await emailMessageQueue.AddAsync(message);
            } else 
                log.LogInformation("There are no new ads!");
            
        }
    }
}