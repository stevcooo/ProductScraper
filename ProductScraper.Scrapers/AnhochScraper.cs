using HtmlAgilityPack;
using ProductScraper.Models.EntityModels;
using System;

namespace ProductScraper.Scrapers
{
    public class AnhochScraper : IScraper
    {
        private readonly HtmlWeb _htmlWeb;

        public AnhochScraper()
        {
            _htmlWeb = new HtmlWeb();
        }
        public bool Scrape(ProductInfo product)
        {
            bool hasChanges = false;

            if (string.IsNullOrWhiteSpace(product.URL))
                throw new Exception("URL can not be empty!");

            HtmlDocument doc = _htmlWeb.Load(product.URL);

            var titleNode = doc.DocumentNode.SelectSingleNode("//div[@class='box-heading ']/h3");
            if (titleNode != null)
                product.Name = titleNode.InnerText;

            var productContent = doc.DocumentNode.SelectSingleNode("//div[@class='product-content']");
            if (productContent != null)
            {
                product.LastCheckedOn = DateTime.UtcNow;
                var priceNode = productContent.SelectSingleNode("//span[@class='nm']");

                if (priceNode != null)
                {
                    if (product.Price != priceNode.InnerText)
                        hasChanges = true;

                    product.Price = priceNode.InnerText;
                }

                var availabilityNode = productContent.SelectNodes("//i[@class='icon-ok']");
                bool isAviliable = availabilityNode != null;

                if (product.Availability != isAviliable)
                    hasChanges = true;

                product.Availability = isAviliable;
            }

            return hasChanges;
        }

        public bool Scrape(ScrapeConfig scrapeConfig, ProductInfo product)
        {
            return Scrape(product);
        }
    }
}
