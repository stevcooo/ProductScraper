using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using ProductScraper.Common;
using ProductScraper.Models.EntityModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductScraper.Functions.ScrapeConfigFunctions
{
    public static class GetAllScrapeConfigs
    {
        [FunctionName(FunctionsNames.GetAllScrapeConfigs)]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [Table("ScrapeConfig")] CloudTable scrapeConfigTable,
            ILogger log)
        {
            log.LogInformation("GetAllScrapeConfigs trigger function processed a request.");

            //Query
            TableQuery<ScrapeConfig> query = new TableQuery<ScrapeConfig>();

            List<ScrapeConfig> results = new List<ScrapeConfig>();
            TableContinuationToken continuationToken = null;
            do
            {
                TableQuerySegment<ScrapeConfig> queryResults =
                    await scrapeConfigTable.ExecuteQuerySegmentedAsync(query, continuationToken);

                continuationToken = queryResults.ContinuationToken;
                results.AddRange(queryResults.Results);

            } while (continuationToken != null);

            return new OkObjectResult(results);
        }
    }
}
