using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using ProductScraper.Common.Naming;
using ProductScraper.Functions.Common;
using ProductScraper.Models.EntityModels;
using ProductScraper.Models.Extensions;
using ProductScraper.Models.ViewModels;
using SendGrid.Helpers.Mail;
using System;
using System.Linq;
using System.Text;

namespace ProductScraper.Functions.ScrapeFunctions
{
    public static class ScrapeUserProducts
    {
        [FunctionName(FunctionName.ScrapeUserProducts)]
        public static async void Run(
            [QueueTrigger(QueueName.UsersReadyForNotifications, Connection = CommonName.Connection)]UserProfile userProfile,
            [Queue(QueueName.AddProductHistory)] IAsyncCollector<ProductInfo> addProductHistoryMessageQueue,
            [Queue(QueueName.ProductUpdateEmailNotifications)] IAsyncCollector<EmailMessage> emailMessageQueue,
            [Queue(QueueName.EmailsToSend)] IAsyncCollector<SendGridMessage> sendGridMessageQueue,
            IBinder binder,
            ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {userProfile.FirstName}");

            CloudTable productInfoTable = await binder.BindAsync<CloudTable>(new TableAttribute(TableName.ProductInfo, userProfile.UserId)
            {
                Connection = CommonName.Connection
            });

            CloudTable scrapeConfigTable = await binder.BindAsync<CloudTable>(new TableAttribute(TableName.ScrapeConfig)
            {
                Connection = CommonName.Connection
            });

            TableQuery<ProductInfo> productQuery = new TableQuery<ProductInfo>();
            TableQuerySegment<ProductInfo> userProducts = await productInfoTable.ExecuteQuerySegmentedAsync(productQuery, null);

            //Load all configs in the begginign/maybe this should be changed in the future
            TableQuery<ScrapeConfig> configsQuery = new TableQuery<ScrapeConfig>();
            TableQuerySegment<ScrapeConfig> allConfigs = await scrapeConfigTable.ExecuteQuerySegmentedAsync(configsQuery, null);

            EmailMessage emailMessage;
            StringBuilder emailBodyBuilder = new StringBuilder();
            
            log.LogInformation($"userProducts: {userProducts.Results.Count}");
            foreach (ProductInfo product in userProducts)
            {
                //Find config from allConfigs
                ScrapeConfig config = allConfigs.FirstOrDefault(t => t.PartitionKey.Equals(product.URL.ToCoreUrl()));                
                if (config != null)
                {
                    log.LogInformation($"ScrapeConfig : {config.Name}");
                    Utils utils = new Utils();
                    try
                    {
                        await utils.Scrape(config, product, log);
                    }
                    catch(Exception ex)
                    {
                        string errorMsg = ex.Message;
                        
                        while (ex.InnerException != null)
                        {
                            errorMsg += Environment.NewLine + ex.InnerException.Message;
                            ex = ex.InnerException;
                        }

                        var error = $"Error while trying to scrape product = {product.Name}, URL={product.URL}. Error: {ex.Message}";
                        log.LogError(error);
                        SendGridMessage message = new SendGridMessage();
                        message.AddTo("stevcooo@gmail.com");
                        message.AddContent("text/html", error);
                        message.SetFrom(new EmailAddress("stevan@kostoski.com"));
                        message.SetSubject("Product scrape exception");
                        await sendGridMessageQueue.AddAsync(message);
                    }

                    if (product.HasChangesSinceLastTime)
                    {
                        var productUpdateLine = utils.CreateProductEmailLine(product);
                        emailBodyBuilder.AppendLine(productUpdateLine);
                        emailBodyBuilder.AppendLine("<br>");

                        //Add to history queue
                        await addProductHistoryMessageQueue.AddAsync(product);
                    }

                    //Update product in db
                    TableOperation operation = TableOperation.InsertOrReplace(product);
                    await productInfoTable.ExecuteAsync(operation);
                }
                else
                {
                    log.LogInformation($"Multiple scrape config matches the criteria URL={product.URL}");
                }
            }
            if (emailBodyBuilder.Length > 0)
            {
                emailBodyBuilder.AppendLine();
                emailBodyBuilder.AppendLine("<br>");
                emailBodyBuilder.AppendLine("<br>");
                emailBodyBuilder.AppendLine("<a href='https://product-scrape.azurewebsites.net/Products'>Here you can see the list of your products</a>");
                emailMessage = new EmailMessage
                {
                    UserId = userProfile.UserId,
                    Subject = "Products updates",
                    Content = emailBodyBuilder.ToString()
                };

                log.LogInformation($"EmailMessage Product updates");
                await emailMessageQueue.AddAsync(emailMessage);
            }
            else if (userProfile.SendEmailWhenNoProductHasBeenChanged)
            {
                log.LogInformation($"EmailMessage No Product update");
                emailMessage = new EmailMessage
                {
                    UserId = userProfile.UserId,
                    Subject = "Products updates",
                    Content = "None of your products has been updated/changed since last check."
                };
                await emailMessageQueue.AddAsync(emailMessage);
            }
        }
    }
}
