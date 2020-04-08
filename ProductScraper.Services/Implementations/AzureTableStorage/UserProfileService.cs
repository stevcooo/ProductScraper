using ProductScraper.Models.EntityModels;
using ProductScraper.Services.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ProductScraper.Services.Implementations.AzureTableStorage
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IAzureTableStorage<UserProfile> _repository;
        
        public UserProfileService(IAzureTableStorage<UserProfile> repository)
        {
            _repository = repository;
        }

        public async Task AddAsync(UserProfile userProfile)
        {
            userProfile.Id = DateTime.Now.Ticks;
            userProfile.RowKey = userProfile.Id.ToString();
            userProfile.PartitionKey = userProfile.UserId;
            userProfile.LastNotificationEmailSendOn = DateTime.MaxValue;
            await _repository.Insert(userProfile);
        }

        public async Task<UserProfile> GetByUserId(string userId)
        {
            var users = await _repository.GetList(userId);
            if (users.Count != 1)
                throw new Exception("Too many users!");
            return users.FirstOrDefault();
        }

        public async Task UpdateAsync(UserProfile userProfile)
        {
            userProfile.RowKey = userProfile.Id.ToString();
            userProfile.PartitionKey = userProfile.UserId;
            await _repository.Update(userProfile);
        }
    }
}
