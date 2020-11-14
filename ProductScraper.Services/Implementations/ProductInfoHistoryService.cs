using Microsoft.Extensions.Options;
using ProductScraper.Models.EntityModels;
using ProductScraper.Models.ViewModels;
using ProductScraper.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductScraper.Services.Implementations
{
    public class ProductInfoHistoryService : IProductInfoHistoryService
    {
        private readonly IAzureTableStorage<ProductInfoHistory> _repository;
        private readonly IOptions<AppSettings> _settings;

        public ProductInfoHistoryService(IAzureTableStorage<ProductInfoHistory> repository,
            IOptions<AppSettings> settings)
        {
            _repository = repository;
            _settings = settings;
        }

        public async Task AddAsync(long productId, ProductInfoHistory history)
        {
            history.Id = DateTime.Now.Ticks;
            history.RowKey = history.Id.ToString();
            history.PartitionKey = productId.ToString();
            await _repository.Insert(history);
        }

        public async Task<IList<ProductInfoHistory>> GetAllForProductAsync(long productId)
        {
            return await _repository.GetList(productId);
        }

        public async Task<ProductInfoHistory> GetDetailsAsync(long productId, long id)
        {
            return await _repository.GetItem(productId, id);
        }
    }
}