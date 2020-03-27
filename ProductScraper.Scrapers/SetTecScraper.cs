using HtmlAgilityPack;
using ProductScraper.Models.EntityModels;
using System;

namespace ProductScraper.Scrapers
{
    public class SetTecScraper : IScraper
    {
        private readonly HtmlWeb _htmlWeb;

        public SetTecScraper()
        {
            _htmlWeb = new HtmlWeb();
        }
        public bool Scrape(ProductInfo product)
        {
            bool hasChanges = false;

            if (string.IsNullOrWhiteSpace(product.URL))
                throw new Exception("URL can not be empty!");

            HtmlDocument doc = _htmlWeb.Load(product.URL);

            var titleNode = doc.DocumentNode.SelectSingleNode("//h1[@id='title-page']");
            if (titleNode != null)
                product.Name = titleNode.InnerText;

            product.LastCheckedOn = DateTime.UtcNow;

            var price1 = doc.DocumentNode.SelectSingleNode("//span[@id='price-old-product']");
            var price2 = doc.DocumentNode.SelectSingleNode("//span[@id='price-special']");

            if (price1 != null)
            {
                if (product.Price != price1.InnerText)
                    hasChanges = true;

                product.Price = price1.InnerText;
            }

            if (price2 != null)
            {
                if (product.SecondPrice != price2.InnerText)
                    hasChanges = true;

                product.SecondPrice = price2.InnerText;
            }

            return hasChanges;
        }
    }
}
