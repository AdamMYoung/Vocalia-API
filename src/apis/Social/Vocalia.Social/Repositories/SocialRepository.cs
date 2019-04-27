using System.Threading.Tasks;
using Vocalia.UserFacade;

namespace Vocalia.Social.Repositories
{
    internal class SocialRepository : ISocialRepository
    {
        /// <summary>
        /// API for Auth0 user access.
        /// </summary>
        private IUserFacade UserAPI { get; }

        /// <summary>
        /// Instantiates a new SocialRepository.
        /// </summary>
        /// <param name="context"></param>
        public SocialRepository(IUserFacade userFacade)
        {
            UserAPI = userFacade;
        }

        /// <summary>
        /// Returns user information for a specific user.
        /// </summary>
        /// <param name="userId">ID of the user.</param>
        /// <param name="accessToken">Auth token for user access.</param>
        /// <returns></returns>
        public async Task<DomainModels.User> GetUserDetailAsync(string userId)
        {
            var user = await UserAPI.GetUserInfoAsync(userId);

            return new DomainModels.User
            {
                FirstName = user.given_name,
                LastName = user.family_name,
                UserUID = user.user_id,
                UserTag = user.nickname,
                PictureUrl = user.picture
            };
        }
    }
}
