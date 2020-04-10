using ProductScraper.Models.EntityModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductScraper.Services.Interfaces
{
    public interface IScrapeConfigService
    {
        Task<IList<ScrapeConfig>> GetAllAsync();
        Task<ScrapeConfig> GetDetailsAsync(string partitionKey, string rowKey);
        Task AddAsync(ScrapeConfig scrapeConfig);
        Task UpdateAsync(ScrapeConfig scrapeConfig);
        Task DeleteAsync(string partitionKey, string rowKey);
    }
}
