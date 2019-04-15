using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Vocalia.Ingest.DTOs;
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
        /// Gets general information about all podcasts belonging to the user.
        /// </summary>
        /// <returns></returns>
        [Route("podcasts")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPodcasts()
        {
            string userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var podcasts = await Repository.GetPodcastsAsync(userId);
            if (podcasts == null)
            {
                return NotFound();
            }

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
        /// Gets detailed information about a specific podcast.
        /// </summary>
        /// <returns></returns>
        [Route("podcast")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPodcastDetail(Guid podcastUid)
        {
            string userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var podcast = await Repository.GetPodcastDetailAsync(userId, podcastUid);
            if (podcast == null)
            {
                podcast = await Repository.GetPodcastOverviewAsync(podcastUid);
                if (podcast == null)
                {
                    return null;
                }
            }

            var podcastDTO = new Podcast
            {
                UID = podcast.UID,
                Name = podcast.Name,
                Description = podcast.Description,
                ImageUrl = podcast.ImageUrl,
                Members = podcast.Members.Select(m => new User
                {
                    UID = m.UID,
                    IsAdmin = m.IsAdmin
                }),
                Sessions = podcast.Sessions.Select(s => new Session
                {
                    UID = s.UID,
                    Date = s.Date
                })
            };

            return Ok(podcastDTO);
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
        public async Task<IActionResult> EditPodcast([FromBody] PodcastUpload podcast)
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

            var isCompleted = await Repository.DeleteSessionAsync(sessionUid, userId);
            if (!isCompleted)
                return Unauthorized();

            return Ok();
        }

        /// <summary>
        /// Finishes the session's recording, and passes the info onto editing.
        /// </summary>
        /// <param name="sessionUid">Session UID to finish.</param>
        /// <returns></returns>
        [Route("session/complete")]
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> CompleteSession(Guid sessionUid)
        {
            string userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var isCompleted = await Repository.CompleteSessionAsync(sessionUid, userId);
            if (!isCompleted)
                return Unauthorized();
  
            return Ok();
        }

        /// <summary>
        /// Gets all clips belonging to the specified sessionUID.
        /// </summary>
        /// <param name="sessionUid">Session to get clips for.</param>
        /// <returns></returns>
        [Route("clip")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetClips(Guid sessionUid)
        {
            string userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var clips = await Repository.GetClipsAsync(sessionUid, userId);

            var clipDTOs = clips.Select(c => new SessionClip
            {
                MediaUrl = c.MediaUrl,
                Size = c.Size,
                UID = c.UID,
                UserUID = c.UserUID,
                Time = c.Time
            });

            if (clips == null)
                return NotFound();

            return Ok(clipDTOs);
        }

        /// <summary>
        /// Deletes the specified clip from the database.
        /// </summary>
        /// <param name="clipUid">Clip to delete.</param>
        /// <returns></returns>
        [Route("clip")]
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteClip(Guid clipUid)
        {
            string userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var isDeleted = await Repository.DeleteClipAsync(clipUid, userId);

            if (!isDeleted)
                return Unauthorized();

            return Ok();
        }

        /// <summary>
        /// Finishes the current clip.
        /// </summary>
        /// <param name="sessionUid">Session to finish the clip of.</param>
        /// <returns></returns>
        [Route("clip")]
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> FinishClip(Guid sessionUid)
        {
            string userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var isCompleted = await Repository.FinishClipAsync(sessionUid, userId);

            if (!isCompleted)
                return Unauthorized();

            return Ok();
        }

        #endregion

        #region Invite

        /// <summary>
        /// Returns the podcast info belonging to the provided invite.
        /// </summary>
        /// <param name="inviteLink"></param>
        /// <returns></returns>
        [Route("invite/info")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetInvitePodcastInfo(Guid inviteLink)
        {
            var podcast = await Repository.GetInviteInfoAsync(inviteLink);

            if (podcast == null)
            {
                return NotFound();
            }

            var podcastDto = new Vocalia.Ingest.DTOs.Podcast
            {
                UID = podcast.UID,
                Name = podcast.Name,
                Description = podcast.Description,
                ImageUrl = podcast.ImageUrl
            };

            return Ok(podcastDto);
        }

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
            {
                return Unauthorized();
            }
            else
            {
                return Ok(link);
            }
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
            {
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }

        #endregion

        #region Ingestion

        /// <summary>
        /// Uploads a media blob to the database.
        /// </summary>
        /// <param name="upload">Data to upload.</param>
        /// <returns></returns>
        [Route("record")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostBlob([FromForm] BlobUpload upload)
        {
            string userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            int.TryParse(upload.Timestamp, out int value);

            var uploadDM = new Vocalia.Ingest.DomainModels.BlobUpload
            {
                UserUID = userId,
                SessionUID = upload.SessionUID,
                Timestamp = value,
                Data = upload.Data
            };

            await Repository.PostMediaBlobAsync(uploadDM);
            return Ok();
        }
        #endregion
    }
}
