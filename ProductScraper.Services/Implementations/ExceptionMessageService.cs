using Microsoft.EntityFrameworkCore;
using ProductScraper.Models.EntityModels;
using ProductScraper.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductScraper.Services.Implementations
{
    public class ExceptionMessageService : IExceptionMessageService
    {
        private readonly IDbContext _context;

        public ExceptionMessageService(IDbContext context)
        {
            _context = context;
        }

        public async Task<IList<ExceptionMessage>> GetAllAsync()
        {
            return await _context.ExceptionMessages.ToListAsync();
        }

        public async Task AddAsync(ExceptionMessage exceptionMessage)
        {
            await _context.AddAsync(exceptionMessage);
            await _context.SaveChangesAsync();
        }        
    }
}
