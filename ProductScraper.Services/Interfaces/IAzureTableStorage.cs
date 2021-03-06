﻿using ProductScraper.Models.EntityModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductScraper.Services.Interfaces
{
    public interface IAzureTableStorage<T> where T : AzureTableEntity, new()
    {
        Task Delete(string partitionKey, string rowKey);
        Task<T> GetItem(long partitionKey, long rowKey);
        Task<T> GetItem(string partitionKey, string rowKey);
        Task<List<T>> GetList();
        Task<List<T>> GetList(string partitionKey);
        Task<List<T>> GetList(long partitionKey);
        Task Insert(T item);
        Task Update(T item);
    }
}
