using ProductScraper.Models.ViewModels;
using System.Threading.Tasks;

namespace ProductScraper.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmail(EmailMessage message);
    }
}
