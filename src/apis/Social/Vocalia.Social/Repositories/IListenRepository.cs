using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vocalia.Social.DomainModels;

namespace Vocalia.Social.Repositories
{
    interface IListenRepository
    {
        /// <summary>
        /// Returns the user's co mbined feed.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        Task<IEnumerable<SocialEntry>> GetUserFeedAsync(string userId, int count);

        /// <summary>
        /// Returns feed information about a single user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<User> GetSingleUserAsync(string userId);

    }
}
