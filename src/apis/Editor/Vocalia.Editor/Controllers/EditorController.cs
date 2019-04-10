using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
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

        /// <summary>
        /// Gets all streams belonging to the session UID.
        /// </summary>
        /// <param name="sessionUid">UID of the session.</param>
        /// <returns></returns>
        [Route("streams")]
        [HttpGet]
        public async Task<IActionResult> GetStream(Guid sessionUid)
        {
            string userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var streams = await Repository.GetStreamsAsync(sessionUid, userId);

            if (streams != null)
                return Unauthorized();
            else
                return Ok(streams);
        }
    }
}