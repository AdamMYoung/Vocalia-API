using RestSharp;
using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;

namespace Vocalia.UserFacade
{
    public class UserFacade: IUserFacade
    {
        /// <summary>
        /// API token for management access.
        /// </summary>
     
        private static string AccessToken { get; set; }

        public UserFacade()
        {
            new Timer(
                e => GetAccessToken().Wait(),
                null,
                TimeSpan.Zero,
                TimeSpan.FromHours(18));
        }

        /// <summary>
        /// Sets the access token for the Auth service.
        /// </summary>
        private static async Task GetAccessToken()
        {
            var client = new RestClient("https://vocalia.eu.auth0.com/oauth/token");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", "{\"client_id\":\"LjokVu10MznuFJhXd9ZIJwiVERJl4wqP\",\"client_secret\":\"zgeLda4nK_sVQW2_MrNrrdi0OjRzcivkkcW4NfTTnizXrkocmjiAErpVBY-cCQHm\",\"audience\":\"https://vocalia.eu.auth0.com/api/v2/\",\"grant_type\":\"client_credentials\"}", ParameterType.RequestBody);
            var response = await client.ExecuteTaskAsync<Auth0TokenResponse>(request);

            AccessToken = response.Data.access_token;
        }

        /// <summary>
        /// Queries the Auth0 servers for user info.
        /// </summary>
        /// <param name="userUID">ID to get.</param>
        /// <param name="accessToken">Authentication access token for the Vocalia endpoint.</param>
        /// <returns></returns>
        public async Task<User> GetUserInfoAsync(string userUID)
        {
            var client = new RestClient($"https://vocalia.eu.auth0.com/api/v2/users/{userUID}");
            var request = new RestRequest(Method.GET);
            request.AddHeader("authorization", $"Bearer {AccessToken}");

            var response = await client.ExecuteTaskAsync<User>(request);
            return response.Data;
        }

        /// <summary>
        /// Queries the Auth0 servers for a set of user info.
        /// </summary>
        /// <param name="userUIDs">IDs to get.</param>
        /// <param name="accessToken">Authentication access token for the Vocalia endpoint.</param>
        /// <returns></returns>
        public async Task<IEnumerable<User>> GetUserInfoAsync(IEnumerable<string> userUIDs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Queries the Auth0 servers for the user search term.
        /// </summary>
        /// <param name="term">Term to search for.</param>
        /// <param name="accessToken">Authentication access token for the Vocalia endpoint.</param>
        /// <returns></returns>
        public async Task<IEnumerable<User>> SearchUsersAsync(string term)
        {
            var searchTerm = HtmlEncoder.Create().Encode($"name:*{term}*");

            var client = new RestClient($"https://YOUR_DOMAIN/api/v2/users?q={searchTerm}&search_engine=v3");
            var request = new RestRequest(Method.GET);
            request.AddHeader("authorization", $"Bearer {AccessToken}");
            var response = await client.ExecuteGetTaskAsync<IEnumerable<User>>(request);

            return response.Data;

        }
    }
}
