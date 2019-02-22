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
        public async Task<DomainModels.User> GetUserAsync(string userId)
        {
            var user = await DbContext.Users.FirstOrDefaultAsync(u => u.UserUID == userId && u.Active);

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
        public async Task<IEnumerable<DomainModels.Listen>> GetUserFeedAsync(string userId, int count)
        {
            var user = await DbContext.Users.FirstOrDefaultAsync(u => u.UserUID == userId && u.Active);

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
    }
}
