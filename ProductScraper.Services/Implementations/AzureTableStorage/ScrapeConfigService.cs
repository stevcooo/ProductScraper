using ProductScraper.Models.EntityModels;
using ProductScraper.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductScraper.Services.Implementations.AzureTableStorage
{
    public class ScrapeConfigService : IScrapeConfigService
    {
        private readonly IAzureTableStorage<ScrapeConfig> _repository;
        private const string PARTITION_KEY = "ScrapeConfig";

        public ScrapeConfigService(IAzureTableStorage<ScrapeConfig> repository)
        {
            _repository = repository;
        }

        public async Task AddAsync(ScrapeConfig scrapeConfig)
        {
            scrapeConfig.Id = DateTime.Now.Ticks;
            scrapeConfig.RowKey = scrapeConfig.Id.ToString();
            scrapeConfig.PartitionKey = PARTITION_KEY;            
            await _repository.Insert(scrapeConfig);
        }

        public async Task DeleteAsync(long id)
        {
            await _repository.Delete(PARTITION_KEY, id.ToString());
        }

        public async Task<IList<ScrapeConfig>> GetAllAsync()
        {
            return await _repository.GetList(PARTITION_KEY);
        }

        public async Task<ScrapeConfig> GetDetailsAsync(long id)
        {
            return await _repository.GetItem(PARTITION_KEY, id.ToString());
        }

        public async Task UpdateAsync(ScrapeConfig scrapeConfig)
        {
            scrapeConfig.RowKey = scrapeConfig.Id.ToString();
            scrapeConfig.PartitionKey = PARTITION_KEY;
            await _repository.Update(scrapeConfig);            
        }
    }
}
