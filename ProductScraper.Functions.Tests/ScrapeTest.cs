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
        ScrapeConfig galerijaConfig = new ScrapeConfig();
        Utils utils;

        [SetUp]
        public void Setup()
        {
            logger = new Mock<ILogger>();
            galerijaConfig = new ScrapeConfig()
            {
                Name = "Galerija.mk",
                URL = "https://www.galerija.com.mk",
                ProductNamePath = "//*[contains(@id,'product-')]/div[1]/div[2]/h1",
                ProductPricePath = "//*[contains(@id,'product-')]/div[1]/div[2]/p/span/span/text()",
            };
            utils = new Utils();
        }

        [Test]
        public void ScrapeGalerijaTest(
            [Values("https://www.galerija.com.mk/kupi/logitech-g332-gaming-headset/")] string url)
        {
            ProductInfo product = new ProductInfo();
            product.URL = url;
            utils.Scrape(galerijaConfig, product, logger.Object).Wait();
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
            var email = utils.CreateProductEmailLine(product);
            Assert.IsTrue(email.Length > 0);
        }
    }
}