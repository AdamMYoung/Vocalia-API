using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vocalia.Social.Repositories;

namespace Vocalia.Social.Controllers
{
    [Route("api")]
    [ApiController]
    public class SocialController : ControllerBase
    {
        /// <summary>
        /// Repository for social data.
        /// </summary>
        private ISocialRepository Repository { get; }

        /// <summary>
        /// Instantiates a new SocialController.
        /// </summary>
        /// <param name="repository">Repository object.</param>
        public SocialController(ISocialRepository repository)
        {
            Repository = repository;
        }

        /// <summary>
        /// Gets detailed information for the user with the specified tag.
        /// </summary>
        /// <param name="userId">User to fetch.</param>
        /// <returns></returns>
        [Route("user/detail")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserDetail(string userId)
        {
            var user = await Repository.GetUserDetailAsync(userId);
            if (user == null)
                return NotFound();       

            var userDto = new DTOs.User
            {
                UserUID = userId,
                UserTag = user.UserTag,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PictureUrl = user.PictureUrl,
            };

            return Ok(userDto);
        }

        /// <summary>
        /// Gets detailed information about the current user.
        /// </summary>
        /// <returns></returns>
        [Route("user")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserDetail()
        {
            string userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var user = await Repository.GetUserDetailAsync(userId);
            if (user == null)
                return NotFound();

            var userDto = new DTOs.User
            {
                UserUID = userId,
                UserTag = user.UserTag,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PictureUrl = user.PictureUrl,
            };

            return Ok(userDto);
        }
    }
}