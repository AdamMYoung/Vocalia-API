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
        /// <param name="accessToken">Auth token for user access.</param>
        /// <returns></returns>
        Task<IEnumerable<Listen>> GetTimelineFeedAsync(string userId, int count);

        /// <summary>
        /// Returns user information for a specific user.
        /// </summary>
        /// <param name="userTag">Tag of the user.</param>
        /// <param name="accessToken">Auth token for user access.</param>
        /// <returns></returns>
        Task<User> GetUserAsync(string userUid);

        /// <summary>
        /// Returns feed information for a speficic user.
        /// </summary>
        /// <param name="userTag">Tag of the user.</param>
        /// <param name="count">Number of elements to get.</param>
        /// <param name="accessToken">Auth token for user access.</param>
        /// <returns></returns>
        Task<IEnumerable<Listen>> GetUserFeedAsync(string userUid, int count);

        /// <summary>
        /// Returns all followings of the specified user tag.
        /// </summary>
        /// <param name="userId">ID of the user.</param>
        /// <param name="accessToken">Auth token for user access.</param>
        /// <returns></returns>
        Task<IEnumerable<User>> GetFollowingsAsync(string userUid);

        /// <summary>
        /// Returns all followers of the specified user tag.
        /// </summary>
        /// <param name="userId">ID of the user.</param>
        /// <param name="accessToken">Auth token for user access.</param>
        /// <returns></returns>
        Task<IEnumerable<User>> GetFollowersAsync(string userUid);

        /// <summary>
        /// Removes the specified user from the users' following list.
        /// </summary>
        /// <param name="userId">User to remove following from.</param>
        /// <param name="friendId">User to remove.</param>
        /// <returns></returns>
        Task RemoveFollowingAsync(string userUid, string followingUid);

        /// <summary>
        /// Adds the specified user to the users' following list.
        /// </summary>
        /// <param name="userId">User to add following to.</param>
        /// <param name="friendId">User to add.</param>
        /// <returns></returns>
        Task AddFollowingAsync(string userUid, string followingUid);
    }
}
