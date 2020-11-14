using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using ProductScraper.Common.Naming;
using ProductScraper.Models.EntityModels;
using System;

namespace ProductScraper.Functions.ProductInfoHistoryFunctions
{
    public static class AddProductInfoHistory
    {
        [FunctionName(FunctionName.AddProductInfoHistory)]
        public static async void Run(
            [QueueTrigger(QueueName.AddProductHistory, Connection = CommonName.Connection)] ProductInfo productInfo,
            [Table(TableName.ProductInfoHistory)] CloudTable productInfoHistoryTable,
            ILogger log)
        {

            log.LogInformation($"C# Queue trigger function for history record for product : {productInfo.Name}");

            if (productInfo != null)
            {
                ProductInfoHistory historyRecord = new ProductInfoHistory();
                historyRecord.Id = DateTime.Now.Ticks;
                historyRecord.RowKey = historyRecord.Id.ToString();
                historyRecord.PartitionKey = productInfo.Id.ToString();
                historyRecord.Price = productInfo.Price;
                historyRecord.ProductInfoId = productInfo.Id;

                TableOperation operation = TableOperation.Insert(historyRecord);
                await productInfoHistoryTable.ExecuteAsync(operation);
            }
        }
    }
}
