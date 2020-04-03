using HtmlAgilityPack;
using ProductScraper.Models.EntityModels;
using ProductScraper.Models.ViewModels;
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
        private readonly IExceptionMessageService _exceptionMessageService;
        private readonly IEmailSender _emailSender;
        private readonly WebClient _webClient;

        public ScrapeService(IScrapeConfigService scrapeConfigService, 
            IEmailSender emailSender, IExceptionMessageService exceptionMessageService)
        {
            _scrapeConfigService = scrapeConfigService;
            _exceptionMessageService = exceptionMessageService;
            _emailSender = emailSender;
            _webClient = new WebClient();
        }

        public async Task ScrapeProductInfoAsync(ProductInfo product)
        {
            var allConfigs = await _scrapeConfigService.GetAllAsync();
            var configs = allConfigs.Where(t => product.URL.Contains(t.URL)).ToList();
            if (configs.Any())
            {
                if (configs.Count > 1)
                    SendEmailToAdmin("Duplicate condifuration", $"For url {product.URL} there are multiple configurations!");

                await Scrape(configs.FirstOrDefault(), product);
            }
            else
            {
                SendEmailToAdmin("Missing condifuration", $"Missing configuration for URL: {product.URL}, UserProfileId: {product.UserProfileId}");
                throw new ScrapeServiceException("Sorry, currently we don't support this URL, as soon as we support it, we will let you know.");
            }
        }

        private async Task Scrape(ScrapeConfig scrapeConfig, ProductInfo product)
        {
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
                await _exceptionMessageService.AddAsync(new ExceptionMessage()
                {
                    Message = ex.Message,
                    Method = this.GetType().Name,
                    ProductId = product.Id,
                    UserId = product.UserProfile.UserId
                });
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
                await _exceptionMessageService.AddAsync(new ExceptionMessage()
                {
                    Message = ex.Message,
                    Method = this.GetType().Name,
                    ProductId = product.Id,
                    UserId = product.UserProfile.UserId
                });
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
                await _exceptionMessageService.AddAsync(new ExceptionMessage()
                {
                    Message = ex.Message,
                    Method = this.GetType().Name,
                    ProductId = product.Id,
                    UserId = product.UserProfile.UserId
                });
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
                await _exceptionMessageService.AddAsync(new ExceptionMessage()
                {
                    Message = ex.Message,
                    Method = this.GetType().Name,
                    ProductId = product.Id,
                    UserId = product.UserProfile.UserId
                });
            }
            product.LastCheckedOn = DateTime.UtcNow;

            if (product.HasChangesSinceLastTime)
            {
                NotifyUserAboutChanges(product);
            }
        }

        private void NotifyUserAboutChanges(ProductInfo product)
        {
            var userId = product.UserProfile.UserId;
            var msg = new EmailMessage("stevcooo@gmail.com", $"Changes in your product {product.Name}", 
                $"The product {product.Name} has changes. You can check them on this link: {product.URL}");
            _emailSender.SendEmail(msg);
        }

        private void SendEmailToAdmin(string subject, string content)
        {
            var msg = new EmailMessage("stevcooo@gmail.com", subject, content);
            _emailSender.SendEmail(msg);
        }
    }
}
