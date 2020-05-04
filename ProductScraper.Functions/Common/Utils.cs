using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using ProductScraper.Models.EntityModels;
using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ProductScraper.Functions.Common
{
    public static class Utils
    {
        private static readonly WebClient _webClient = new WebClient();

        public static async Task Scrape(ScrapeConfig scrapeConfig, ProductInfo product, ILogger log)
        {
            if (string.IsNullOrWhiteSpace(product.URL))
            {
                log.LogInformation("URL can not be empty!");
                return;
            }

            string html =  await _webClient.DownloadStringTaskAsync(product.URL);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            product.HasChangesSinceLastTime = false;
            product.Currency = scrapeConfig.Currency;
            try
            {
                HtmlNode titleNode = doc.DocumentNode.SelectSingleNode(scrapeConfig.ProductNamePath);
                if (titleNode != null && product.Name != titleNode.InnerText)
                {
                    product.HasChangesSinceLastTime = true;
                    product.PreviousName = product.Name;
                    product.Name = titleNode.InnerText;
                }
            }
            catch (Exception ex)
            {
                log.LogInformation(ex.Message);
            }

            try
            {
                HtmlNode priceNode = doc.DocumentNode.SelectSingleNode(scrapeConfig.ProductPricePath);
                if (priceNode != null)
                {
                    var newPrice = priceNode.InnerText.Replace("&nbsp;", "");
                    if(product.Price != newPrice)
                    {
                        product.HasChangesSinceLastTime = true;
                        product.PreviousPrice = product.Price;
                        product.Price = newPrice;
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogInformation(ex.Message);
            }

            try
            {
                HtmlNode availabilityNode = doc.DocumentNode.SelectSingleNode(scrapeConfig.ProductAvailabilityPath);
                if (availabilityNode != null)
                {
                    bool isAviliable = false;

                    if (scrapeConfig.ProductAvailabilityIsAtributeValue)
                    {
                        HtmlAttribute attr = availabilityNode.Attributes.FirstOrDefault(t => t.Value == scrapeConfig.ProductAvailabilityValue);
                        if (attr != null)
                        {
                            isAviliable = true;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(scrapeConfig.ProductAvailabilityValue) && availabilityNode.InnerText == scrapeConfig.ProductAvailabilityValue)
                        {
                            isAviliable = true;
                        }
                        else
                        {
                            isAviliable = availabilityNode != null;
                        }

                    }

                    if (product.Availability != isAviliable)
                    {
                        product.HasChangesSinceLastTime = true;
                        product.PreviousAvailability = product.Availability;
                        product.Availability = isAviliable;
                    }
                }
                else
                {
                    product.Availability = null;
                }
            }
            catch (Exception ex)
            {
                //Log the exception
                log.LogInformation(ex.Message);
            }
            product.LastCheckedOn = DateTime.UtcNow;
        }

        public static string CreateProductEmailLine(ProductInfo product)
        {
            var emailBodyBuilder = new StringBuilder();
            emailBodyBuilder.Append($"<a href='{product.URL}' target='_blank'>{product.Name}</a> Price: {product.Price} {product.Currency} ");

            if (!string.IsNullOrEmpty(product.PreviousPrice))
                emailBodyBuilder.Append($"previous price: {product.PreviousPrice} {product.Currency}");

            if (product.Availability.HasValue)
                emailBodyBuilder.Append($" Availability: {product.Availability}");

            emailBodyBuilder.Append($" Checked on: { product.LastCheckedOn.ToShortDateString()}");

            return emailBodyBuilder.ToString();
        }
    }
}
