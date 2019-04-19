using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
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

            var podcast = await Repository.GetPodcastDetailAsync(podcastUid, userId);
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

            var streams = await Repository.DeleteSessionAsync(sessionUid, userId);

            if (streams)
                return Ok();
            else
                return NotFound();
        }

        #endregion

        /// <summary>
        /// Gets timeline of the session.
        /// </summary>
        /// <param name="sessionUid">UID of the session.</param>
        /// <returns></returns>
        [Route("timeline")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetTimeline(Guid sessionUid)
        {
            string userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var clips = await Repository.GetTimelineAsync(sessionUid, userId);

            if (clips == null)
                return Unauthorized();

            var clipDTOs = clips.Select(x => new Clip
            {
                UID = x.UID,
                Date = x.Date,
                SessionUID = x.SessionUID,
                Name = x.Name,
                Media = x.Media.Select(c => new DTOs.Media
                {
                    Date = c.Date,
                    UID = c.UID,
                    MediaUrl = c.MediaUrl,
                    UserUID = c.UserUID,
                    UserImageUrl = c.UserImageUrl,
                    UserName = c.UserName,

                }),
                Edit = x.Edit != null ? new Edit
                {
                    StartTrim = x.Edit.StartTrim,
                    EndTrim = x.Edit.EndTrim,
                    ClipUID = x.UID
                } : null
            });

            return Ok(clipDTOs);
        }

        /// <summary>
        /// Sets the timeline for the specified session.
        /// </summary>
        /// <param name="sessionUid">Session UID of the timeline.</param>
        /// <param name="clips">Clips to apply to the timeline.</param>
        /// <returns></returns>
        [Route("timeline")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SetTimeline(Guid sessionUid, [FromBody] IEnumerable<DTOs.Clip> clips)
        {
            string userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var clipsDM = clips.Select(c => new DomainModels.Clip
            {
                SessionUID = sessionUid,
                Date = c.Date,
                Name = c.Name,
                UID = c.UID
            });

            var result = await Repository.SetTimelineAsync(sessionUid, userId, clipsDM);

            if (!result)
                return Unauthorized();
            else
                return Ok();
        }

        /// <summary>
        /// Gets all clips not in a timeline for the session.
        /// </summary>
        /// <param name="sessionUid">UID of the session.</param>
        /// <returns></returns>
        [Route("clips")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetClips(Guid sessionUid)
        {
            string userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var clips = await Repository.GetUnassignedClipsAsync(sessionUid, userId);

            if (clips == null)
                return Unauthorized();

            var clipDTOs = clips.Select(x => new Clip
            {
                UID = x.UID,
                Date = x.Date,
                SessionUID = x.SessionUID,
                Name = x.Name,
                Media = x.Media.Select(c => new DTOs.Media
                {
                    Date = c.Date,
                    UID = c.UID,
                    MediaUrl = c.MediaUrl,
                    UserUID = c.UserUID,
                    UserImageUrl = c.UserImageUrl,
                    UserName = c.UserName,

                }),
                Edit = x.Edit != null ? new Edit
                {
                    StartTrim = x.Edit.StartTrim,
                    EndTrim = x.Edit.EndTrim,
                    ClipUID = x.UID
                } : null
            });

            return Ok(clipDTOs);
        }

        /// <summary>
        /// Applies an edit to the clip.
        /// </summary>
        /// <param name="edit">Edit to apply.</param>
        /// <returns></returns>
        [Route("edit")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SetEdit([FromBody] DTOs.Edit edit)
        {
            string userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var editDM = new DomainModels.Edit
            {
                ClipUID = edit.ClipUID,
                StartTrim = edit.StartTrim,
                EndTrim = edit.EndTrim
            };

            var result = await Repository.AddEditAsync(userId, editDM);

            if (!result)
                return Unauthorized();
            else
                return Ok();
        }

        /// <summary>
        /// Submits the edit to the publishing microservice.
        /// </summary>
        /// <param name="sessionUid">Session to submit.</param>
        /// <returns></returns>
        [Route("submit")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Submit(Guid sessionUid)
        {
            string userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await Repository.FinishEditingAsync(sessionUid, userId);

            if (!result)
                return Unauthorized();
            else
                return Ok();
        }
    }
}