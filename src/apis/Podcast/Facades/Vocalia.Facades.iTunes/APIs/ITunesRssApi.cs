using Refit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Vocalia.Facades.iTunes
{
    interface ITunesRssApi
    {
        /// <summary>
        /// Returns the top podcasts from the iTunes podcast database.
        /// </summary>
        /// <param name="count">Number of items to return.</param>
        /// <param name="isoCountryCode">Country code to search for. Default is "gb".</param>
        /// <returns></returns>
        [Get("/{isoCountryCode}/podcasts/top-podcasts/all/{count}/explicit.json")]
        Task<DTOs.RssResult> GetTopPodcastsExplicitAsync(int count, string isoCountryCode = "gb");

        /// <summary>
        /// Returns the top podcasts from the iTunes podcast database, filtering explicit content.
        /// </summary>
        /// <param name="count">Number of items to return.</param>
        /// <param name="isoCountryCode">Country code to search for. Default is "gb".</param>
        [Get("/{isoCountryCode}/podcasts/top-podcasts/all/{count}/non-explicit.json")]
        Task<DTOs.RssResult> GetTopPodcastsChildFriendlyAsync(int count, string isoCountryCode = "gb");
    }
}
