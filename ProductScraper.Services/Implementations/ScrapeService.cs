using HtmlAgilityPack;
using ProductScraper.Models.EntityModels;
using ProductScraper.Services.Exceptions;
using ProductScraper.Services.Interfaces;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ProductScraper.Services.Implementations
{
    public class ScrapeService : IScrapeService
    {
        private readonly IScrapeConfigService _scrapeConfigService;
        private readonly HtmlWeb _htmlWeb;
        private readonly WebClient _webClient;

        public ScrapeService(IScrapeConfigService scrapeConfigService)
        {
            _scrapeConfigService = scrapeConfigService;
            _htmlWeb = new HtmlWeb();
            _webClient = new WebClient();
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

            
            string html = _webClient.DownloadString(product.URL);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            product.HasChangesSinceLastTime = false;

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
                if (availabilityNode != null)
                {
                    bool isAviliable = false;

                    if (scrapeConfig.ProductAvailabilityIsAtributeValue)
                    {
                        var attr = availabilityNode.Attributes.FirstOrDefault(t => t.Value == scrapeConfig.ProductAvailabilityValue);
                        if (attr != null)
                        {
                            isAviliable = true;
                        }
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(scrapeConfig.ProductAvailabilityValue) && availabilityNode.InnerText == scrapeConfig.ProductAvailabilityValue)
                            isAviliable = true;
                        else
                        {
                            isAviliable = availabilityNode != null;
                        }

                    }

                    if (product.Availability != isAviliable)
                    {
                        product.HasChangesSinceLastTime = true;
                        product.Availability = isAviliable;
                    }
                }
                else
                    product.Availability = null;

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
