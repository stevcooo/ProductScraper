using HtmlAgilityPack;
using ProductScraper.Models.EntityModels;
using System;

namespace ProductScraper.Scrapers
{
    public class TehnomarketScraper : IScraper
    {
        private readonly HtmlWeb _htmlWeb;

        public TehnomarketScraper()
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

            var priceNodes = doc.DocumentNode.SelectNodes("//div[@class='price product-price']//span[@class='nm']");

            if (priceNodes != null)
            {
                product.LastCheckedOn = DateTime.UtcNow;

                var firstPriceNode = priceNodes[0];
                if (firstPriceNode != null)
                {
                    if (product.Price != firstPriceNode.InnerText)
                        hasChanges = true;

                    product.Price = firstPriceNode.InnerText;
                }

                if (priceNodes.Count > 1)
                {
                    var secondPriceNode = priceNodes[1];
                    if (secondPriceNode != null)
                    {
                        if (product.SecondPrice != secondPriceNode.InnerText)
                            hasChanges = true;

                        product.SecondPrice = secondPriceNode.InnerText;
                    }
                }

                var availabilityNode = doc.DocumentNode.SelectNodes("//div[@class='product-content']//i[@class='icon-ok']");
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
