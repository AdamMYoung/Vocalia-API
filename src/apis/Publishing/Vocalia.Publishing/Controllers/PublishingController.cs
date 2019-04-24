using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Vocalia.Ingest.Db;
using Vocalia.Publishing.Repository;

namespace Vocalia.Publishing.Controllers
{
    [Route("api")]
    [ApiController]
    public class PublishingController : ControllerBase
    {
        /// <summary>
        /// Data repository for publisher access.
        /// </summary>
        private IPublishingRepository Repository { get; }

        public PublishingController(IPublishingRepository repository)
        {
            Repository = repository;
        }

        /// <summary>
        /// Gets all unassigned podcasts from the database.
        /// </summary>
        /// <returns></returns>
        [Route("podcasts/unassigned")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUnassignedPodcasts()
        {
            string userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var podcasts = await Repository.GetAllUnassignedPodcasts(userId);

            if (podcasts == null)
                return NotFound();

            var podcastDTOs = podcasts.Select(c => new DTOs.UnassignedPodcast
            {
                ImageUrl = c.ImageUrl,
                Name = c.ImageUrl,
                UID = c.UID
            });

            return Ok(podcastDTOs);
        }

        /// <summary>
        /// Gets all assigned podcasts from the database.
        /// </summary>
        /// <returns></returns>
        [Route("podcasts/assigned")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAssignedPodcasts()
        {
            string userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var podcasts = await Repository.GetAllPodcasts(userId);

            if (podcasts == null)
                return NotFound();

            var podcastDTOs = podcasts.Select(c => new DTOs.Podcast
            {
                UID = c.UID,
                CategoryID = c.CategoryID,
                LanguageID = c.LanguageID,
                Title = c.Title,
                Description = c.Description,
                ImageUrl = c.ImageUrl,
                IsExplicit = c.IsExplicit,
                IsActive = c.IsActive,
                Episodes = c.Episodes.Select(x => new DTOs.Episode
                {
                    UID = x.UID,
                    Title = x.Title,
                    Description = x.Description,
                    RssUrl = x.RssUrl,
                    MediaUrl = x.MediaUrl,
                    PublishDate = x.PublishDate
                })
            });

            return Ok(podcastDTOs);
        }

        /// <summary>
        /// Gets all unassigned episodes from the database.
        /// </summary>
        /// <returns></returns>
        [Route("episodes")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUnassignedEpisodes()
        {
            string userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var episodes = await Repository.GetAllUnassignedEpisodes(userId);

            if (episodes == null)
                return NotFound();

            var episodeDTOs = episodes.Select(c => new DTOs.UnassignedEpisode
            {
                UID = c.UID,
                PodcastUID = c.UnassignedPodcastUID,
                Name = c.Name
            });

            return Ok(episodeDTOs);
        }

        /// <summary>
        /// Updates the specified podcast in the database.
        /// </summary>
        /// <param name="podcast">Podcast to update.</param>
        /// <returns></returns>
        [Route("podcast")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdatePodcast(DTOs.Podcast podcast)
        {
            string userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var dmPodcast = new DomainModels.Podcast
            {
                UID = podcast.UID,
                CategoryID = podcast.CategoryID,
                LanguageID = podcast.LanguageID,
                Title = podcast.Title,
                Description = podcast.Description,
                ImageUrl = podcast.ImageUrl,
                IsExplicit = podcast.IsExplicit,
                IsActive = podcast.IsActive
            };

            var isUpdated = await Repository.UpdatePodcast(userId, dmPodcast);

            if (isUpdated)
                return Ok();
            else
                return Unauthorized();
        }

        /// <summary>
        /// Deletes the specified podcast from the database.
        /// </summary>
        /// <param name="podcastUid">UID of the podcast to delete.</param>
        /// <returns></returns>
        [Route("podcast")]
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeletePodcast(Guid podcastUid)
        {
            string userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var isDeleted = await Repository.DeletePodcast(userId, podcastUid);

            if (isDeleted)
                return Ok();
            else
                return Unauthorized();
        }



        /// <summary>
        /// Updates the specified episode in the database.
        /// </summary>
        /// <param name="episode">Episode to update.</param>
        /// <returns></returns>
        [Route("episode")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateEpisode(DTOs.Episode episode)
        {
            string userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var dmEpisode = new DomainModels.Episode
            {
                UID = episode.UID,
                Title = episode.Title,
                Description = episode.Description,
                RssUrl = episode.RssUrl,
                MediaUrl = episode.MediaUrl,
                PublishDate = episode.PublishDate
            };

            var isUpdated = await Repository.UpdateEpisode(userId, dmEpisode);

            if (isUpdated)
                return Ok();
            else
                return Unauthorized();
        }

        /// <summary>
        /// Deletes the specified podcast from the database.
        /// </summary>
        /// <param name="podcastUid">UID of the podcast to delete.</param>
        /// <returns></returns>
        [Route("podcast")]
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteEpisode(Guid episodeUid)
        {
            string userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var isDeleted = await Repository.DeleteEpisode(userId, episodeUid);

            if (isDeleted)
                return Ok();
            else
                return Unauthorized();
        }

        /// <summary>
        /// Gets all categories that can be assigned.
        /// </summary>
        /// <returns></returns>
        [Route("categories")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await Repository.GetCategories();

            if (categories == null)
                return NotFound();

            var categoryDTOs = categories.Select(c => new DTOs.Category
            {
                ID = c.ID,
                ITunesID = c.ITunesID,
                GPodderTag = c.GPodderTag,
                Title = c.Title
            });

            return Ok(categoryDTOs);
        }

        /// <summary>
        /// Gets all lanugages that can be assigned.
        /// </summary>
        /// <returns></returns>
        [Route("languages")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetLanguages()
        {
            var languages = await Repository.GetLanguages();

            if (languages == null)
                return NotFound();

            var categoryDTOs = languages.Select(c => new DTOs.Language
            {
                ID = c.ID,
                ISOCode = c.ISOCode,
                Name = c.Name
            });

            return Ok(categoryDTOs);
        }
    }
}
