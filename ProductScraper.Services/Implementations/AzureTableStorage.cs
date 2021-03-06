﻿using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using ProductScraper.Models.EntityModels;
using ProductScraper.Models.ViewModels;
using ProductScraper.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductScraper.Services.Implementations
{
    public class AzureTableStorage<T> : IAzureTableStorage<T>
        where T : AzureTableEntity, new()
    {
        #region " Public "

        public AzureTableStorage(AzureTableSettings settings)
        {
            this.settings = settings;
        }

        public async Task<List<T>> GetList()
        {
            //Table
            CloudTable table = await GetTableAsync();

            //Query
            TableQuery<T> query = new TableQuery<T>();

            List<T> results = new List<T>();
            TableContinuationToken continuationToken = null;
            do
            {
                TableQuerySegment<T> queryResults =
                    await table.ExecuteQuerySegmentedAsync(query, continuationToken);

                continuationToken = queryResults.ContinuationToken;
                results.AddRange(queryResults.Results);

            } while (continuationToken != null);

            return results;
        }

        public async Task<List<T>> GetList(long partitionKey)
        {
            return await GetList(partitionKey.ToString());
        }

        public async Task<List<T>> GetList(string partitionKey)
        {
            //Table
            CloudTable table = await GetTableAsync();

            //Query
            TableQuery<T> query = new TableQuery<T>()
                                        .Where(TableQuery.GenerateFilterCondition("PartitionKey",
                                                QueryComparisons.Equal, partitionKey));

            List<T> results = new List<T>();
            TableContinuationToken continuationToken = null;
            do
            {
                TableQuerySegment<T> queryResults =
                    await table.ExecuteQuerySegmentedAsync(query, continuationToken);

                continuationToken = queryResults.ContinuationToken;

                results.AddRange(queryResults.Results);

            } while (continuationToken != null);

            return results;
        }
        public async Task<T> GetItem(long partitionKey, long rowKey)
        {
            return await GetItem(partitionKey.ToString(), rowKey.ToString());
        }

        public async Task<T> GetItem(string partitionKey, string rowKey)
        {
            //Table
            CloudTable table = await GetTableAsync();

            //Operation
            TableOperation operation = TableOperation.Retrieve<T>(partitionKey, rowKey);

            //Execute
            TableResult result = await table.ExecuteAsync(operation);

            return (T)(dynamic)result.Result;
        }

        public async Task Insert(T item)
        {
            //Table
            CloudTable table = await GetTableAsync();

            //Operation
            TableOperation operation = TableOperation.Insert(item);

            //Execute
            await table.ExecuteAsync(operation);
        }

        public async Task Update(T item)
        {
            //Table
            CloudTable table = await GetTableAsync();

            //Operation
            TableOperation operation = TableOperation.InsertOrReplace(item);

            //Execute
            await table.ExecuteAsync(operation);
        }

        public async Task Delete(string partitionKey, string rowKey)
        {
            //Item
            T item = await GetItem(partitionKey, rowKey);

            //Table
            CloudTable table = await GetTableAsync();

            //Operation
            TableOperation operation = TableOperation.Delete(item);

            //Execute
            await table.ExecuteAsync(operation);
        }

        #endregion

        #region " Private "

        private readonly AzureTableSettings settings;

        private async Task<CloudTable> GetTableAsync()
        {
            //Client
            CloudTableClient tableClient = CloudStorageAccount.Parse(settings.StorageConnectionString).CreateCloudTableClient();

            //Table
            CloudTable table = tableClient.GetTableReference(settings.TableName);
            await table.CreateIfNotExistsAsync();

            return table;
        }

        #endregion
    }
}
