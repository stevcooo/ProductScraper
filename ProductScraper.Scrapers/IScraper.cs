using ProductScraper.Models.EntityModels;


namespace ProductScraper.Scrapers
{
    public interface IScraper
    {
        bool Scrape(ProductInfo product);
    }
}
