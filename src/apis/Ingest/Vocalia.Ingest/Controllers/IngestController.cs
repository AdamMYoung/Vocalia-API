using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vocalia.Ingest.Repositories;

namespace Ingest_API.Controllers
{
    [Route("api")]
    [ApiController]
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

        /// <summary>
        /// Gets all groups belonging to the authenticated user.
        /// </summary>
        /// <returns></returns>
        [Route("group")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserGroups()
        {
            string userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var groups = await Repository.GetUserGroupsAsync(userId);
            if (groups == null)
                return NotFound();

            var groupDTOs = groups.Select(x => new Vocalia.Ingest.DTOs.Group
            {
                UID = x.UID,
                Description = x.Description,
                Name = x.Name
            });

            return Ok(groupDTOs);
        }

        /// <summary>
        /// Creates a group for the specified user using the provided group info.
        /// </summary>
        /// <param name="name">Name of the group to insert.</param>
        /// <param name="description">Description of the group to insert.</param>
        /// <returns></returns>
        [Route("group")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateUserGroup(string name, string description)
        {
            string userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            await Repository.CreateGroupAsync(userId, name, description);

            return Ok();
        }

        /// <summary>
        /// Gets all podcasts belonging to the specified group UID.
        /// </summary>
        /// <param name="groupUid">Group UID.</param>
        /// <returns></returns>
        [Route("podcast")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetGroupPodcasts(Guid groupUid)
        {
            string userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var podcasts = await Repository.GetGroupPodcastsAsync(groupUid, userId);
            if (podcasts == null)
                return NotFound();

            var podcastDTOs = podcasts.Select(x => new Vocalia.Ingest.DTOs.Podcast
            {
                UID = x.UID,
                Name = x.Name
            });

            return Ok(podcastDTOs);
        }

        /// <summary>
        /// Creates a podcast for the specified group using the provided podcast info.
        /// </summary>
        /// <param name="groupUid">Group UID to add to.</param>
        /// <param name="podcast">Podcast to insert.</param>
        /// <returns></returns>
        [Route("podcast")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateGroupPodcast(Guid groupUid, string name)
        {
            string userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            await Repository.CreateGroupPodcastAsync(userId, groupUid, name);

            return Ok();
        }

        /// <summary>
        /// Gets all sessions belonging to the specified podcast UID.
        /// </summary>
        /// <param name="podcastUid">Podcast UID.</param>
        /// <returns></returns>
        [Route("session")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPodcastSessions(Guid podcastUid)
        {
            string userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var sessions = await Repository.GetPodcastSessionsAsync(podcastUid, userId);
            if (sessions == null)
                return NotFound();

            var sessionDTOs = sessions.Select(x => new Vocalia.Ingest.DTOs.Session
            {
                UID = x.UID,
                Date = x.Date,
                InProgress = x.InProgress
            });

            return Ok(sessionDTOs);
        }

        /// <summary>
        /// Creates a new session for the specified podcast Uid.
        /// </summary>
        /// <param name="podcastUid">Podcast UID.</param>
        /// <returns></returns>
        [Route("session")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> CreatePodcastSession(Guid podcastUid)
        {
            string userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var sessionGuid = await Repository.CreateNewSessionAsync(podcastUid, userId);
            if (sessionGuid == null)
                return NotFound();

            return Ok(sessionGuid);
        }

        /// <summary>
        /// Creates an invite link for the specififed group ID.
        /// </summary>
        /// <param name="groupUid">GUID to add.</param>
        /// <returns></returns>
        [Route("invite")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetInviteLink(Guid groupUid, DateTime? expiry = null)
        {
            string userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var link = await Repository.CreateInviteLinkAsync(groupUid, userId, expiry);
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
    }
}
