using Microsoft.AspNetCore.Mvc;
using ProductScraper.Models.ViewModels;
using ProductScraper.Services.Interfaces;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProductScraper.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductInfoService _productInfoService;

        public HomeController(IProductInfoService productInfoService)
        {
            _productInfoService = productInfoService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                string _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                return View(await _productInfoService.GetAllAsync(_userId));
            }
            catch
            {

            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
