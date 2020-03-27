using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ProductScraper.Models.EntityModels;
using System.Threading;
using System.Threading.Tasks;

namespace ProductScraper.Services.Interfaces
{
    public interface IDbContext
    {
        ValueTask<EntityEntry<TEntity>> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        EntityEntry Update(object entity);

        DbSet<ProductInfo> ProductInfos { get; set; }
        DbSet<UserProfile> UserProfiles { get; set; }
    }
}
