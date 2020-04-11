using HtmlAgilityPack;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using ProductScraper.Common;
using ProductScraper.Models.EntityModels;
using ProductScraper.Models.Extensions;
using ProductScraper.Models.ViewModels;
using System;
using System.Linq;
using System.Net;
using System.Text;

namespace ProductScraper.Functions.ScrapeFunctions
{
    public static class ScrapeUserProducts
    {
        static WebClient _webClient = new WebClient();

        [FunctionName(FunctionsNames.ScrapeUserProducts)]
        public static async void Run(
            [QueueTrigger("usersReadyForNotifications", Connection = "AzureWebJobsStorage")]UserProfile userProfile,
            [Queue("ProductUpdateEmailNotifications")] IAsyncCollector<EmailMessage> emailMessageQueue,
            IBinder binder,
            ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {userProfile.FirstName}");
            
            var productInfoTable = await binder.BindAsync<CloudTable>(new TableAttribute("ProductInfo", userProfile.UserId)
            {
                Connection = "AzureWebJobsStorage"
            });

            var scrapeConfigTable = await binder.BindAsync<CloudTable>(new TableAttribute("ScrapeConfig")
            {
                Connection = "AzureWebJobsStorage"
            });

            var productQuery = new TableQuery<ProductInfo>();
            var userProducts = await productInfoTable.ExecuteQuerySegmentedAsync(productQuery, null);

            //Load all configs in the begginign/maybe this should be changed in the future
            var configsQuery = new TableQuery<ScrapeConfig>();
            var allConfigs = await scrapeConfigTable.ExecuteQuerySegmentedAsync(configsQuery, null);

            EmailMessage emailMessage;
            StringBuilder emailBodyBoulder = new StringBuilder();
            foreach (var product in userProducts)
            {
                //Find config from allConfigs
                var config = allConfigs.FirstOrDefault(t => t.PartitionKey.Equals(product.URL.ToCoreUrl()));

                if (config != null)
                {
                    Scrape(config, product, log);
                    //Update product in db
                    var operation = TableOperation.InsertOrReplace(product);
                    await productInfoTable.ExecuteAsync(operation);
                    if (product.HasChangesSinceLastTime)
                    {
                        //Add to Email
                        emailBodyBoulder.AppendLine($"{product.Name} Price: {product.Price} / {product.SecondPrice} Availability: {product.Availability} CheckedOn: {product.LastCheckedOn}");                        
                    }
                }
                else
                {
                    log.LogInformation($"Multiple scrape config matches the criteria URL={product.URL}");
                }
            }
            if (emailBodyBoulder.Length > 0)
            {
                emailMessage = new EmailMessage(userProfile.UserId, "Products updates", emailBodyBoulder.ToString());
                await emailMessageQueue.AddAsync(emailMessage);
            }
            else if(userProfile.SendEmailWhenNoProductHasBeenChanged)
            {
                emailMessage = new EmailMessage(userProfile.UserId, "Products updates", "None of your products has been updated/changed since last check.");
                await emailMessageQueue.AddAsync(emailMessage);
            }
        }

        private static void Scrape(ScrapeConfig scrapeConfig, ProductInfo product, ILogger log)
        {
            if (string.IsNullOrWhiteSpace(product.URL))
            {
                log.LogInformation("URL can not be empty!");
                return;
            }

            string html = _webClient.DownloadString(product.URL);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            product.HasChangesSinceLastTime = false;

            try
            {
                var titleNode = doc.DocumentNode.SelectSingleNode(scrapeConfig.ProductNamePath);
                if (titleNode != null && product.Name != titleNode.InnerText)
                {
                    product.HasChangesSinceLastTime = true;
                    product.Name = titleNode.InnerText;
                }
            }
            catch (Exception ex)
            {
                log.LogInformation(ex.Message);
            }

            try
            {
                var priceNode = doc.DocumentNode.SelectSingleNode(scrapeConfig.ProductPricePath);
                if (priceNode != null && product.Price != priceNode.InnerText)
                {
                    product.HasChangesSinceLastTime = true;
                    product.Price = priceNode.InnerText;
                }
            }
            catch (Exception ex)
            {
                log.LogInformation(ex.Message);
            }


            try
            {
                var secondPriceNode = doc.DocumentNode.SelectSingleNode(scrapeConfig.ProductSecondPricePath);
                if (secondPriceNode != null && product.SecondPrice != secondPriceNode.InnerText)
                {
                    product.HasChangesSinceLastTime = true;
                    product.SecondPrice = secondPriceNode.InnerText;
                }
            }
            catch (Exception ex)
            {
                log.LogInformation(ex.Message);
            }

            try
            {
                var availabilityNode = doc.DocumentNode.SelectSingleNode(scrapeConfig.ProductAvailabilityPath);
                if (availabilityNode != null)
                {
                    bool isAviliable = false;

                    if (scrapeConfig.ProductAvailabilityIsAtributeValue)
                    {
                        var attr = availabilityNode.Attributes.FirstOrDefault(t => t.Value == scrapeConfig.ProductAvailabilityValue);
                        if (attr != null)
                        {
                            isAviliable = true;
                        }
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(scrapeConfig.ProductAvailabilityValue) && availabilityNode.InnerText == scrapeConfig.ProductAvailabilityValue)
                            isAviliable = true;
                        else
                        {
                            isAviliable = availabilityNode != null;
                        }

                    }

                    if (product.Availability != isAviliable)
                    {
                        product.HasChangesSinceLastTime = true;
                        product.Availability = isAviliable;
                    }
                }
                else
                    product.Availability = null;

            }
            catch (Exception ex)
            {
                //Log the exception
                log.LogInformation(ex.Message);
            }
            product.LastCheckedOn = DateTime.UtcNow;
        }
    }
}
