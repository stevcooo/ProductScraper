using Microsoft.AspNetCore.Mvc;
using ProductScraper.Services.Interfaces;
using System.Threading.Tasks;

namespace ProductScraper.Controllers
{
    public class ExceptionMessagesController : Controller
    {
        private IExceptionMessageService _exceptionMessageService;

        public ExceptionMessagesController(IExceptionMessageService exceptionMessageService)
        {
            _exceptionMessageService = exceptionMessageService;
        }

        // GET: ExceptionMessages
        public async Task<IActionResult> Index()
        {
            return View(await _exceptionMessageService.GetAllAsync());
        }
    }
}
