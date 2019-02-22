using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vocalia.Social.DomainModels;

namespace Vocalia.Social.Repositories
{
    public interface ISocialRepository
    {
        /// <summary>
        /// Returns the user's combined feed.
        /// </summary>
        /// <param name="userId">ID of the user.</param>
        /// <param name="count">Number of elements to get.</param>
        /// <returns></returns>
        Task<IEnumerable<Listen>> GetTimelineFeedAsync(string userId, int count);

        /// <summary>
        /// Returns user information for a specific user.
        /// </summary>
        /// <param name="userId">ID of the user.</param>
        /// <returns></returns>
        Task<User> GetUserAsync(string userId);

        /// <summary>
        /// Returns feed information for a speficic user
        /// </summary>
        /// <param name="userId">ID of the user.</param>
        /// <param name="count">Number of elements to get.</param>
        /// <returns></returns>
        Task<IEnumerable<Listen>> GetUserFeedAsync(string userId, int count);
    }
}
