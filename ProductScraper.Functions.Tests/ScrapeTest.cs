using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using ProductScraper.Functions.Common;
using ProductScraper.Models.EntityModels;
using System;

namespace ProductScraper.Functions.Tests
{
    public class ScrapeTest
    {
        Mock<ILogger> logger;
        ScrapeConfig mediMarketConfig = new ScrapeConfig();

        [SetUp]
        public void Setup()
        {
            logger = new Mock<ILogger>();
            mediMarketConfig = new ScrapeConfig()
            {
                Name = "https://www.mediamarkt.de",
                URL = "https://www.mediamarkt.de",
                ProductNamePath = "/html/body/div[1]/div/div[2]/div[2]/div[2]/div[1]/div[1]/div/h1/text()",
                ProductPricePath = "/html/body/div[1]/div/div[2]/div[2]/div[2]/div[1]/div[5]/div[1]/div/div/div/div/div/div/div/div[2]/span",
                ProductAvailabilityPath = "/html/body/div[1]/div/div[2]/div[2]/div[2]/div[1]/div[5]/div[5]/div/div[1]/span/span"
            };            
        }

        [Test]
        public void ScrapeMediaMarket()
        {
            ProductInfo product = new ProductInfo();
            product.URL = "https://www.mediamarkt.de/de/product/_logitech-mx-master-3-2590562.html";
            Utils.Scrape(mediMarketConfig, product, logger.Object).Wait();
            Assert.IsTrue(product.HasChangesSinceLastTime);
        }

        [Test]
        public void ProductEmailTest()
        {
            ProductInfo product = new ProductInfo();
            product.URL = "https://www.mediamarkt.de/de/product/_logitech-mx-master-3-2590562.html";
            product.Price = "122";
            product.Name = "ProductName";
            product.LastCheckedOn = DateTime.Now;
            product.HasChangesSinceLastTime = true;
            var email = Utils.CreateProductEmailLine(product);
            Assert.IsTrue(email.Length > 0);
        }

        [Test]
        public void ProductPrevoiousPriceEmailTest()
        {
            ProductInfo product = new ProductInfo();
            product.URL = "https://www.mediamarkt.de/de/product/_logitech-mx-master-3-2590562.html";
            product.Price = "122";
            product.Name = "ProductName";
            product.LastCheckedOn = DateTime.Now;
            product.HasChangesSinceLastTime = true;
            
            Utils.Scrape(mediMarketConfig, product, logger.Object).Wait();

            var email = Utils.CreateProductEmailLine(product);
            Assert.IsTrue(email.Length > 0);
        }
    }
}