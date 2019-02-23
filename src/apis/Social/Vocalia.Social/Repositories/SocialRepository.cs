using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vocalia.Social.Db;
using Vocalia.Social.DomainModels;

namespace Vocalia.Social.Repositories
{
    internal class SocialRepository : ISocialRepository
    {
        /// <summary>
        /// DB context for social data.
        /// </summary>
        private SocialContext DbContext { get; }

        /// <summary>
        /// Instantiates a new SocialRepository.
        /// </summary>
        /// <param name="context"></param>
        public SocialRepository(SocialContext context)
        {
            DbContext = context;
        }

        #region Feed

        /// <summary>
        /// Returns the user's combined feed.
        /// </summary>
        /// <param name="userId">ID of the user.</param>
        /// <param name="count">Number of elements to get.</param>
        /// <returns></returns>
        public async Task<IEnumerable<DomainModels.Listen>> GetTimelineFeedAsync(string userId, int count)
        {
            var user = await DbContext.Users.FirstOrDefaultAsync(u => u.UserUID == userId && u.Active);

            if (user == null)
                return null;

            var followingUsers = user.Followings.Select(x => x.Following);

            // Get all listen entries for the users the selected user follows, order them by descending then get the newest entry for each episode.
            var feed = followingUsers
                .SelectMany(x => x.Listens
                .OrderByDescending(c => c.Date)
                .GroupBy(c => c.EpisodeUrl)
                .Select(e => e.FirstOrDefault())).Take(count);

            return feed.Select(x => new DomainModels.Listen
            {
                ID = x.ID,
                UserUID = x.UserUID,
                UserName = x.User.FirstName + " " + x.User.LastName,
                RssUrl = x.RssUrl,
                EpisodeUrl = x.EpisodeUrl,
                EpisodeName = x.EpisodeName,
                Date = x.Date,
                IsCompleted = x.IsCompleted
            });
        }

        /// <summary>
        /// Returns user information for a specific user.
        /// </summary>
        /// <param name="userId">ID of the user.</param>
        /// <returns></returns>
        public async Task<DomainModels.User> GetUserAsync(string userTag)
        {
            var user = await DbContext.Users.FirstOrDefaultAsync(u => u.UserTag == userTag && u.Active);

            if (user == null)
                return null;

            return new DomainModels.User
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Birthday = user.Birthday,
                UserUID = user.UserUID
            };
        }

        /// <summary>
        /// Returns feed information for a speficic user
        /// </summary>
        /// <param name="userId">ID of the user.</param>
        /// <param name="count">Number of items to return.</param>
        /// <returns></returns>
        public async Task<IEnumerable<DomainModels.Listen>> GetUserFeedAsync(string userTag, int count)
        {
            var user = await DbContext.Users.FirstOrDefaultAsync(u => u.UserTag == userTag && u.Active);
            if (user == null)
                return null;

            var feed = user.Listens.OrderByDescending(c => c.Date)
                .GroupBy(c => c.EpisodeUrl)
                .Select(e => e.FirstOrDefault()).Take(count);

            return feed.Select(x => new DomainModels.Listen
            {
                ID = x.ID,
                UserName = user.FirstName + " " + user.LastName,
                UserUID = x.UserUID,
                EpisodeName = x.EpisodeName,
                EpisodeUrl = x.EpisodeUrl,
                RssUrl = x.RssUrl,
                Date = x.Date,
                IsCompleted = x.IsCompleted
            });
        }

        #endregion

        #region Following

        /// <summary>
        /// Returns all followings of the specified user UID.
        /// </summary>
        /// <param name="userId">ID of the user.</param>
        /// <returns></returns>
        public async Task<IEnumerable<DomainModels.User>> GetFollowingsAsync(string userTag)
        {
            var user = await DbContext.Users.FirstOrDefaultAsync(x => x.UserTag == userTag);
            var followings = user.Followings.Select(x => x.Following);

            return followings.Select(c => new DomainModels.User
            {
                UserUID = c.UserUID,
                UserTag = c.UserTag,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Birthday = c.Birthday
            });
        }

        /// <summary>
        /// Returns all followers of the specified user tag.
        /// </summary>
        /// <param name="userId">ID of the user.</param>
        /// <returns></returns>
        public async Task<IEnumerable<DomainModels.User>> GetFollowersAsync(string userTag)
        {
            var user = await DbContext.Users.FirstOrDefaultAsync(x => x.UserTag == userTag);
            var follower = user.Followers.Select(x => x.Follower);

            return follower.Select(c => new DomainModels.User
            {
                UserUID = c.UserUID,
                UserTag = c.UserTag,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Birthday = c.Birthday
            });
        }

        /// <summary>
        /// Adds the specified user to the users' following list.
        /// </summary>
        /// <param name="userId">User to add following to.</param>
        /// <param name="friendId">User to add.</param>
        /// <returns></returns>
        public async Task AddFollowingAsync(string userId, string followingTag)
        {
            var user = await DbContext.Users.FirstOrDefaultAsync(x => x.UserUID == userId);
            var following = await DbContext.Users.FirstOrDefaultAsync(x => x.UserTag == followingTag);

            await DbContext.Follows.AddAsync(new Follow
            {
                UserUID = user.UserUID,
                FollowUID = user.UserUID
            });

            await DbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Removes the specified user from the users' following list.
        /// </summary>
        /// <param name="userId">User to remove following from.</param>
        /// <param name="friendId">User to remove.</param>
        /// <returns></returns>
        public async Task RemoveFollowingAsync(string userId, string followingTag)
        {
            var user = await DbContext.Users.FirstOrDefaultAsync(x => x.UserUID == userId);
            var following = await DbContext.Users.FirstOrDefaultAsync(x => x.UserTag == followingTag);

            var entry = user.Followings.FirstOrDefault(x => x.UserUID == user.UserUID && x.FollowUID == following.UserUID);
            if (entry != null)
                DbContext.Follows.Remove(entry);

            await DbContext.SaveChangesAsync();
        }

        #endregion
    }
}
