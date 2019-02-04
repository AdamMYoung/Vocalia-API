using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vocalia.Podcast.DTOs;

namespace Vocalia.Podcast.Repositories
{
    public interface IPodcastRepository
    {
        /// <summary>
        /// Gets the top podcasts from the stored sources available.
        /// </summary>
        /// <param name="limit">Number of entries to return.</param>
        /// <param name="allowExplicit">Filters child-friendly content.</param>
        /// <returns></returns>
        Task<IEnumerable<DomainModels.Podcast>> GetTopPodcastsAsync(int? limit, int? categoryId, bool allowExplicit = true);

        /// <summary>
        /// Returns all categories from the Voalica service.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<DomainModels.Category>> GetCategoriesAsync();

        /// <summary>
        /// Parses an RSS feed into C# DTOs to serialize, allowing additional information to be added such as listen times and listen info.
        /// </summary>
        /// <param name="rssUrl">URL to parse.</param>
        /// <returns></returns>
        Task<Feed> GetFeedFromUrl(string rssUrl);
    }
}
