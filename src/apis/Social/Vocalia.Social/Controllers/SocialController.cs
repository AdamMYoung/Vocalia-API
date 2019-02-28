using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vocalia.Social.Repositories;

namespace Vocalia.Social.Controllers
{
    [Route("api")]
    [ApiController]
    public class SocialController : ControllerBase
    {
        /// <summary>
        /// Repository for social data.
        /// </summary>
        private ISocialRepository Repository { get; }

        /// <summary>
        /// Instantiates a new SocialController.
        /// </summary>
        /// <param name="repository">Repository object.</param>
        public SocialController(ISocialRepository repository)
        {
            Repository = repository;
        }

        /// <summary>
        /// Gets the timeline for the current authenticated user.
        /// </summary>
        /// <param name="count">Number of entries to return.</param>
        /// <returns></returns>
        [Route("feed")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserFeed(int count = 25)
        {
            string userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var feed = await Repository.GetTimelineFeedAsync(userId, count);

            if (feed == null)
                return NotFound();

            var entries = feed.Select(x => new DTOs.Listen
            {
                UserUID = x.UserUID,
                UserName = x.UserName,
                RssUrl = x.RssUrl,
                EpisodeUrl = x.EpisodeUrl,
                EpisodeName = x.EpisodeName,
                Date = x.Date,
                IsCompleted = x.IsCompleted
            });

            return Ok(entries);
        }

        /// <summary>
        /// Gets the user with the specified user tag.
        /// </summary>
        /// <param name="userTag">User to fetch.</param>
        /// <param name="count">Number of entries to return.</param>
        /// <returns></returns>
        [Route("user")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetUser(string userTag, int count = 25)
        {
            var user = await Repository.GetUserAsync(userTag);
            if (user == null)
                return NotFound();

            var followers = await Repository.GetFollowersAsync(userTag);
            var following = await Repository.GetFollowingsAsync(userTag);
            var feed = await Repository.GetUserFeedAsync(userTag, 25);

            var userDto = new DTOs.User
            {
                UserTag = user.UserTag,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PictureUrl = user.PictureUrl,
                Followers = followers.Select(x => new DTOs.User
                {
                    UserTag = x.UserTag,
                    FirstName = x.FirstName,
                    LastName = x.LastName
                }),
                Following = following.Select(x => new DTOs.User
                {
                    UserTag = x.UserTag,
                    FirstName = x.FirstName,
                    LastName = x.LastName
                }),
                Feed = feed.Select(x => new DTOs.Listen {
                    UserUID = x.UserUID,
                    UserName = x.UserName,
                    RssUrl = x.RssUrl,
                    EpisodeUrl = x.EpisodeUrl,
                    EpisodeName = x.EpisodeName,
                    Date = x.Date,
                    IsCompleted = x.IsCompleted
                })
            };

            return Ok(user);
        }

        /// <summary>
        /// Adds the specified user tag to the authorized user's follow list.
        /// </summary>
        /// <param name="userTag">User to add.</param>
        /// <returns></returns>
        [Route("followers")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddFollower(string userTag)
        {
            string userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await Repository.AddFollowingAsync(userId, userTag);

            return Ok();
        }

        /// <summary>
        /// Removes the specified user from the authorized user's follow list.
        /// </summary>
        /// <param name="userTag">User to remove.</param>
        /// <returns></returns>
        [Route("followers")]
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> RemoveFollower(string userTag)
        {
            string userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await Repository.RemoveFollowingAsync(userId, userTag);

            return Ok();
        }
    }
}