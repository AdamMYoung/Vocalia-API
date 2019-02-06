using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public PodcastController(IPodcastRepository repository)
        {
            Repository = repository;

        }

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
        /// <param name="limit">Number of entries to return.</param>
        /// <param name="category">Category to filter by.</param>
        /// <returns></returns>
        [Route("top")]
        [HttpGet]
        public async Task<IActionResult> GetTopPodcasts(int? categoryId, bool allowExplicit = true, int limit = 100)
        {
            var podcasts = await Repository.GetTopPodcastsAsync(limit, categoryId, allowExplicit);

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
        /// <param name="term">Query to search for.</param>
        /// <param name="limit">Number of items to return.</param>
        /// <returns></returns>
        [Route("search")]
        [HttpGet]
        public async Task<IActionResult> SearchPodcasts(string term, int limit = 10, bool allowExplicit = true)
        {
            if (term == null || term.Length == 0)
                return BadRequest();

            var query = await Repository.SearchPodcastsAsync(term, limit, allowExplicit);

            if (query == null)
                return BadRequest();

            return Ok(query);
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

            return Ok(parsedFeed);
        }
    }
}