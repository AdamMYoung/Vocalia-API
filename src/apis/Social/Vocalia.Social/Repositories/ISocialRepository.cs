using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vocalia.Social.DomainModels;

namespace Vocalia.Social.Repositories
{
    internal interface ISocialRepository
    {
        /// <summary>
        /// Returns the user's combined feed.
        /// </summary>
        /// <param name="userId">ID of the user.</param>
        /// <param name="count">Number of elements to get.</param>
        /// <returns></returns>
        Task<IEnumerable<SocialEntry>> GetFeedAsync(string userId, int count);

        /// <summary>
        /// Returns feed information about a single user.
        /// </summary>
        /// <param name="userId">ID of the user.</param>
        /// <returns></returns>
        Task<User> GetUserAsync(string userId);
    }
}
