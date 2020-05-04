using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductScraper.Common.Naming;
using ProductScraper.Helpers;
using ProductScraper.Models.EntityModels;
using ProductScraper.Services.Interfaces;
using System.Threading.Tasks;

namespace ProductScraper.Areas.Admin.Controllers
{
    [Authorize(Policy = Policy.AdminOnly)]
    public class ScrapeConfigsController : Controller
    {
        private readonly IScrapeConfigService _scrapeConfigService;

        public ScrapeConfigsController(IScrapeConfigService scrapeConfigService)
        {
            _scrapeConfigService = scrapeConfigService;
        }

        // GET: ScrapeConfigs
        public async Task<IActionResult> Index()
        {
            return View(await _scrapeConfigService.GetAllAsync());
        }

        // GET: ScrapeConfigs/Details/5
        [Route("Details/{partitionKey}/{rowKey}")]
        public async Task<IActionResult> Details(string partitionKey, string rowKey)
        {
            if (string.IsNullOrWhiteSpace(partitionKey) || string.IsNullOrWhiteSpace(rowKey))
            {
                return NotFound();
            }
            ScrapeConfig scrapeConfig = await _scrapeConfigService.GetDetailsAsync(partitionKey, rowKey);
            if (scrapeConfig == null)
            {
                return NotFound();
            }

            return View(scrapeConfig);
        }

        // GET: ScrapeConfigs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ScrapeConfigs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,URL,ProductNamePath,ProductPricePath,Currency,ProductAvailabilityPath,ProductAvailabilityValue,ProductAvailabilityIsAtributeValue")] ScrapeConfig scrapeConfig)
        {
            if (ModelState.IsValid)
            {
                scrapeConfig.ProductNamePath = scrapeConfig.ProductNamePath.RemoveSpecialCharacters();
                scrapeConfig.ProductPricePath = scrapeConfig.ProductPricePath.RemoveSpecialCharacters();
                scrapeConfig.Currency = scrapeConfig.Currency;
                scrapeConfig.ProductAvailabilityPath = scrapeConfig.ProductAvailabilityPath.RemoveSpecialCharacters();
                await _scrapeConfigService.AddAsync(scrapeConfig);
                return RedirectToAction(nameof(Index));
            }
            return View(scrapeConfig);
        }

        // GET: ScrapeConfigs/Edit/5
        [Route("Edit/{partitionKey}/{rowKey}")]
        public async Task<IActionResult> Edit(string partitionKey, string rowKey)
        {
            if (string.IsNullOrWhiteSpace(partitionKey) || string.IsNullOrWhiteSpace(rowKey))
            {
                return NotFound();
            }
            ScrapeConfig scrapeConfig = await _scrapeConfigService.GetDetailsAsync(partitionKey, rowKey);
            if (scrapeConfig == null)
            {
                return NotFound();
            }
            return View(scrapeConfig);
        }

        // POST: ScrapeConfigs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit/{partitionKey}/{rowKey}")]
        public async Task<IActionResult> Edit(long id, [Bind("PartitionKey,RowKey,Id,Name,URL,ProductNamePath,ProductPricePath,Currency,ProductAvailabilityPath,ProductAvailabilityValue,ProductAvailabilityIsAtributeValue")] ScrapeConfig scrapeConfig)
        {
            if (id != scrapeConfig.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                scrapeConfig.ProductNamePath = scrapeConfig.ProductNamePath.RemoveSpecialCharacters();
                scrapeConfig.ProductPricePath = scrapeConfig.ProductPricePath.RemoveSpecialCharacters();
                scrapeConfig.Currency = scrapeConfig.Currency;
                scrapeConfig.ProductAvailabilityPath = scrapeConfig.ProductAvailabilityPath.RemoveSpecialCharacters();
                await _scrapeConfigService.UpdateAsync(scrapeConfig);
                return RedirectToAction(nameof(Index));
            }
            return View(scrapeConfig);
        }

        // GET: ScrapeConfigs/Delete/5
        [Route("Delete/{partitionKey}/{rowKey}")]
        public async Task<IActionResult> Delete(string partitionKey, string rowKey)
        {
            if (string.IsNullOrWhiteSpace(partitionKey) || string.IsNullOrWhiteSpace(rowKey))
            {
                return NotFound();
            }
            ScrapeConfig scrapeConfig = await _scrapeConfigService.GetDetailsAsync(partitionKey, rowKey);
            if (scrapeConfig == null)
            {
                return NotFound();
            }

            return View(scrapeConfig);
        }

        // POST: ScrapeConfigs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Delete/{partitionKey}/{rowKey}")]
        public async Task<IActionResult> DeleteConfirmed(string partitionKey, string rowKey)
        {
            if (string.IsNullOrWhiteSpace(partitionKey) || string.IsNullOrWhiteSpace(rowKey))
            {
                return NotFound();
            }
            await _scrapeConfigService.DeleteAsync(partitionKey, rowKey);
            return RedirectToAction(nameof(Index));
        }
    }
}
