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
        /// <param name="userTag">Tag of the user.</param>
        /// <returns></returns>
        Task<User> GetUserAsync(string userTag);

        /// <summary>
        /// Returns feed information for a speficic user.
        /// </summary>
        /// <param name="userTag">Tag of the user.</param>
        /// <param name="count">Number of elements to get.</param>
        /// <returns></returns>
        Task<IEnumerable<Listen>> GetUserFeedAsync(string userTag, int count);

        /// <summary>
        /// Returns all followings of the specified user tag.
        /// </summary>
        /// <param name="userId">ID of the user.</param>
        /// <returns></returns>
        Task<IEnumerable<User>> GetFollowingsAsync(string userTag);

        /// <summary>
        /// Returns all followers of the specified user tag.
        /// </summary>
        /// <param name="userId">ID of the user.</param>
        /// <returns></returns>
        Task<IEnumerable<User>> GetFollowersAsync(string userTag);

        /// <summary>
        /// Removes the specified user from the users' following list.
        /// </summary>
        /// <param name="userId">User to remove following from.</param>
        /// <param name="friendId">User to remove.</param>
        /// <returns></returns>
        Task RemoveFollowingAsync(string userId, string followingTag);

        /// <summary>
        /// Adds the specified user to the users' following list.
        /// </summary>
        /// <param name="userId">User to add following to.</param>
        /// <param name="friendId">User to add.</param>
        /// <returns></returns>
        Task AddFollowingAsync(string userId, string followingTag);
    }
}
