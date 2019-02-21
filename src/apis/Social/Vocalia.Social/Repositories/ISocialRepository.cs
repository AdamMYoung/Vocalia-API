using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        Task<IEnumerable<object>> GetFeedAsync(string userId, int count);

        /// <summary>
        /// Returns feed information about a single user.
        /// </summary>
        /// <param name="userId">ID of the user.</param>
        /// <returns></returns>
        Task<DomainModels.User> GetUserAsync(string userId);
    }
}
