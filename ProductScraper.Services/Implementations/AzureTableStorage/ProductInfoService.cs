using ProductScraper.Models.EntityModels;
using ProductScraper.Services.Interfaces;
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
            productInfo.PartitionKey = userId;
            await _repository.Insert(productInfo);
        }

        public Task CheckAsync(string userId, int id)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteAsync(string userId, int id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IList<ProductInfo>> GetAllAsync(string userId)
        {
            return await _repository.GetList(userId);
        }

        public Task<ProductInfo> GetDetailsAsync(string userId, int id)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateAsync(string userId, ProductInfo productInfo)
        {
            throw new System.NotImplementedException();
        }
    }
}
