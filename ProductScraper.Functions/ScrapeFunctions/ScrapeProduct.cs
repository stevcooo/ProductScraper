using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using ProductScraper.Common.Naming;
using ProductScraper.Functions.Common;
using ProductScraper.Models.EntityModels;
using ProductScraper.Models.Extensions;
using SendGrid.Helpers.Mail;
using System.Linq;
using System.Threading.Tasks;

namespace ProductScraper.Functions.ScrapeFunctions
{
    public static class ScrapeProduct
    {
        [FunctionName(FunctionName.ScrapeProduct)]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = FunctionName.ScrapeProduct + "/{userId}/{productId}")] HttpRequest req,
            [Table(TableName.ProductInfo, "{userId}")] CloudTable productInfoTable,
            [Table(TableName.ScrapeConfig)] CloudTable scrapeConfigTable,
            [Queue(QueueName.AddProductHistory)] IAsyncCollector<ProductInfo> addProductHistoryMessageQueue,
            [Queue(QueueName.EmailsToSend)] IAsyncCollector<SendGridMessage> emailMessageQueue,
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
                if (!configs.Any())
                {
                    //Notify the admin
                    SendGridMessage message = new SendGridMessage();
                    message.SetFrom(new EmailAddress("info@productscrape.com", "Product scraper"));
                    message.AddTo(CommonName.AdminEmail);
                    message.SetSubject("Missing configuration");
                    message.AddContent("text/plain", $"There was request for scraping products from {product.URL.ToCoreUrl()}, consider adding configuration soon. Product Url {product.URL}");
                    await emailMessageQueue.AddAsync(message);
                }
                else if (configs.Count() > 1)
                {
                    //Notify the admin
                    SendGridMessage message = new SendGridMessage();
                    message.SetFrom(new EmailAddress("info@productscrape.com", "Product scraper"));
                    message.AddTo(CommonName.AdminEmail);
                    message.SetSubject("Multiple configurations");
                    message.AddContent("text/plain", $"There are more than one configuration for: {product.URL.ToCoreUrl()}, consider deleting one.");
                    await emailMessageQueue.AddAsync(message);
                }
                else if (configs.Count() == 1)
                {
                    Utils utils = new Utils();
                    await utils.Scrape(configs.First(), product, log);
                    //Update product in db
                    TableOperation operation = TableOperation.InsertOrReplace(product);
                    await productInfoTable.ExecuteAsync(operation);

                    //Add to history queue
                    await addProductHistoryMessageQueue.AddAsync(product);
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
    }
}
