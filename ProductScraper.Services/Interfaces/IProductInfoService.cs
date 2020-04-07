using ProductScraper.Models.EntityModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductScraper.Services.Interfaces
{
    public interface IProductInfoService
    {
        Task<IList<ProductInfo>> GetAllAsync(string userId);
        Task<ProductInfo> GetDetailsAsync(string userId, long id);
        Task CheckAsync(string userId, long id);
        Task AddAsync(string userId, ProductInfo productInfo);
        Task UpdateAsync(string userId, ProductInfo productInfo);
        Task DeleteAsync(string userId, long id);
    }
}
