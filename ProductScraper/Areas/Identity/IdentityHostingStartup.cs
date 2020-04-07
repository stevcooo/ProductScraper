using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(ProductScraper.Areas.Identity.IdentityHostingStartup))]
namespace ProductScraper.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}