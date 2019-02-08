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
        /// Gets the top podcasts from the cache, or queries data sources if not present.
        /// </summary>
        /// <param name="limit">Number of entries to return.</param>
        /// <param name="categoryId">ID of the category to filter.</param>
        /// <param name="allowExplicit">Filters child-friendly content.</param>
        /// <param name="countryCode">Country to fetch data for.</param>
        /// <returns></returns>
        Task<IEnumerable<DomainModels.Podcast>> GetTopPodcastsAsync(int limit, int? categoryId, bool allowExplicit, string countryCode);

        /// <summary>
        /// Returns all categories from the Voalica service.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<DomainModels.Category>> GetCategoriesAsync();

        /// <summary>
        /// Queries sources for the specified search term.
        /// </summary>
        /// <param name="query">Value to search for.</param>
        /// <param name="limit">Number of items to return.</param>
        /// <param name="allowExplicit">Filters child-friendly content.</param>
        /// <param name="countryCode">Country to fetch data for.</param>
        /// <returns></returns>
        Task<IEnumerable<DomainModels.Podcast>> SearchPodcastsAsync(string query, int limit, bool alowExplicit, string countryCode);

        /// <summary>
        /// Parses an RSS feed into C# DTOs to serialize, allowing additional information to be added such as listen times and listen info.
        /// </summary>
        /// <param name="rssUrl">URL to parse.</param>
        /// <returns></returns>
        Task<DomainModels.Feed> GetFeedFromUrl(string rssUrl);

        /// <summary>
        /// Gets all subscriptions for the user.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<DomainModels.Subscription>> GetSubscriptions(string userUID);

        /// <summary>
        /// Deletes the subscription with the specified ID in the database.
        /// </summary>
        /// <param name="id">ID of the subscription.</param>
        /// <returns></returns>
        Task DeleteSubscription(string GUID);

        /// <summary>
        /// Adds a subscription to the current user.
        /// </summary>
        /// <param name="subscription">Subscription to add</param>
        /// <returns></returns>
        Task AddSubscription(DomainModels.Subscription subscription);
    }
}
