using ProductScraper.Models.EntityModels;
using System.Threading.Tasks;

namespace ProductScraper.Services.Interfaces
{
    public interface IScrapeService
    {
        Task<bool> ScrapeProductInfoAsync(ProductInfo productInfo);
    }
}
