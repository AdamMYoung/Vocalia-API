using Refit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Vocalia.Facades.GPodder.DTOs;

namespace Vocalia.Facades.GPodder
{
    interface GPodderApi
    {
        /// <summary>
        /// Returns the top podcasts from the GPodder podcast database.
        /// </summary>
        /// <param name="number">Number of podcasts to return.</param>
        /// <returns></returns>
        [Get("/toplist/{number}.json")]
        Task<IEnumerable<Podcast>> GetTopPodcastsAsync(int number);

        /// <summary>
        /// Returns the top podcasts from the GPodder database sorting by tag.
        /// </summary>
        /// <param name="tag">GPodder tag to search for.</param>
        /// <param name="count">Number of podcasts to return.</param>
        /// <returns></returns>
        [Get("/api/2/tag/{tag}/{count}.json")]
        Task<IEnumerable<Podcast>> GetPodcastsByTagAsync(string tag, int count);

        /// <summary>
        /// Searches the GPodder database for the provided query.
        /// </summary>
        /// <param name="query">Query to search for.</param>
        /// <returns></returns>
        [Get("/search.json")]
        Task<IEnumerable<Podcast>> SearchPodcastsAsync([AliasAs("q")] string query);
    }
}
