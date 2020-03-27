using ProductScraper.Models.EntityModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductScraper.Services.Interfaces
{
    public interface IProductInfoService
    {
        Task<IList<ProductInfo>> GetAllAsync(string userId);
    }
}
