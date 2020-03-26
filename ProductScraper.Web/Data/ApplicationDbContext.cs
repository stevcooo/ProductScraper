using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProductScraper.Web.EntityModels;

namespace ProductScraper.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ProductInfo> ProductInfos { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
    }
}
