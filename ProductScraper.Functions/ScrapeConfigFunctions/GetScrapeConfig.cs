using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using ProductScraper.Common;
using ProductScraper.Common.Naming;
using ProductScraper.Models.EntityModels;
using System.Threading.Tasks;

namespace ProductScraper.Functions.ScrapeConfigFunctions
{
    public static class GetScrapeConfig
    {
        [FunctionName(FunctionName.GetScrapeConfig)]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = FunctionName.GetScrapeConfig + "/{partitionKey}/{rowKey}")] HttpRequest req,
            [Table(TableName.ScrapeConfig)] CloudTable scrapeConfigTable,
            string partitionKey,
            string rowKey,
            ILogger log)
        {
            log.LogInformation("GetScrapeConfig trigger function processed a request.");

            TableOperation getOperation = TableOperation.Retrieve<ScrapeConfig>(partitionKey, rowKey);
            TableResult result = await scrapeConfigTable.ExecuteAsync(getOperation);

            if (199 < result.HttpStatusCode && result.HttpStatusCode < 300)
                return new OkObjectResult((ScrapeConfig)(dynamic)result.Result);
            else
                return new BadRequestObjectResult(result.Result);
        }
    }
}
