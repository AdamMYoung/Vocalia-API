using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vocalia.Social.DomainModels;

namespace Vocalia.Social.Repositories
{
    internal class SocialRepository : ISocialRepository
    {
        /// <summary>
        /// Returns the user's combined feed.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public Task<IEnumerable<object>> GetFeedAsync(string userId, int count)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns feed information about a single user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<User> GetUserAsync(string userId)
        {
            throw new NotImplementedException();
        }
    }
}
