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
        /// <param name="userUID">Optional ID of the user to fetch customized information.</param>
        /// <returns></returns>
        Task<DomainModels.Feed> GetFeedFromUrlAsync(string rssUrl, string userUID = null);

        /// <summary>
        /// Gets all subscriptions for the user.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<DomainModels.Subscription>> GetSubscriptionsAsync(string userUID);

        /// <summary>
        /// Gets the latest podcast for the user.
        /// </summary>
        /// <param name="userUID">ID to check.</param>
        /// <returns></returns>
        Task<DomainModels.FeedItem> GetLatestPodcastListenedAsync(string userUID);

        /// <summary>
        /// Deletes the subscription with the specified ID in the database.
        /// </summary>
        /// <param name="rssUrl">RSS url of the entry.</param>
        /// <param name="userUID">User UID to remove the item from.</param>
        /// <returns></returns>
        Task DeleteSubscriptionAsync(string rssUrl, string userUID);

        /// <summary>
        /// Adds a subscription to the current user.
        /// </summary>
        /// <param name="subscription">Subscription to add</param>
        /// <returns></returns>
        Task AddSubscriptionAsync(DomainModels.Subscription subscription);

        /// <summary>
        /// Gets listen info for the specified RSS url.
        /// </summary>
        /// <param name="rssUrl">RSS URL of the entry.</param>
        /// <param name="userUID">User UID to get the information for.</param>
        /// <returns></returns>
        Task<DomainModels.Listen> GetListenInfoAsync(string rssUrl, string userUID);

        /// <summary>
        /// Sets information regarding listen information.
        /// </summary>
        /// <param name="listenInfo">Information to store about listen details.</param>
        /// <returns></returns>
        Task SetListenInfoAsync(DomainModels.Listen listenInfo);
    }
}
