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
        ScrapeConfig galerijaConfig = new ScrapeConfig();

        [SetUp]
        public void Setup()
        {
            logger = new Mock<ILogger>();
            mediMarketConfig = new ScrapeConfig()
            {
                Name = "Mediamarkt.de",
                URL = "https://www.mediamarkt.de",
                ProductNamePath = "/html/body/div[1]/div/div[2]/div[2]/div[2]/div[1]/div[1]/div/h1/text()",
                ProductPricePath = "/html/body/div[1]/div/div[2]/div[2]/div[2]/div[1]/div[5]/div[1]/div/div/div/div/div/div/div/div[2]/span",
                ProductAvailabilityPath = "/html/body/div[1]/div/div[2]/div[2]/div[2]/div[1]/div[5]/div[5]/div/div[1]/span/span"
            };

            galerijaConfig = new ScrapeConfig()
            {
                Name = "Galerija.mk",
                URL = "https://www.galerija.com.mk",
                ProductNamePath = "//*[contains(@id,'product-')]/div[1]/div[2]/h1",
                ProductPricePath = "//*[contains(@id,'product-')]/div[1]/div[2]/p/span/span/text()",
            };
        }

        [Test]
        public void ScrapeMediaMarketTest(
            [Values(
            "https://www.mediamarkt.de/de/product/_logitech-mx-master-3-2590562.html",
            "https://www.mediamarkt.de/de/product/_apple-airpods-2-gen-mit-ladecase-2539111.html")] string url)
        {
            ProductInfo product = new ProductInfo();
            product.URL = url;
            Utils.Scrape(mediMarketConfig, product, logger.Object).Wait();
            Assert.IsNotNull(product.Name);
            Assert.IsNotNull(product.Price);
            Assert.IsTrue(product.HasChangesSinceLastTime);
        }

        [Test]
        public void ScrapeGalerijaTest(
            [Values("https://www.galerija.com.mk/kupi/tv-kd65xf9005/",
            "https://www.galerija.com.mk/kupi/kd65ag8baep/",
            "https://www.galerija.com.mk/kupi/logitech-g332-gaming-headset/")] string url)
        {
            ProductInfo product = new ProductInfo();
            product.URL = url;
            Utils.Scrape(galerijaConfig, product, logger.Object).Wait();
            Assert.IsNotNull(product.Name);
            Assert.IsNotNull(product.Price);
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