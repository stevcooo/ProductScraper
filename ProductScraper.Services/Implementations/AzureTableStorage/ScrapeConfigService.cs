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

        public async Task DeleteAsync(long id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IList<ScrapeConfig>> GetAllAsync()
        {
            return await _repository.GetList();
        }

        public async Task<ScrapeConfig> GetDetailsAsync(long id)
        {
            throw new System.NotImplementedException();
        }

        public async Task UpdateAsync(ScrapeConfig scrapeConfig)
        {
            scrapeConfig.RowKey = scrapeConfig.Id.ToString();
            scrapeConfig.PartitionKey = scrapeConfig.URL.ToCoreUrl();
            await _repository.Update(scrapeConfig);            
        }
    }
}
