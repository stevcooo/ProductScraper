using Microsoft.EntityFrameworkCore;
using ProductScraper.Models.EntityModels;
using ProductScraper.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductScraper.Services.Implementations
{
    public class ProductInfoService : IProductInfoService
    {
        private readonly IDbContext _context;
        private readonly IScrapeService _scrapeService;

        public ProductInfoService(IDbContext context, IScrapeService scrapeService)
        {
            _context = context;
            _scrapeService = scrapeService;
        }

        public async Task<IList<ProductInfo>> GetAllAsync(string userId)
        {
            return await _context.ProductInfos
                .Include(p => p.UserProfile)
                .Where(m => m.UserProfile.UserId == userId).ToListAsync();
        }

        public async Task<ProductInfo> GetDetailsAsync(string userId, long id)
        {
            return await _context.ProductInfos
                .Include(p => p.UserProfile)
                .Where(m => m.UserProfile.UserId == userId && m.Id == id).FirstOrDefaultAsync();
        }

        public async Task AddAsync(string userId, ProductInfo productInfo)
        {
            var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(t => t.UserId == userId);
            productInfo.UserProfileId = userProfile.Id;

            await _scrapeService.ScrapeProductInfoAsync(productInfo);            
            await _context.AddAsync(productInfo);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(string userId, ProductInfo productInfo)
        {
            var product = await _context.ProductInfos
                .Include(p => p.UserProfile)
                .FirstOrDefaultAsync(m => m.UserProfile.UserId == userId && m.Id == productInfo.Id);
            if (product == null)
                throw new Exception("Item not found");

            product.URL = productInfo.URL;
            await _scrapeService.ScrapeProductInfoAsync(product);
            try
            {
                _context.Update(product);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(productInfo.Id))
                {
                    throw new Exception("Item not found");
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task DeleteAsync(string userId, long id)
        {
            var productInfo = await _context.ProductInfos
                .Include(p => p.UserProfile)
                .FirstOrDefaultAsync(m => m.UserProfile.UserId == userId && m.Id == id);
            if (productInfo == null)
                throw new Exception("Item not found");

            _context.ProductInfos.Remove(productInfo);
            await _context.SaveChangesAsync();
        }

        public async Task CheckAsync(string userId, long id)
        {
            var productInfo = await _context.ProductInfos
                .Include(p => p.UserProfile)
                .FirstOrDefaultAsync(m => m.UserProfile.UserId == userId && m.Id == id);
            if (productInfo == null)
                throw new Exception("Item not found");

            await _scrapeService.ScrapeProductInfoAsync(productInfo);
            try
            {
                _context.Update(productInfo);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(productInfo.Id))
                {
                    throw new Exception("Item not found");
                }
                else
                {
                    throw;
                }
            }
        }

        private bool Exists(long id)
        {
            return _context.ProductInfos.Any(e => e.Id == id);
        }
    }
}
