using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using ProductScraper.Models.EntityModels;
using ProductScraper.Models.Extensions;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ProductScraper.Functions
{
    public static class ScrapeProduct
    {
        [FunctionName("ScrapeProduct")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "ScrapeProduct/{userId}/{productId}")] HttpRequest req,
            [Table("ProductInfo", "{userId}")] CloudTable productInfoTable,
            [Table("ScrapeConfig")] CloudTable scrapeConfigTable,
            string userId,
            string productId,
            ILogger log)
        {
            log.LogInformation($"Request to scrape product {productId}");


            var productQuery = new TableQuery<ProductInfo>().Where(
                TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, productId));
            var products = await productInfoTable.ExecuteQuerySegmentedAsync(productQuery, null);
            if (products != null && products.Count() == 1)
            {
                //Find matching criteria
                var product = products.First();
                var configQuery = new TableQuery<ScrapeConfig>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, product.URL.ToCoreUrl()));
                var configs = await scrapeConfigTable.ExecuteQuerySegmentedAsync(configQuery, null);
                if (configs != null && configs.Count() == 1)
                {
                    Scrape(configs.First(), product, log);
                    
                    //Update product in db
                    TableOperation operation = TableOperation.InsertOrReplace(product);                    
                    await productInfoTable.ExecuteAsync(operation);
                    
                    if (product.HasChangesSinceLastTime)
                    {

                    }
                }
                else
                {
                    log.LogInformation($"Multiple scrape config matches the criteria URL={product.URL}");
                }
            }
            else
            {
                log.LogInformation($"Multiple products matches the criteria userId={userId} productId={productId}");
            }

            string responseMessage = string.IsNullOrEmpty(productId)
                ? "Please provide productId in the path" : $"Product {productId} has been scraped";
            return new OkObjectResult(responseMessage);
        }

        private static void Scrape(ScrapeConfig scrapeConfig, ProductInfo product, ILogger log)
        {
            if (string.IsNullOrWhiteSpace(product.URL))
            {
                log.LogInformation("URL can not be empty!");
                return;
            }

            var webClient = new WebClient();
            string html = webClient.DownloadString(product.URL);
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

            if (product.HasChangesSinceLastTime)
            {
                //Send email
            }
        }
    }
}
