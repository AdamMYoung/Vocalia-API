using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vocalia.Facades.GPodder
{
    public interface IGPodderFacade
    {
        /// <summary>
        /// Gets the top podcasts in the GPodder database. Optionally sorts by genre.
        /// </summary>
        /// <param name="limit">Number of podcasts to return.</param>
        /// <param name="genre">Genre to search by.</param>
        /// <returns></returns>
        Task<IEnumerable<DTOs.Podcast>> GetTopPodcastsAsync(int limit, string tag = null);

        /// <summary>
        /// Searches the GPodder database for the provided query.
        /// </summary>
        /// <param name="query">String to search for.</param>
        /// <returns></returns>
        Task<IEnumerable<DTOs.Podcast>> SearchPodcastsAsync(string query);
    }
}
