using ProductScraper.Models.EntityModels;
using ProductScraper.Scrapers;
using ProductScraper.Services.Interfaces;
using System;

namespace ProductScraper.Services.Implementations
{
    public class ScrapeService : IScrapeService
    {
        private readonly IScraper _anhochScraper;
        private readonly IScraper _tehnomarketScraper;
        private readonly IScraper _setTecScraper;

        public ScrapeService()
        {
            _anhochScraper = new AnhochScraper();
            _tehnomarketScraper = new TehnomarketScraper();
            _setTecScraper = new SetTecScraper();
        }
        public bool ScrapeProductInfo(ProductInfo product)
        {
            if (product.URL.Contains("anhoch.com"))
                return _anhochScraper.Scrape(product);

            if (product.URL.Contains("tehnomarket.com"))
                return _tehnomarketScraper.Scrape(product);

            if (product.URL.Contains("setec.mk"))
                return _setTecScraper.Scrape(product);

            throw new Exception("Not supported url!");
        }
    }
}
