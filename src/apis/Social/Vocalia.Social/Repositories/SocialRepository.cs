using Microsoft.EntityFrameworkCore;
using RestSharp;
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
        public async Task<IEnumerable<DomainModels.Listen>> GetTimelineFeedAsync(string userId, int count, string accessToken)
        {
            var followingUsers = await DbContext.Follows.Where(x => x.UserUID == userId).ToListAsync();

            // Get all listen entries for the users the selected user follows, order them by descending then get the newest entry for each episode.
            var feed = followingUsers
                .SelectMany(x => DbContext.Listens.Where(c => c.UserUID == x.UserUID)
                .OrderByDescending(c => c.Date)
                .GroupBy(c => c.EpisodeUrl)
                .Select(e => e.FirstOrDefault())).Take(count);

            var timeline = feed.Select(x => new DomainModels.Listen
            {
                ID = x.ID,
                UserUID = x.UserUID,
                RssUrl = x.RssUrl,
                EpisodeUrl = x.EpisodeUrl,
                EpisodeName = x.EpisodeName,
                Date = x.Date,
                IsCompleted = x.IsCompleted
            });

            foreach (var user in timeline)
            {
                var auth0Entry = await UserFacade.Auth0.GetUserAsync(userId, accessToken);
                user.UserName = auth0Entry.name;
            }

            return timeline;
        }

        /// <summary>
        /// Returns user information for a specific user.
        /// </summary>
        /// <param name="userId">ID of the user.</param>
        /// <returns></returns>
        public async Task<DomainModels.User> GetUserAsync(string userId, string accessToken)
        {
            var user = await UserFacade.Auth0.GetUserAsync(userId, accessToken);

            return new DomainModels.User
            {
                FirstName = user.given_name,
                LastName = user.family_name,
                UserUID = user.user_id,
                UserTag = user.nickname,
                PictureUrl = user.picture
            };
        }

        /// <summary>
        /// Returns feed information for a speficic user
        /// </summary>
        /// <param name="userId">ID of the user.</param>
        /// <param name="count">Number of items to return.</param>
        /// <returns></returns>
        public async Task<IEnumerable<DomainModels.Listen>> GetUserFeedAsync(string userUid, int count, string accessToken)
        {
            var user = await UserFacade.Auth0.GetUserAsync(userUid, accessToken);

            var feed = await DbContext.Listens.Where(x => x.UserUID == userUid).OrderByDescending(c => c.Date)
                .GroupBy(c => c.EpisodeUrl)
                .Select(e => e.FirstOrDefault()).Take(count).ToListAsync();

            return feed.Select(x => new DomainModels.Listen
            {
                ID = x.ID,
                UserName = user.name,
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
        public async Task<IEnumerable<DomainModels.User>> GetFollowingsAsync(string userUid, string accessToken)
        {
            var users = new List<UserFacade.User>();
            await DbContext.Follows.Where(x => x.UserUID == userUid).ForEachAsync(async x =>
            {
                users.Add(await UserFacade.Auth0.GetUserAsync(x.FollowUID, accessToken));
            });         

            return users.Select(c => new DomainModels.User
            {
                UserUID = c.user_id,
                UserTag = c.nickname,
                FirstName = c.family_name,
                LastName = c.given_name
            });
        }

        /// <summary>
        /// Returns all followers of the specified user tag.
        /// </summary>
        /// <param name="userId">ID of the user.</param>
        /// <returns></returns>
        public async Task<IEnumerable<DomainModels.User>> GetFollowersAsync(string userUid, string accessToken)
        {
            var users = new List<UserFacade.User>();
            await DbContext.Follows.Where(x => x.FollowUID == userUid).ForEachAsync(async x =>
            {
                users.Add(await UserFacade.Auth0.GetUserAsync(x.UserUID, accessToken));
            });

            return users.Select(c => new DomainModels.User
            {
                UserUID = c.user_id,
                UserTag = c.nickname,
                FirstName = c.family_name,
                LastName = c.given_name
            });
        }

        /// <summary>
        /// Adds the specified user to the users' following list.
        /// </summary>
        /// <param name="userId">User to add following to.</param>
        /// <param name="friendId">User to add.</param>
        /// <returns></returns>
        public async Task AddFollowingAsync(string userUid, string followingUid)
        {
            await DbContext.Follows.AddAsync(new Follow
            {
                UserUID = userUid,
                FollowUID = followingUid
            });

            await DbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Removes the specified user from the users' following list.
        /// </summary>
        /// <param name="userId">User to remove following from.</param>
        /// <param name="friendId">User to remove.</param>
        /// <returns></returns>
        public async Task RemoveFollowingAsync(string userUid, string followingUid)
        {
            var entry = await DbContext.Follows.FirstOrDefaultAsync(x => x.UserUID == userUid && x.FollowUID == followingUid);
            if (entry != null)
                DbContext.Follows.Remove(entry);

            await DbContext.SaveChangesAsync();
        }

        #endregion
    }
}
