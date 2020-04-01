using HtmlAgilityPack;
using ProductScraper.Models.EntityModels;
using System;

namespace ProductScraper.Scrapers
{
    public class GenericScraper : IScraper
    {
        private readonly HtmlWeb _htmlWeb;

        public GenericScraper()
        {
            _htmlWeb = new HtmlWeb();
        }

        public bool Scrape(ProductInfo product)
        {
            return false;
        }

        public bool Scrape(ScrapeConfig scrapeConfig, ProductInfo product)
        {
            bool hasChanges = false;
            bool hasError = false;

            if (string.IsNullOrWhiteSpace(product.URL))
                throw new Exception("URL can not be empty!");

            HtmlDocument doc = _htmlWeb.Load(product.URL);

            try
            {
                var titleNode = doc.DocumentNode.SelectSingleNode(scrapeConfig.ProductNamePath);
                if (titleNode != null && product.Name != titleNode.InnerText)
                {
                    hasChanges = true;
                    product.Name = titleNode.InnerText;
                }
            }
            catch(Exception ex)
            {
                //Log the exception
                hasError = true;
            }

            try
            {
                var priceNode = doc.DocumentNode.SelectSingleNode(scrapeConfig.ProductPricePath);
                if (priceNode != null && product.Price != priceNode.InnerText)
                {
                    hasChanges = true;
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
                    hasChanges = true;
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
                    hasChanges = true;
                    product.Availability = isAviliable;
                }
            }
            catch (Exception ex)
            {
                //Log the exception
                hasError = true;
            }

            product.LastCheckedOn = DateTime.UtcNow;
            return hasChanges;
        }
    }
}
