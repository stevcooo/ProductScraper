using ProductScraper.Models.ViewModels;

namespace ProductScraper.Services.Interfaces
{
    public interface IEmailSender
    {
        void SendEmail(EmailMessage message);
    }
}
