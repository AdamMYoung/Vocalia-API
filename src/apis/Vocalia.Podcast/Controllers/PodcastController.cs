using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vocalia.Facades.GPodder;

namespace Vocalia.Podcast.Controllers
{
    [Route("api")]
    [ApiController]
    public class PodcastController : ControllerBase
    {
        private IGPodderFacade GPodder { get; set; }

        private readonly int defaultPodcastCount = 20;
        private readonly int defaultCategoryCount = 20;

        public PodcastController(IGPodderFacade gpodderFacade)
        {
            GPodder = gpodderFacade;
        }

        /// <summary>
        /// Returns podcast categories sorted by popularity. Can be optionally limited.
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        [Route("categories")]
        [HttpGet]
        public async Task<IActionResult> GetCategories(int? limit)
        {
            var count = limit ?? defaultCategoryCount;

            var gPodderCategories = await GPodder.GetCategoriesAsync(count);

            return Ok(gPodderCategories);
        }

        [Route("top")]
        [HttpGet]
        public async Task<IActionResult> GetPodcasts(int? limit, string category = null)
        {
            var count = limit ?? defaultPodcastCount;

            var gPodderPodcasts = category != null ?
                await GPodder.GetPodcastsByCategoryAsync(category, count) : 
                await GPodder.GetTopPodcastsAsync(count);

            return Ok(gPodderPodcasts);
        }
    }
}