using ProductScraper.Models.EntityModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductScraper.Services.Interfaces
{
    public interface IScrapeConfigService
    {
        Task<IList<ScrapeConfig>> GetAllAsync();
        Task<ScrapeConfig> GetDetailsAsync(int id);
        Task AddAsync(ScrapeConfig scrapeConfig);
        Task UpdateAsync(ScrapeConfig scrapeConfig);
        Task DeleteAsync(int id);
    }
}
