using Microsoft.EntityFrameworkCore;
using ProductScraper.Models.EntityModels;
using ProductScraper.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductScraper.Services.Implementations
{
    public class ProductInfoService : IProductInfoService
    {
        private readonly IDbContext _context;

        public ProductInfoService(IDbContext context)
        {
            _context = context;

        }

        public async Task<IList<ProductInfo>> GetAllAsync(string userId)
        {
            return await _context.ProductInfos
                .Include(p => p.UserProfile)
                .Where(m => m.UserProfile.UserId == userId).ToListAsync();
        }
    }
}
