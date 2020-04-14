using Microsoft.Extensions.Options;
using ProductScraper.Common.Naming;
using ProductScraper.Models.EntityModels;
using ProductScraper.Models.Extensions;
using ProductScraper.Models.ViewModels;
using ProductScraper.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductScraper.Services.Implementations
{
    public class ScrapeConfigService : IScrapeConfigService
    {
        private readonly IOptions<AppSettings> _settings;
        private readonly IHttpHandlerService _httpHandlerService;

        public ScrapeConfigService(IOptions<AppSettings> settings,
            IHttpHandlerService httpHandlerService)
        {
            _settings = settings;
            _httpHandlerService = httpHandlerService;
        }

        public async Task AddAsync(ScrapeConfig scrapeConfig)
        {
            scrapeConfig.Id = DateTime.Now.Ticks;
            scrapeConfig.RowKey = scrapeConfig.Id.ToString();
            scrapeConfig.PartitionKey = scrapeConfig.URL.ToCoreUrl();

            string url = _settings.Value.AzureFunctionURL + FunctionName.AddScrapeConfig + "/" + _settings.Value.AzureFunctionCode;
            await _httpHandlerService.HandlePostRequest(url, scrapeConfig);
        }

        public async Task DeleteAsync(string partitionKey, string rowKey)
        {
            string url = _settings.Value.AzureFunctionURL + FunctionName.DeleteScrapeConfig + $"/{partitionKey}/{rowKey}/" + _settings.Value.AzureFunctionCode;
            await _httpHandlerService.HandlePostRequest(url, null);
        }

        public async Task<IList<ScrapeConfig>> GetAllAsync()
        {
            string url = _settings.Value.AzureFunctionURL + FunctionName.GetAllScrapeConfigs + "/" + _settings.Value.AzureFunctionCode;
            return await _httpHandlerService.HandleGetRequest<IList<ScrapeConfig>>(url);
        }

        public async Task<ScrapeConfig> GetDetailsAsync(string partitionKey, string rowKey)
        {
            string url = _settings.Value.AzureFunctionURL + FunctionName.GetScrapeConfig + $"/{partitionKey}/{rowKey}/" + _settings.Value.AzureFunctionCode;
            return await _httpHandlerService.HandleGetRequest<ScrapeConfig>(url);
        }

        public async Task UpdateAsync(ScrapeConfig scrapeConfig)
        {
            string url = _settings.Value.AzureFunctionURL + FunctionName.UpdateScrapeConfig + "/" + _settings.Value.AzureFunctionCode;
            await _httpHandlerService.HandlePostRequest(url, scrapeConfig);
        }
    }
}
