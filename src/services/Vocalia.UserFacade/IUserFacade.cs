using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vocalia.UserFacade
{
    public interface IUserFacade
    {
        /// <summary>
        /// Queries the Auth0 servers for user info.
        /// </summary>
        /// <param name="userUID">ID to get.</param>
        /// <param name="accessToken">Authentication access token for the Vocalia endpoint.</param>
        /// <returns></returns>
        Task<User> GetUserInfoAsync(string userUID);

        /// <summary>
        /// Queries the Auth0 servers for the user search term.
        /// </summary>
        /// <param name="term">Term to search for.</param>
        /// <param name="accessToken">Authentication access token for the Vocalia endpoint.</param>
        /// <returns></returns>
        Task<IEnumerable<User>> SearchUsersAsync(string term);
    }
}

