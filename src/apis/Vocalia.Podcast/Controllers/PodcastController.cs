using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vocalia.Facades.GPodder;
using Vocalia.Podcast.DomainModels;
using Vocalia.Podcast.Facades.iTunes;

namespace Vocalia.Podcast.Controllers
{
    [Route("api")]
    [ApiController]
    public class PodcastController : ControllerBase
    {
        private IGPodderFacade GPodderService { get; set; }
        private IITunesFacade ITunesService { get; set; }

        private readonly int defaultPodcastCount = 20;

        public PodcastController(IGPodderFacade gpodderFacade, IITunesFacade iTunesFacade)
        {
            GPodderService = gpodderFacade;
            ITunesService = iTunesFacade;
        }

        /// <summary>
        /// Returns all podcast categories.
        /// </summary>
        /// <returns></returns>
        [Route("categories")]
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {


            return Ok();
        }

        /// <summary>
        /// Gets the top podcasts from all supported services.
        /// </summary>
        /// <param name="limit">Number of entries to return.</param>
        /// <param name="category">Category to filter by.</param>
        /// <returns></returns>
        [Route("top")]
        [HttpGet]
        public async Task<IActionResult> GetTopPodcasts(int? limit, DTOs.Category category = null)
        {
            var count = limit ?? defaultPodcastCount;

            //var gPodderPodcasts = await GPodder.GetTopPodcastsAsync(count, category);
            var iTunesPodcasts = await ITunesService.GetTopPodcastsAsync(count);

            return Ok(iTunesPodcasts);
        }
    }
}