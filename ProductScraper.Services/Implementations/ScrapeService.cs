using ProductScraper.Models.EntityModels;
using ProductScraper.Scrapers;
using ProductScraper.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProductScraper.Services.Implementations
{
    public class ScrapeService : IScrapeService
    {
        private readonly IScraper _anhochScraper;
        private readonly IScraper _tehnomarketScraper;
        private readonly IScraper _setTecScraper;
        private readonly GenericScraper _genericScraper;

        //This should be loaded from service/db
        List<ScrapeConfig> ScrapeConfigs = new List<ScrapeConfig>();

        public ScrapeService()
        {
            _anhochScraper = new AnhochScraper();
            _tehnomarketScraper = new TehnomarketScraper();
            _setTecScraper = new SetTecScraper();
            _genericScraper = new GenericScraper();

            //This should be loaded from service/db
            AddConfigs();
        }

        //This should be loaded from service/db
        private void AddConfigs()
        {
            ScrapeConfigs = new List<ScrapeConfig>();
            ScrapeConfig anhoch = new ScrapeConfig()
            {
                Name = "Anhoch",
                URL = "anhoch.com",
                ProductNamePath = "/body[1]/div[3]/div[1]/div[1]/div[1]/section[1]/div[1]/div[3]/section[1]/div[1]/div[1]/h3[1]",
                ProductPricePath = "/body[1]/div[3]/div[1]/div[1]/div[1]/section[1]/div[1]/div[3]/section[1]/div[1]/div[2]/section[1]/div[1]/div[2]/div[1]/div[1]/div[1]/span[1]",
                ProductSecondPricePath = "",
                ProductAvailabilityPath = "/body[1]/div[3]/div[1]/div[1]/div[1]/section[1]/div[1]/div[3]/section[1]/div[1]/div[2]/section[1]/div[1]/div[2]/div[1]/div[2]/a[2]/i[1]",
            };
            ScrapeConfigs.Add(anhoch);
        }

        public bool ScrapeProductInfo(ProductInfo product)
        {
            var configs = ScrapeConfigs.Where(t => product.URL.Contains(t.URL)).ToList();
            if (configs.Any())
            {
                if (configs.Count > 1)
                {
                    //Alert if more than one config exists for current url
                    //Do not throw error, just send an email to the admin and continue processing with the last one                    
                }
                return _genericScraper.Scrape(configs.FirstOrDefault(), product);
            }
            else //Try hardcoded configs
            {
                if (product.URL.Contains("anhoch.com"))
                    return _anhochScraper.Scrape(product);

                if (product.URL.Contains("tehnomarket.com"))
                    return _tehnomarketScraper.Scrape(product);

                if (product.URL.Contains("setec.mk"))
                    return _setTecScraper.Scrape(product);
            }

            throw new Exception("Not supported url!");
        }
    }
}
