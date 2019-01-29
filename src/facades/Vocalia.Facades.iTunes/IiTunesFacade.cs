using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Podcast.Facades.iTunes
{
    public interface IITunesFacade
    {
        /// <summary>
        /// Gets the top podcasts in the iTunes podcast database.
        /// </summary>
        /// <param name="count">Number of items to return.</param>
        /// <param name="isExplicit">Toggles filtering of explicit content.</param>
        /// <returns></returns>
        Task<IEnumerable<Vocalia.Facades.iTunes.DTOs.Podcast>> GetTopPodcastsAsync(int count, bool isExplicit = true);

        /// <summary>
        /// Searches the iTunes database for the query term, optionally sorting by genre.
        /// </summary>
        /// <param name="query">Term to search for.</param>
        /// <param name="genreCode">Optional genre to sort by.</param>
        /// <returns></returns>
        Task<IEnumerable<Vocalia.Facades.iTunes.DTOs.Podcast>> SearchPodcastsAsync(string query, int? genreCode = null);
    }
}
