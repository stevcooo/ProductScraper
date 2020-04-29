using ProductScraper.Models.EntityModels;
using System.Threading.Tasks;

namespace ProductScraper.Services.Interfaces
{
    public interface IUserProfileService
    {
        Task<UserProfile> GetByUserId(string userId);
        Task AddAsync(UserProfile userProfile);
        Task UpdateAsync(UserProfile userProfile);
        Task<int> GetUsersCount();
    }
}
