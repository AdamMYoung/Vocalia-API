using Refit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Vocalia.Facades.iTunes.DTOs;

namespace Vocalia.Facades.iTunes
{
    interface iTunesSearchApi
    {
        /// <summary>
        /// Searches the iTunes podcast database for the specified query.
        /// </summary>
        /// <param name="query">Term to search for.</param>
        /// <param name="count">Number of items to return.</param>
        /// <param name="isExplicit">Toggles filtering of explicit content.</param>
        /// <returns></returns>
        [Get("/search?term={query}&limit={count}&country={isoCountryCode}&explicit={isExplicit}&entity=podcast")]
        Task<ITunesSearch> SearchPodcastsAsync(string query, int count, string isoCountryCode, bool isExplicit = true);

        /// <summary>
        /// Searches the iTunes podcast database for the specified ID, and returns a link to the podcast's RSS feed.
        /// </summary>
        /// <param name="id">iTunes podcast ID to search for.</param>
        /// <returns></returns>
        [Get("/lookup?id={id}&entity=podcast")]
        Task<string> GetRssFeedByIdAsync(int id);
    }
}
