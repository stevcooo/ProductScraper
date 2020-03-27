using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductScraper.Models.EntityModels;
using ProductScraper.Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProductScraper.Web.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly IProductInfoService _productInfoService;
        public ProductsController(IProductInfoService productInfoService)
        {
            _productInfoService = productInfoService;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View(await _productInfoService.GetAllAsync(_userId));
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var productInfo = await _productInfoService.GetDetailsAsync(_userId, id.Value);
            if (productInfo == null)
            {
                return NotFound();
            }

            return View(productInfo);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("URL")] ProductInfo productInfo)
        {
            //ViewData["UserProfileId"] = new SelectList(_context.UserProfiles, "Id", "Id");

            if (ModelState.IsValid)
            {
                var _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _productInfoService.AddAsync(_userId, productInfo);                
                return RedirectToAction(nameof(Index));
            }            
            return View(productInfo);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var productInfo = await _productInfoService.GetDetailsAsync(_userId, id.Value);
            if (productInfo == null)
            {
                return NotFound();
            }            
            return View(productInfo);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,URL")] ProductInfo productInfo)
        {
            if (id != productInfo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _productInfoService.UpdateAsync(_userId, productInfo);
                return RedirectToAction(nameof(Index));
            }
            return View(productInfo);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var productInfo = await _productInfoService.GetDetailsAsync(_userId, id.Value);
            if (productInfo == null)
            {
                return NotFound();
            }

            return View(productInfo);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _productInfoService.DeleteAsync(_userId, id);
            return RedirectToAction(nameof(Index));
        }
    }
}
