﻿using Microsoft.Extensions.Options;
using ProductScraper.Common;
using ProductScraper.Models.EntityModels;
using ProductScraper.Models.Extensions;
using ProductScraper.Models.ViewModels;
using ProductScraper.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductScraper.Services.Implementations.AzureTableStorage
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
            
            var url = _settings.Value.AzureFunctionURL + FunctionsNames.AddScrapeConfig + "/" + _settings.Value.AzureFunctionCode;
            await _httpHandlerService.HandlePostRequest(url, scrapeConfig);
        }

        public async Task DeleteAsync(string partitionKey, string rowKey)
        {
            var url = _settings.Value.AzureFunctionURL + FunctionsNames.DeleteScrapeConfig + $"/{partitionKey}/{rowKey}/" + _settings.Value.AzureFunctionCode;
            await _httpHandlerService.HandlePostRequest(url, null);
        }

        public async Task<IList<ScrapeConfig>> GetAllAsync()
        {
            var url = _settings.Value.AzureFunctionURL + FunctionsNames.GetAllScrapeConfigs + "/" + _settings.Value.AzureFunctionCode;
            return await _httpHandlerService.HandleGetRequest<IList<ScrapeConfig>>(url);
        }

        public async Task<ScrapeConfig> GetDetailsAsync(string partitionKey, string rowKey)
        {
            var url = _settings.Value.AzureFunctionURL + FunctionsNames.GetScrapeConfig + $"/{partitionKey}/{rowKey}/" + _settings.Value.AzureFunctionCode;
            return await _httpHandlerService.HandleGetRequest<ScrapeConfig>(url);
        }

        public async Task UpdateAsync(ScrapeConfig scrapeConfig)
        {
            var url = _settings.Value.AzureFunctionURL + FunctionsNames.UpdateScrapeConfig + "/" + _settings.Value.AzureFunctionCode;
            await _httpHandlerService.HandlePostRequest(url, scrapeConfig);
        }
    }
}
