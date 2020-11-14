using ProductScraper.Models.EntityModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductScraper.Services.Interfaces
{
    public interface IProductInfoHistoryService
    {
        Task<IList<ProductInfoHistory>> GetAllForProductAsync(long productId);
        Task<ProductInfoHistory> GetDetailsAsync(long productId, long id);
        Task AddAsync(long productId, ProductInfoHistory history);
    }
}
