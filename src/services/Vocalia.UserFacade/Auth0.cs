using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vocalia.UserFacade
{
    public class Auth0
    {
        /// <summary>
        /// Queries the Auth0 servers for user info.
        /// </summary>
        /// <param name="userUID">ID to get.</param>
        /// <param name="accessToken">Authentication access token for the Vocalia endpoint.</param>
        /// <returns></returns>
        public static async Task<User> GetUserInfoAsync(string userUID, string accessToken)
        {
            var client = new RestClient($"https://vocalia.eu.auth0.com/api/v2/users/{userUID}");
            var request = new RestRequest(Method.GET);
            request.AddHeader("authorization", $"Bearer {accessToken}");

            var response = await client.ExecuteTaskAsync<User>(request);
            return response.Data;
        }

        /// <summary>
        /// Queries the Auth0 servers for a set of user info.
        /// </summary>
        /// <param name="userUIDs">IDs to get.</param>
        /// <param name="accessToken">Authentication access token for the Vocalia endpoint.</param>
        /// <returns></returns>
        public static async Task<IEnumerable<User>> GetUserInfoAsync(IEnumerable<string> userUIDs, string accessToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Queries the Auth0 servers for the user search term.
        /// </summary>
        /// <param name="term">Term to search for.</param>
        /// <param name="accessToken">Authentication access token for the Vocalia endpoint.</param>
        /// <returns></returns>
        public static async Task<IEnumerable<User>> SearchUsersAsync(string term, string accessToken)
        {
            throw new NotImplementedException();
        }
    }
}
