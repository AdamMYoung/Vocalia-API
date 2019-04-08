using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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