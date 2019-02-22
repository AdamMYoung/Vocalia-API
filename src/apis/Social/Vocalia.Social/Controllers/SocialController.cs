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
        /// Gets the feed of the provided userUID, or the timeline for the current user if none is provided.
        /// </summary>
        /// <param name="userUID"></param>
        /// <returns></returns>
        [Route("feed")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetFeed(string userUID = null, int count = 25)
        {
            string userId = null;
            IEnumerable<DomainModels.Listen> feed = null;
            if (userUID == null)
            {
                userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                    return BadRequest();
            }

            if (userUID != null)
                feed = await Repository.GetUserFeedAsync(userUID, count);
            else
                feed = await Repository.GetTimelineFeedAsync(userId, count);

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
    }
}