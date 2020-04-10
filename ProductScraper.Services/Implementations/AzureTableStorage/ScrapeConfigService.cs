using Microsoft.Extensions.Options;
using ProductScraper.Common;
using ProductScraper.Models.EntityModels;
using ProductScraper.Models.Extensions;
using ProductScraper.Models.ViewModels;
using ProductScraper.Services.Helpers;
using ProductScraper.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ProductScraper.Services.Implementations.AzureTableStorage
{
    public class ScrapeConfigService : IScrapeConfigService
    {
        private readonly IAzureTableStorage<ScrapeConfig> _repository;
        private readonly IOptions<AppSettings> _settings;

        public ScrapeConfigService(IOptions<AppSettings> settings, IAzureTableStorage<ScrapeConfig> repository)
        {
            _repository = repository;
            _settings = settings;
        }

        public async Task AddAsync(ScrapeConfig scrapeConfig)
        {
            scrapeConfig.Id = DateTime.Now.Ticks;
            scrapeConfig.RowKey = scrapeConfig.Id.ToString();
            scrapeConfig.PartitionKey = scrapeConfig.URL.ToCoreUrl();
            
            CancellationToken cancellationToken;
            var url = _settings.Value.AzureFunctionURL + FunctionsNames.AddScrapeConfig + "/" + _settings.Value.AzureFunctionCode;
            using var client = new HttpClient();
            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            using var httpContent = HttpContentHelper.Create(scrapeConfig);
            request.Content = httpContent;

            using var response = await client
                .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                .ConfigureAwait(false);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                throw new Exception(await response.Content.ReadAsStringAsync());
        }

        public async Task DeleteAsync(string partitionKey, string rowKey)
        {
            await _repository.Delete(partitionKey, rowKey);
        }

        public async Task<IList<ScrapeConfig>> GetAllAsync()
        {
            return await _repository.GetList();
        }

        public async Task<ScrapeConfig> GetDetailsAsync(string partitionKey, string rowKey)
        {
            return await _repository.GetItem(partitionKey, rowKey);
        }

        public async Task UpdateAsync(ScrapeConfig scrapeConfig)
        {
            scrapeConfig.RowKey = scrapeConfig.Id.ToString();
            scrapeConfig.PartitionKey = scrapeConfig.URL.ToCoreUrl();
            await _repository.Update(scrapeConfig);            
        }
    }
}
