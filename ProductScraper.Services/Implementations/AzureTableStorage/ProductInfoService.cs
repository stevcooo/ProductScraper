using ProductScraper.Models.EntityModels;
using ProductScraper.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductScraper.Services.Implementations.AzureTableStorage
{
    public class ProductInfoService : IProductInfoService
    {
        private readonly IAzureTableStorage<ProductInfo> _repository;

        public ProductInfoService(IAzureTableStorage<ProductInfo> repository)
        {
            _repository = repository;
        }

        public async Task AddAsync(string userId, ProductInfo productInfo)
        {
            productInfo.Id = DateTime.Now.Ticks;
            productInfo.RowKey = productInfo.Id.ToString();
            productInfo.PartitionKey = userId;
            await _repository.Insert(productInfo);
        }

        public Task CheckAsync(string userId, long id)
        {
            throw new System.NotImplementedException();
        }

        public async Task DeleteAsync(string userId, long id)
        {
            await _repository.Delete(userId, id.ToString());
        }

        public async Task<IList<ProductInfo>> GetAllAsync(string userId)
        {
            return await _repository.GetList(userId);
        }

        public async Task<ProductInfo> GetDetailsAsync(string userId, long id)
        {
            return await _repository.GetItem(userId, id.ToString());            
        }

        public async Task UpdateAsync(string userId, ProductInfo productInfo)
        {
            productInfo.RowKey = productInfo.Id.ToString();
            productInfo.PartitionKey = userId;
            await _repository.Update(productInfo);            
        }
    }
}
