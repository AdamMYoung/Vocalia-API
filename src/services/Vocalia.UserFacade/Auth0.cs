using RestSharp;
using System;
using System.Threading.Tasks;

namespace Vocalia.UserFacade
{
    public class Auth0
    {
        /// <summary>
        /// Queries the Auth0 servers for user info.
        /// </summary>
        /// <param name="userUID"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static async Task<User> GetUserAsync(string userUID, string accessToken)
        {
            var client = new RestClient($"https://vocalia.eu.auth0.com/api/v2/users/{userUID}");
            var request = new RestRequest(Method.GET);
            request.AddHeader("authorization", $"Bearer {accessToken}");

            var response = await client.ExecuteTaskAsync<User>(request);
            return response.Data;
        }
    }
}
