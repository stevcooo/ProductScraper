using ProductScraper.Models.EntityModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductScraper.Services.Interfaces
{
    public interface IExceptionMessageService
    {
        Task<IList<ExceptionMessage>> GetAllAsync();
        Task AddAsync(ExceptionMessage scrapeConfig);        
    }
}
