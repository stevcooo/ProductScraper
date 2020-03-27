using Microsoft.EntityFrameworkCore;
using ProductScraper.Models.EntityModels;

namespace ProductScraper.Services.Interfaces
{
    public interface IDbContext
    {
        DbSet<ProductInfo> ProductInfos { get; set; }
        DbSet<UserProfile> UserProfiles { get; set; }
    }
}
