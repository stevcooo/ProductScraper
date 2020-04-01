using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductScraper.Models.EntityModels;
using ProductScraper.Services.Interfaces;
using System.Threading.Tasks;

namespace ProductScraper.Areas.Admin.Controllers
{
    [Authorize]
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
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var scrapeConfig = await _scrapeConfigService.GetDetailsAsync(id.Value);
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
        public async Task<IActionResult> Create([Bind("Name,URL,ProductNamePath,ProductPricePath,ProductSecondPricePath,ProductAvailabilityPath")] ScrapeConfig scrapeConfig)
        {
            if (ModelState.IsValid)
            {
                await _scrapeConfigService.AddAsync(scrapeConfig);
                return RedirectToAction(nameof(Index));
            }
            return View(scrapeConfig);
        }

        // GET: ScrapeConfigs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var scrapeConfig = await _scrapeConfigService.GetDetailsAsync(id.Value);
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,URL,ProductNamePath,ProductPricePath,ProductSecondPricePath,ProductAvailabilityPath")] ScrapeConfig scrapeConfig)
        {
            if (id != scrapeConfig.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _scrapeConfigService.UpdateAsync(scrapeConfig);
                return RedirectToAction(nameof(Index));
            }
            return View(scrapeConfig);
        }

        // GET: ScrapeConfigs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var scrapeConfig = await _scrapeConfigService.GetDetailsAsync(id.Value);
            if (scrapeConfig == null)
            {
                return NotFound();
            }

            return View(scrapeConfig);
        }

        // POST: ScrapeConfigs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _scrapeConfigService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
