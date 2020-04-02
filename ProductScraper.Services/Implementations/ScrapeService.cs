using HtmlAgilityPack;
using ProductScraper.Models.EntityModels;
using ProductScraper.Services.Exceptions;
using ProductScraper.Services.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ProductScraper.Services.Implementations
{
    public class ScrapeService : IScrapeService
    {
        private readonly IScrapeConfigService _scrapeConfigService;
        private readonly HtmlWeb _htmlWeb;

        public ScrapeService(IScrapeConfigService scrapeConfigService)
        {
            _scrapeConfigService = scrapeConfigService;
            _htmlWeb = new HtmlWeb();
        }

        public async Task ScrapeProductInfoAsync(ProductInfo product)
        {
            var allConfigs = await _scrapeConfigService.GetAllAsync();
            var configs = allConfigs.Where(t => product.URL.Contains(t.URL)).ToList();
            if (configs.Any())
            {
                if (configs.Count > 1)
                {
                    //Alert if more than one config exists for current url
                    //Do not throw error, just send an email to the admin and continue processing with the last one                    
                }
                Scrape(configs.FirstOrDefault(), product);
            }
            else
            {
                //Alert if more no config exists so it should be added
                throw new ScrapeServiceException("Unsupported URL!");
            }
        }

        public void Scrape(ScrapeConfig scrapeConfig, ProductInfo product)
        {
            bool hasError = false;

            if (string.IsNullOrWhiteSpace(product.URL))
                throw new Exception("URL can not be empty!");

            product.HasChangesSinceLastTime = false;
            HtmlDocument doc = _htmlWeb.Load(product.URL);

            try
            {
                var titleNode = doc.DocumentNode.SelectSingleNode(scrapeConfig.ProductNamePath);
                if (titleNode != null && product.Name != titleNode.InnerText)
                {
                    product.HasChangesSinceLastTime = true;
                    product.Name = titleNode.InnerText;
                }
            }
            catch (Exception ex)
            {
                //Log the exception
                hasError = true;
            }

            try
            {
                var priceNode = doc.DocumentNode.SelectSingleNode(scrapeConfig.ProductPricePath);
                if (priceNode != null && product.Price != priceNode.InnerText)
                {
                    product.HasChangesSinceLastTime = true;
                    product.Price = priceNode.InnerText;
                }
            }
            catch (Exception ex)
            {
                //Log the exception
                hasError = true;
            }


            try
            {
                var secondPriceNode = doc.DocumentNode.SelectSingleNode(scrapeConfig.ProductSecondPricePath);
                if (secondPriceNode != null && product.SecondPrice != secondPriceNode.InnerText)
                {
                    product.HasChangesSinceLastTime = true;
                    product.SecondPrice = secondPriceNode.InnerText;
                }
            }
            catch (Exception ex)
            {
                //Log the exception
                hasError = true;
            }

            try
            {
                var availabilityNode = doc.DocumentNode.SelectSingleNode(scrapeConfig.ProductAvailabilityPath);
                bool isAviliable = availabilityNode != null;
                if (product.Availability != isAviliable)
                {
                    product.HasChangesSinceLastTime = true;
                    product.Availability = isAviliable;
                }
            }
            catch (Exception ex)
            {
                //Log the exception
                hasError = true;
            }

            product.LastCheckedOn = DateTime.UtcNow;
        }
    }
}
