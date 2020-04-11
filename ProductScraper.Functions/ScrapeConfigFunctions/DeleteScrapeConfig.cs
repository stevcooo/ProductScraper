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
    public static class DeleteScrapeConfig
    {
        [FunctionName(FunctionName.DeleteScrapeConfig)]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = FunctionName.DeleteScrapeConfig+"/{partitionKey}/{rowKey}")] HttpRequest req,            
            [Table(TableName.ScrapeConfig)] CloudTable scrapeConfigTable,
            string partitionKey,
            string rowKey,
            ILogger log)
        {
            log.LogInformation("DeleteScrapeConfig trigger function processed a request.");

            TableOperation getOperation = TableOperation.Retrieve<ScrapeConfig>(partitionKey, rowKey);

            TableResult item = await scrapeConfigTable.ExecuteAsync(getOperation);

            var itemToDelete = (ScrapeConfig)(dynamic)item.Result;

            TableOperation deleteOperation = TableOperation.Delete(itemToDelete);

            //Execute
            var result = await scrapeConfigTable.ExecuteAsync(deleteOperation);

            if (199 < result.HttpStatusCode && result.HttpStatusCode < 300)
                return new OkObjectResult(result.Result);
            else
                return new BadRequestObjectResult(result.Result);
        }
    }
}
