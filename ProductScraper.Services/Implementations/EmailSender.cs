using Microsoft.Extensions.Options;
using ProductScraper.Common.Naming;
using ProductScraper.Models.ViewModels;
using ProductScraper.Services.Interfaces;
using System.Threading.Tasks;

namespace ProductScraper.Services.Implementations
{
    public class EmailSender : IEmailSender
    {
        private IOptions<AppSettings> _settings;
        private IHttpHandlerService _httpHandlerService;

        public EmailSender(IOptions<AppSettings> settings,
            IHttpHandlerService httpHandlerService)
        {
            _settings = settings;
            _httpHandlerService = httpHandlerService;
        }

        public async Task SendEmail(EmailMessage message)
        {
            var url = _settings.Value.AzureFunctionURL + FunctionName.EmailSender + "/" + _settings.Value.AzureFunctionCode;
            await _httpHandlerService.HandlePostRequest(url, message);
        }       
    }
}
