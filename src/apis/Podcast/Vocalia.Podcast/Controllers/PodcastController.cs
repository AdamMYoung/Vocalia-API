using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Vocalia.Facades.GPodder;
using Vocalia.Podcast.DomainModels;
using Vocalia.Podcast.Facades.iTunes;
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
            if (rssUrl.Length == 0 || rssUrl == null)
                return BadRequest();

            var parsedFeed = await Repository.GetFeedFromUrl(rssUrl);

            if (parsedFeed == null)
                return NotFound();

            var feed = new DTOs.Feed()
            {
                Title = parsedFeed.Title,
                Link = parsedFeed.Link,
                Description = parsedFeed.Description,
                Copyright = parsedFeed.Copyright,
                ImageUrl = parsedFeed.ImageUrl,
                Items = parsedFeed.Items.Select(i => new DTOs.FeedItem
                {
                    Title = i.Title,
                    RssUrl = rssUrl,
                    ImageUrl = i.ImageUrl,
                    Description = i.Description,
                    PublishingDate = i.PublishingDate,
                    Author = i.Title,
                    Id = i.Id,
                    Content = i.Content
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
        public async Task<IActionResult> GetSubscriptions()
        {
            var subs = await Repository.GetSubscriptions("userUID");
            if (subs == null)
                return NotFound();

            var subDTOs = subs.Select(s => new DTOs.Subscription
            {
                Name = s.Name,
                Description = s.Description,
                GUID = s.GUID,
                ImageUrl = s.ImageUrl,
                RssUrl = s.RssUrl
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
        public async Task<IActionResult> AddSubscription(DTOs.Subscription subscription)
        {
            var sub = new DomainModels.Subscription()
            {
                Name = subscription.Name,
                Description = subscription.Description,
                ImageUrl = subscription.ImageUrl,
                RssUrl = subscription.RssUrl,
                UserUID = "userUID"
            };
            await Repository.AddSubscription(sub);
            return Ok();
        }

        /// <summary>
        /// Removes the specified subscription from the authenticated user.
        /// </summary>
        /// <param name="guid">GUID of the subscription to renew.</param>
        /// <returns></returns>
        [Route("subscriptions")]
        [HttpDelete]
        public async Task<IActionResult> RemoveSubscription(string guid)
        {
            await Repository.DeleteSubscription(guid);
            return Ok();
        }

        #endregion
    }
}