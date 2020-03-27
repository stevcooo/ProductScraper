using ProductScraper.Models.EntityModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductScraper.Services.Interfaces
{
    public interface IProductInfoService
    {
        Task<IList<ProductInfo>> GetAllAsync(string userId);
        Task<ProductInfo> GetDetailsAsync(string userId, int id);
        Task AddAsync(string userId, ProductInfo productInfo);
        Task UpdateAsync(string userId, ProductInfo productInfo);
        Task DeleteAsync(string userId, int id);
    }
}
