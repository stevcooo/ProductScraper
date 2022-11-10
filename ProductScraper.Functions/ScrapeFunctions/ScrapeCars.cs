using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using ProductScraper.Common.Naming;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.WindowsAzure.Storage.Table;
using ProductScraper.Models.EntityModels;
using ScrapySharp.Extensions;
using ScrapySharp.Network;

namespace ProductScraper.Functions.EmailNotificationsFunctions
{
    public static class ScrapeCars
    {
        [FunctionName(FunctionName.ScrapeCars)]
        public static async void Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = FunctionName.ScrapeCars)] HttpRequest req,
            [Table(TableName.Ads)] CloudTable adsTable,
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
                foreach (var ad in adsFromWeb)
                {
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
                log.LogInformation("Email will be send with : {Count}", adsFromWeb.Count);
            } else 
                log.LogInformation("There are no new ads!");
            
        }
    }
}