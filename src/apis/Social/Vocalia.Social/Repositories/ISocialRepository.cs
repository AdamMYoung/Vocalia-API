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
        /// Returns detailed user information for a specific user.
        /// </summary>
        /// <param name="userTag">Tag of the user.</param>
        /// <param name="accessToken">Auth token for user access.</param>
        /// <returns></returns>
        Task<User> GetUserDetailAsync(string userUid);
    }
}
