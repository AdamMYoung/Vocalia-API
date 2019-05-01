using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vocalia.Podcast.Repositories;

namespace Vocalia.Podcast.Controllers
{
    [Route("api")]
    [ApiController]
    public class PodcastController : ControllerBase
    {
        /// <summary>
        /// Repository for API data.
        /// </summary>
        private IPodcastRepository Repository { get; }

        /// <summary>
        /// Instantiates a new PodcastController.
        /// </summary>
        /// <param name="repository">Podcast repository to query.</param>
        public PodcastController(IPodcastRepository repository) => Repository = repository;

        #region Search API

        /// <summary>
        /// Returns all podcast categories.
        /// </summary>
        /// <returns></returns>
        [Route("categories")]
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await Repository.GetCategoriesAsync();

            if (categories == null)
                return StatusCode(StatusCodes.Status500InternalServerError);

            var categoryDTOs = categories.Select(c => new DTOs.Category()
            {
                ID = c.ID,
                Title = c.Title,
                IconUrl = c.IconUrl
            }).ToList();

            return Ok(categoryDTOs);
        }

        /// <summary>
        /// Gets the top podcasts from all supported services.
        /// </summary>
        /// <param name="categoryId">Optional category to filter by..</param>
        /// <param name="allowExplicit">Filters out explicit content.</param>
        /// <param name="limit">Number of entries to return.</param>
        /// <param name="countryCode">Category to filter by.</param>
        /// <returns></returns>
        [Route("top")]
        [HttpGet]
        public async Task<IActionResult> GetTopPodcasts(int? categoryId, bool allowExplicit = true, int limit = 100, string countryCode = "gb")
        {
            var podcasts = await Repository.GetTopPodcastsAsync(limit, categoryId, allowExplicit, countryCode);

            if(podcasts == null)
                return StatusCode(StatusCodes.Status500InternalServerError);

            if (podcasts.Count() == 0)
                return NotFound();

            var podcastDTOs = podcasts.Select(p => new DTOs.Podcast()
            {
                ID = p.ID,
                Title = p.Title,
                ImageUrl = p.ImageUrl,
                RssUrl = p.RssUrl
            }).ToList();

            return Ok(podcastDTOs);
        }

        /// <summary>
        /// Queries all sources for the specified query.
        /// </summary>
        /// <param name="term">Query term to search by.</param>
        /// <param name="limit">Number of entries to return.</param>
        /// <param name="allowExplicit">Filters out explicit content.</param>
        /// <param name="countryCode">Category to filter by.</param>
        /// <returns></returns>
        [Route("search")]
        [HttpGet]
        public async Task<IActionResult> SearchPodcasts(string term, int limit = 10, bool allowExplicit = true, string countryCode = "gb")
        {
            if (term == null || term.Length == 0)
                return BadRequest();

            var query = await Repository.SearchPodcastsAsync(term, limit, allowExplicit, countryCode);

            if (query == null)
                return NotFound();

            var podcastDTOs = query.Select(p => new DTOs.Podcast()
            {
                ID = p.ID,
                Title = p.Title,
                ImageUrl = p.ImageUrl,
                RssUrl = p.RssUrl
            }).ToList();

            return Ok(podcastDTOs);
        }

        /// <summary>
        /// Parses an RSS feed into JSON objects, with current duration and completion status if applicable.
        /// </summary>
        /// <param name="rssUrl">URL to parse.</param>
        /// <returns></returns>
        [Route("parse")]
        [HttpGet]
        public async Task<IActionResult> ParseRssFeed(string rssUrl)
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (rssUrl.Length == 0 || rssUrl == null)
                return BadRequest();

            var parsedFeed = await Repository.GetFeedFromUrlAsync(rssUrl, userId);

            if (parsedFeed == null)
                return NotFound();

