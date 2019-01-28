using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Facades.GPodder
{
    public interface IGPodderFacade
    {
        /// <summary>
        /// Returns categories stored in the GPodder database.
        /// </summary>
        /// <param name="count">Number of categories to return.</param>
        /// <returns></returns>
        Task<IEnumerable<DTOs.CategoryTag>> GetCategoriesAsync(int count);

        /// <summary>
        /// Gets the top podcasts in the GPodder database.
        /// </summary>
        /// <param name="limit">Number of podcasts to return.</param>
        /// <returns></returns>
        Task<IEnumerable<DTOs.Podcast>> GetTopPodcastsAsync(int limit);

        /// <summary>
        /// Gets all podcasts belonging to the passed category. <see cref="DTOs.CategoryTag"/>.
        /// </summary>
        /// <param name="tag">Category to filter by.</param>
        /// <param name="count">Number of podcasts to return.</param>
        /// <returns></returns>
        Task<IEnumerable<DTOs.Podcast>> GetPodcastsByCategoryAsync(string tag, int count);
    }
}
