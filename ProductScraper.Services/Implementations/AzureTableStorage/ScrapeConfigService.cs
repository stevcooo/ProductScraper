using ProductScraper.Models.EntityModels;
using ProductScraper.Models.Extensions;
using ProductScraper.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductScraper.Services.Implementations.AzureTableStorage
{
    public class ScrapeConfigService : IScrapeConfigService
    {
        private readonly IAzureTableStorage<ScrapeConfig> _repository;

        public ScrapeConfigService(IAzureTableStorage<ScrapeConfig> repository)
        {
            _repository = repository;
        }

        public async Task AddAsync(ScrapeConfig scrapeConfig)
        {
            scrapeConfig.Id = DateTime.Now.Ticks;
            scrapeConfig.RowKey = scrapeConfig.Id.ToString();
            scrapeConfig.PartitionKey = scrapeConfig.URL.ToCoreUrl();
            await _repository.Insert(scrapeConfig);
        }

        public async Task DeleteAsync(string partitionKey, string rowKey)
        {
            await _repository.Delete(partitionKey, rowKey);
        }

        public async Task<IList<ScrapeConfig>> GetAllAsync()
        {
            return await _repository.GetList();
        }

        public async Task<ScrapeConfig> GetDetailsAsync(string partitionKey, string rowKey)
        {
            return await _repository.GetItem(partitionKey, rowKey);
        }

        public async Task UpdateAsync(ScrapeConfig scrapeConfig)
        {
            scrapeConfig.RowKey = scrapeConfig.Id.ToString();
            scrapeConfig.PartitionKey = scrapeConfig.URL.ToCoreUrl();
            await _repository.Update(scrapeConfig);            
        }
    }
}
