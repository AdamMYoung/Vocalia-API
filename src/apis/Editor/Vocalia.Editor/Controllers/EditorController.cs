using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Vocalia.Editor.DTOs;
using Vocalia.Editor.Repository;

namespace Vocalia.Editor.Controllers
{
    [Route("api")]
    [ApiController]
    public class EditorController : ControllerBase
    {
        /// <summary>
        /// Repository for editor interaction.
        /// </summary>
        private IEditorRepository Repository { get; }

        /// <summary>
        /// Instantiates a new EditorController.
        /// </summary>
        /// <param name="editorRepo"></param>
        public EditorController(IEditorRepository editorRepo)
        {
            Repository = editorRepo;
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
                return NotFound();

            var podcastDTOs = podcasts.Select(x => new Podcast
            {
                UID = x.UID,
                Name = x.Name,
                ImageUrl = x.ImageUrl,
                Sessions = x.Sessions.Select(c => new Session
                {
                    UID = c.UID,
                    Date = c.Date
                })
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
                return null;

            var podcastDTO = new Podcast
            {
                UID = podcast.UID,
                Name = podcast.Name,
                ImageUrl = podcast.ImageUrl,
                Sessions = podcast.Sessions.Select(s => new Session
                {
                    UID = s.UID,
                    Date = s.Date
                })
            };

            return Ok(podcastDTO);
        }

        #endregion

        #region Session

        /// <summary>
        /// Deletes the specified session from the database.
        /// </summary>
        /// <param name="sessionUid">UID of the session.</param>
        /// <returns></returns>
        [Route("session")]
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteSession(Guid sessionUid)
        {
            string userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var streams = await Repository.DeleteSessionAsync(userId, sessionUid);

            if (streams)
                return Ok();
            else
                return NotFound();
        }

        #endregion

        /// <summary>
        /// Gets all streams belonging to the session UID.
        /// </summary>
        /// <param name="sessionUid">UID of the session.</param>
        /// <returns></returns>
        [Route("streams")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetStreams(Guid sessionUid)
        {
            string userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var streams = await Repository.GetStreamsAsync(sessionUid, userId);

            if (streams == null)
                return Unauthorized();

            var streamsDTOs = streams.Select(x => new UserTrack
            {
                UserUid = x.UserUid,
                SessionUid = x.SessionUid,
                UserName = x.UserName,
                Entries = x.Entries.Select(c => new AudioEntry
                {
                    UID = c.UID,
                    Url = c.Url
                })
            });

            return Ok(streamsDTOs);
        }
    }
}