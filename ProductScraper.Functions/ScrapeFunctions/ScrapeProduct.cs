using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using ProductScraper.Common.Naming;
using ProductScraper.Models.EntityModels;
using ProductScraper.Models.Extensions;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ProductScraper.Functions.ScrapeFunctions
{
    public static class ScrapeProduct
    {
        private static readonly WebClient _webClient = new WebClient();

        [FunctionName(FunctionName.ScrapeProduct)]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = FunctionName.ScrapeProduct + "/{userId}/{productId}")] HttpRequest req,
            [Table(TableName.ProductInfo, "{userId}")] CloudTable productInfoTable,
            [Table(TableName.ScrapeConfig)] CloudTable scrapeConfigTable,
            [Queue(QueueName.ChnagedProducts)] IAsyncCollector<ProductInfo> changedProductQueue,
            string userId,
            string productId,
            ILogger log)
        {
            log.LogInformation($"Request to scrape product {productId}");

            TableQuery<ProductInfo> productQuery = new TableQuery<ProductInfo>().Where(
                TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, productId));
            TableQuerySegment<ProductInfo> products = await productInfoTable.ExecuteQuerySegmentedAsync(productQuery, null);
            if (products != null && products.Count() == 1)
            {
                //Find matching criteria
                ProductInfo product = products.First();
                TableQuery<ScrapeConfig> configQuery = new TableQuery<ScrapeConfig>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, product.URL.ToCoreUrl()));

                TableQuerySegment<ScrapeConfig> configs = await scrapeConfigTable.ExecuteQuerySegmentedAsync(configQuery, null);
                if (configs != null && configs.Count() == 1)
                {
                    await Scrape(configs.First(), product, log);

                    //Update product in db
                    TableOperation operation = TableOperation.InsertOrReplace(product);
                    await productInfoTable.ExecuteAsync(operation);

                    if (product.HasChangesSinceLastTime)
                    {
                        await changedProductQueue.AddAsync(product);
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

        private static async Task Scrape(ScrapeConfig scrapeConfig, ProductInfo product, ILogger log)
        {
            if (string.IsNullOrWhiteSpace(product.URL))
            {
                log.LogInformation("URL can not be empty!");
                return;
            }

            string html = await _webClient.DownloadStringTaskAsync(product.URL);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            product.HasChangesSinceLastTime = false;

            try
            {
                HtmlNode titleNode = doc.DocumentNode.SelectSingleNode(scrapeConfig.ProductNamePath);
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
                HtmlNode priceNode = doc.DocumentNode.SelectSingleNode(scrapeConfig.ProductPricePath);
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
                HtmlNode secondPriceNode = doc.DocumentNode.SelectSingleNode(scrapeConfig.ProductSecondPricePath);
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
                HtmlNode availabilityNode = doc.DocumentNode.SelectSingleNode(scrapeConfig.ProductAvailabilityPath);
                if (availabilityNode != null)
                {
                    bool isAviliable = false;

                    if (scrapeConfig.ProductAvailabilityIsAtributeValue)
                    {
                        HtmlAttribute attr = availabilityNode.Attributes.FirstOrDefault(t => t.Value == scrapeConfig.ProductAvailabilityValue);
                        if (attr != null)
                        {
                            isAviliable = true;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(scrapeConfig.ProductAvailabilityValue) && availabilityNode.InnerText == scrapeConfig.ProductAvailabilityValue)
                        {
                            isAviliable = true;
                        }
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
                {
                    product.Availability = null;
                }
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
