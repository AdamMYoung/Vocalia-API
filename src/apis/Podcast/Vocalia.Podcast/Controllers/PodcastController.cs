using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public IPodcastRepository Repository { get; }

        public PodcastController(IPodcastRepository repository) => Repository = repository;

        /// <summary>
        /// Returns all podcast categories.
        /// </summary>
        /// <returns></returns>
        [Route("categories")]
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await Repository.GetCategoriesAsync();

            return Ok(categories);
        }

        /// <summary>
        /// Gets the top podcasts from all supported services.
        /// </summary>
        /// <param name="limit">Number of entries to return.</param>
        /// <param name="category">Category to filter by.</param>
        /// <returns></returns>
        [Route("top")]
        [HttpGet]
        public async Task<IActionResult> GetTopPodcasts(int? limit)
        {
            var podcasts = await Repository.GetTopPodcastsAsync(limit);

            return Ok(podcasts);
        }
    }
}