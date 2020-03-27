using ProductScraper.Models.EntityModels;

namespace ProductScraper.Services.Interfaces
{
    public interface IScrapeService
    {
        bool ScrapeProductInfo(ProductInfo productInfo);
    }
}
