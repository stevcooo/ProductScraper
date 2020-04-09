using Microsoft.Extensions.Options;
using ProductScraper.Models.EntityModels;
using ProductScraper.Models.ViewModels;
using ProductScraper.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ProductScraper.Services.Implementations.AzureTableStorage
{
    public class ProductInfoService : IProductInfoService
    {
        private readonly IAzureTableStorage<ProductInfo> _repository;
        private readonly IOptions<AppSettings> _settings;

        public ProductInfoService(IAzureTableStorage<ProductInfo> repository, IOptions<AppSettings> settings)
        {
            _repository = repository;
            _settings = settings;
        }

        public async Task AddAsync(string userId, ProductInfo productInfo)
        {
            productInfo.Id = DateTime.Now.Ticks;
            productInfo.RowKey = productInfo.Id.ToString();
            productInfo.PartitionKey = userId;
            await _repository.Insert(productInfo);
        }

        public async Task CheckAsync(string userId, long id)
        {
            var Url = _settings.Value.AzureFunctionURL;
            Url += $"ScrapeProduct/{userId}/{id}";
            Url += _settings.Value.AzureFunctionCode;

            CancellationToken cancellationToken;

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Get, Url))
            //using (var httpContent = HttpContentHelper.Create(content))
            {
                //request.Content = httpContent;

                using (var response = await client
                    .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                    .ConfigureAwait(false))
                {

                }
            }
        }

        public async Task DeleteAsync(string userId, long id)
        {
            await _repository.Delete(userId, id.ToString());
        }

        public async Task<IList<ProductInfo>> GetAllAsync(string userId)
        {
            return await _repository.GetList(userId);
        }

        public async Task<ProductInfo> GetDetailsAsync(string userId, long id)
        {
            return await _repository.GetItem(userId, id.ToString());            
        }

        public async Task UpdateAsync(string userId, ProductInfo productInfo)
        {
            productInfo.RowKey = productInfo.Id.ToString();
            productInfo.PartitionKey = userId;
            await _repository.Update(productInfo);            
        }
    }
}
