using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using ProductScraper.Common;
using ProductScraper.Models.EntityModels;
using System.IO;
using System.Threading.Tasks;

namespace ProductScraper.Functions.ScrapeConfigFunctions
{
    public static class AddScrapeConfig
    {
        [FunctionName(FunctionsNames.AddScrapeConfig)]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [Table("ScrapeConfig")] CloudTable scrapeConfigTable,
            ILogger log)
        {
            log.LogInformation("AddScrapeConfig trigger function processed a request.");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var scrapeConfig = JsonConvert.DeserializeObject<ScrapeConfig>(requestBody);
            
            if (scrapeConfig == null)
                return new BadRequestResult();            

            var insertOperation = TableOperation.Insert(scrapeConfig);
            var result = await scrapeConfigTable.ExecuteAsync(insertOperation);

            if (199 < result.HttpStatusCode && result.HttpStatusCode < 300)
                return new OkObjectResult(result.Result);
            else
                return new BadRequestObjectResult(result.Result);
        }
    }
}