            var feed = new DTOs.Feed()
            {
                Title = parsedFeed.Title,
                Link = parsedFeed.Link,
                Description = parsedFeed.Description,
                Copyright = parsedFeed.Copyright,
                ImageUrl = parsedFeed.ImageUrl,
                IsSubscribed = parsedFeed.IsSubscribed,
                Items = parsedFeed.Items.Select(i => new DTOs.FeedItem
                {
                    Title = i.Title,
                    RssUrl = rssUrl,
                    ImageUrl = i.ImageUrl,
                    Description = i.Description,
                    PublishingDate = i.PublishingDate,
                    Author = i.Author,
                    Id = i.Id,
                    Content = i.Content,
                    Time = i.Time,
                    IsCompleted = i.IsCompleted,
                    Duration = i.Duration
                })
            };

            return Ok(feed);
        }

        #endregion

        #region Subscription API

        /// <summary>
        /// Returns all subscriptions belonging to the user.
        /// </summary>
        /// <returns></returns>
        [Route("subscriptions")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetSubscribedPodcasts()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var subs = await Repository.GetSubscriptionsAsync(userId);
            if (subs == null)
                return NotFound();

            var subDTOs = subs.Select(s => new DTOs.Podcast
            {
                Title = s.Name,
                ImageUrl = s.ImageUrl,
                RssUrl = s.RssUrl,
                ID = 0
               
            });

            return Ok(subDTOs);
        }

        /// <summary>
        /// Adds the specified subscription to the authenticated user.
        /// </summary>
        /// <param name="subscription">Subscription object to add.</param>
        /// <returns></returns>
        [Route("subscriptions")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddPodcastToSubscriptions([FromBody]DTOs.Podcast podast)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var sub = new DomainModels.Subscription()
            {
                Name = podast.Title,
                ImageUrl = podast.ImageUrl,
                RssUrl = podast.RssUrl,
                UserUID = userId
            };
            await Repository.AddSubscriptionAsync(sub);
            return Ok();
        }

        /// <summary>
        /// Removes the specified subscription from the authenticated user.
        /// </summary>
        /// <param name="guid">GUID of the subscription to renew.</param>
        /// <returns></returns>
        [Route("subscriptions")]
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> RemoveSubscription(string rssUrl)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            await Repository.DeleteSubscriptionAsync(rssUrl, userId);
            return Ok();
        }

        #endregion

        #region Listen API 

        /// <summary>
        /// Gets the listen info in the DB.
        /// </summary>
        /// <returns></returns>
        [Route("listen")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetListenInfo(string episodeUrl)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var info = await Repository.GetListenInfoAsync(episodeUrl, userId);
            if (info == null)
                return NotFound();

            var infoDTO = new DTOs.Listen
            {
                RssUrl = info.RssUrl,
                EpisodeUrl = info.EpisodeUrl,
                Time = info.Time,
                EpisodeName = info.EpisodeName,
                IsCompleted = info.IsCompleted,
                Duration = info.Duration
            };

            return Ok(infoDTO);
        }

        /// <summary>
        /// Updates the listen info in the DB.
        /// </summary>
        /// <returns></returns>
        [Route("listen")]
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> SetListenInfo([FromBody] DTOs.Listen listenInfo)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var listen = new DomainModels.Listen
            {
                RssUrl = listenInfo.RssUrl,
                EpisodeUrl = listenInfo.EpisodeUrl,
                EpisodeName = listenInfo.EpisodeName,
                UserUID = userId,
                IsCompleted = listenInfo.IsCompleted,
                Time = listenInfo.Time,
                Duration = listenInfo.Duration
            };

            await Repository.SetListenInfoAsync(listen);
            return Ok();
        }

        /// <summary>
        /// Gets the latest podcast listened to by the user.
        /// </summary>
        /// <returns></returns>
        [Route("latest")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetLatestPodcast()
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var listened = await Repository.GetLatestPodcastListenedAsync(userId);
            if (listened == null)
                return NotFound();

            var listenDTO = new DTOs.FeedItem
            {
                Title = listened.Title,
                Author = listened.Author,
                RssUrl = listened.RssUrl,
                Content = listened.Content,
                IsCompleted = listened.IsCompleted,
                Time = listened.Time,
                ImageUrl = listened.ImageUrl
            };

            return Ok(listenDTO);
        }

        #endregion
    }
}