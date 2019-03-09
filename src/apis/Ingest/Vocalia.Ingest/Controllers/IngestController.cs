using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vocalia.Ingest.Repositories;

namespace Ingest_API.Controllers
{
    [Route("api")]
    public class IngestController : ControllerBase
    {
        /// <summary>
        /// Database repository.
        /// </summary>
        public IIngestRepository Repository { get; }

        /// <summary>
        /// Instantiates a new IngestController.
        /// </summary>
        /// <param name="repository">Database Repository.</param>
        public IngestController(IIngestRepository repository)
        {
            Repository = repository;
        }

        #region Podcast

        /// <summary>
        /// Gets all podcasts belonging to the user.
        /// </summary>
        /// <returns></returns>
        [Route("podcast")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPodcasts()
        {
            string userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var podcasts = await Repository.GetPodcastsAsync(userId);
            if (podcasts == null)
                return NotFound();

            var podcastDTOs = podcasts.Select(x => new Vocalia.Ingest.DTOs.Podcast
            {
                UID = x.UID,
                Name = x.Name,
                Description = x.Description,
                ImageUrl = x.ImageUrl
            });

            return Ok(podcastDTOs);
        }

        /// <summary>
        /// Creates a podcast, and assigns the user as the admin.
        /// </summary>
        /// <param name="podcast">Podcast to insert.</param>
        /// <returns></returns>
        [Route("podcast")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePodcast([FromBody] Vocalia.Ingest.DTOs.PodcastUpload podcast)
        {
            string userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
            var domainModel = new Vocalia.Ingest.DomainModels.PodcastUpload
            {
                Name = podcast.Name,
                UID = podcast.UID,
                Description = podcast.Description,
                ImageData = podcast.ImageData,
                FileType = podcast.FileType
            };

            await Repository.CreatePodcastAsync(userId, domainModel);
            return Ok();
        }

        /// <summary>
        /// Updates the podcast entry provided.
        /// </summary>
        /// <param name="podcast">Podcast to update.</param>
        /// <returns></returns>
        [Route("podcast")]
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> EditPodcast([FromBody] Vocalia.Ingest.DTOs.PodcastUpload podcast)
        {
            string userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var domainModel = new Vocalia.Ingest.DomainModels.PodcastUpload
            {
                Name = podcast.Name,
                UID = podcast.UID,
                Description = podcast.Description,
                ImageData = podcast.ImageData,
                FileType = podcast.FileType
            };

            await Repository.UpdatePodcastAsync(userId, domainModel);
            return Ok();
        }

        /// <summary>
        /// Deletes the specified podcast.
        /// </summary>
        /// <param name="podcastUid">Group UID to add to.</param>
        /// <returns></returns>
        [Route("podcast")]
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeletePodcast(Guid podcastUid)
        {
            string userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            await Repository.DeletePodcastAsync(podcastUid, userId);
            return Ok();
        }

        #endregion

        #region Session

        /// <summary>
        /// Gets all sessions belonging to the specified podcast UID.
        /// </summary>
        /// <param name="podcastUid">Podcast UID to get sessions for.</param>
        /// <returns></returns>
        [Route("session")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetSessions(Guid podcastUid)
        {
            string userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var sessions = await Repository.GetSessionsAsync(podcastUid, userId);
            if (sessions == null)
                return NotFound();

            var sessionDTOs = sessions.Select(x => new Vocalia.Ingest.DTOs.Session
            {
                UID = x.UID,
                Date = x.Date
            });

            return Ok(sessionDTOs);
        }

        /// <summary>
        /// Creates a new session for the specified podcast Uid.
        /// </summary>
        /// <param name="podcastUid">Podcast UID.</param>
        /// <returns></returns>
        [Route("session")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateSession(Guid podcastUid)
        {
            string userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var sessionGuid = await Repository.CreateSessionAsync(podcastUid, userId);
            if (sessionGuid == null)
                return NotFound();

            return Ok();
        }

        /// <summary>
        /// Creates a new session for the specified podcast Uid.
        /// </summary>
        /// <param name="podcastUid">Podcast UID.</param>
        /// <returns></returns>
        [Route("session")]
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteSession(Guid sessionUid)
        {
            string userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            await Repository.DeleteSessionAsync(sessionUid, userId);

            return Ok();
        }

        #endregion

        #region Invite

        /// <summary>
        /// Creates an invite link for the specififed group ID.
        /// </summary>
        /// <param name="podcastUid">GUID to add.</param>
        /// <returns></returns>
        [Route("invite")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetInviteLink(Guid podcastUid, DateTime? expiry = null)
        {
            string userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var link = await Repository.CreateInviteLinkAsync(podcastUid, userId, expiry);
            if (link == null)
                return Unauthorized();

            else
                return Ok(link);
        }

        /// <summary>
        /// Accepts the specified invite link.
        /// </summary>
        /// <param name="inviteLink">GUID to accept.</param>
        /// <returns></returns>
        [Route("invite")]
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> AcceptInviteLink(Guid inviteLink)
        {
            string userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var added = await Repository.AcceptInviteLinkAsync(inviteLink, userId);

            if (added)
                return Ok();
            else
                return Unauthorized();

        }

        #endregion
    }
}
