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
        public async Task<IActionResult> GetTopPodcasts(int? limit, int? categoryId, bool? allowExplicit)
        {
            var expl = allowExplicit.HasValue && allowExplicit.Value == false ? false : true;
            var podcasts = await Repository.GetTopPodcastsAsync(limit, categoryId, expl);

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
    }
}