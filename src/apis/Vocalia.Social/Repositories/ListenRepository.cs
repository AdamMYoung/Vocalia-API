using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vocalia.Social.DomainModels;

namespace Vocalia.Social.Repositories
{
    public class ListenRepository : IListenRepository
    {
        public Task<User> GetSingleUserAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<SocialEntry>> GetUserFeedAsync(string userId, int count)
        {
            throw new NotImplementedException();
        }
    }
}
