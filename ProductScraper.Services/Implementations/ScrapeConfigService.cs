using Microsoft.EntityFrameworkCore;
using ProductScraper.Models.EntityModels;
using ProductScraper.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductScraper.Services.Implementations
{
    public class ScrapeConfigService : IScrapeConfigService
    {
        private readonly IDbContext _context;

        public ScrapeConfigService(IDbContext context)
        {
            _context = context;
        }

        public async Task<IList<ScrapeConfig>> GetAllAsync()
        {
            return await _context.ScrapeConfigs.ToListAsync();
        }

        public async Task<ScrapeConfig> GetDetailsAsync(long id)
        {
            return await _context.ScrapeConfigs.FirstOrDefaultAsync(t=>t.Id == id);
        }

        public async Task AddAsync(ScrapeConfig scrapeConfig)
        {
            await _context.AddAsync(scrapeConfig);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ScrapeConfig scrapeConfig)
        {
            try
            {
                _context.Update(scrapeConfig);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(scrapeConfig.Id))
                {
                    throw new Exception("Item not found");
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task DeleteAsync(long id)
        {
            var item = await _context.ScrapeConfigs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
                throw new Exception("Item not found");

            _context.ScrapeConfigs.Remove(item);
            await _context.SaveChangesAsync();
        }

        private bool Exists(long id)
        {
            return _context.ScrapeConfigs.Any(e => e.Id == id);
        }
    }
}
