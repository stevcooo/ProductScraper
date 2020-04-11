using ProductScraper.Models.ViewModels;
using System.Threading.Tasks;

namespace ProductScraper.Services.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmail(EmailMessage message);
    }
}
