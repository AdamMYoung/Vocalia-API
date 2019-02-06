using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vocalia.Facades.GPodder;
using Vocalia.Podcast.Db;
using Microsoft.EntityFrameworkCore;
using Vocalia.Podcast.Facades.iTunes;
using CodeHollow.FeedReader;
using Microsoft.Extensions.Caching.Memory;

namespace Vocalia.Podcast.Repositories
{
    public class PodcastRepository : IPodcastRepository
    {
        /// <summary>
        /// Default number of podcasts to return.
        /// </summary>
        private readonly int defaultPodcastCount = 100;

        /// <summary>
        /// Service for fetching GPodder podcast data.
        /// </summary>
        private IGPodderFacade GPodderService { get; }

        /// <summary>
        /// Service for fetching iTunes podcast data.
        /// </summary>
        private IITunesFacade ITunesService { get; }

        /// <summary>
        /// DB context for podcast data.
        /// </summary>
        private PodcastContext DbContext { get; }

        /// <summary>
        /// In-memory cache for faster data access.
        /// </summary>
        private IMemoryCache Cache { get; }

        /// <summary>
        /// Initializes a new PodcastRepository.
        /// </summary>
        /// <param name="context">Vocalia database reference.</param>
        /// <param name="gpodderFacade">GPodder API service.</param>
        /// <param name="iTunesFacade">iTunes API service.</param>
        /// <param name="cache">Cache object for storing pre-fetched data.</param>
        public PodcastRepository(PodcastContext context, IGPodderFacade gpodderFacade, IITunesFacade iTunesFacade, IMemoryCache cache)
        {
            DbContext = context;
            GPodderService = gpodderFacade;
            ITunesService = iTunesFacade;
            Cache = cache;
        }

        /// <summary>
        /// Returns all categories from the Voalica service.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<DomainModels.Category>> GetCategoriesAsync()
        {
            if (!Cache.TryGetValue(CacheKeys.Categories, out IEnumerable<DomainModels.Category> categories))
            {
                categories = await DbContext.Categories.Select(c => new DomainModels.Category()
                {
                    ID = c.ID,
                    LanguageID = c.LanguageID,
                    ITunesID = c.ITunesID,
                    GPodderTag = c.GpodderTag,
                    Title = c.Title,
                    IconUrl = c.IconUrl
                }).ToListAsync();

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(DateTime.Now.AddHours(1));
                Cache.Set(CacheKeys.Categories, categories, cacheEntryOptions);
            }

            return categories;
        }

        #region Podcasts

        /// <summary>
        /// Queries sources for the specified search term.
        /// </summary>
        /// <param name="query">Value to search for.</param>
        /// <param name="limit">Number of items to return.</param>
        /// <returns></returns>
        public async Task<IEnumerable<DomainModels.Podcast>> SearchPodcastsAsync(string query, int limit, bool alowExplicit)
        {
            if (!Cache.TryGetValue(query, out IEnumerable<DomainModels.Podcast> podcasts))
            {
                var fetchedPodcasts = new List<DomainModels.Podcast>();
                var iTunes = await ITunesService.SearchPodcastsAsync(query, limit, "gb", alowExplicit);

                podcasts =  iTunes.Select(p => new DomainModels.Podcast
                {
                    Title = p.Name,
                    RssUrl = p.RssUrl,
                    ImageUrl = p.ImageUrl
                });

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(new TimeSpan(3,0,0,0));
                Cache.Set(query, podcasts, cacheEntryOptions);
            }

            return podcasts;
        }

        /// <summary>
        /// Gets the top podcasts from the cache, or queries data sources if not present.
        /// </summary>
        /// <param name="limit">Number of entries to return.</param>
        /// <param name="categoryId">ID of the category to filter.</param>
        /// <param name="allowExplicit">Filters child-friendly content.</param>
        /// <returns></returns>
        public async Task<IEnumerable<DomainModels.Podcast>> GetTopPodcastsAsync(int limit, int? categoryId, bool allowExplicit)
        {
            var cacheTerm = categoryId.HasValue ? CacheKeys.Podcasts + categoryId.Value : CacheKeys.Podcasts;

            //Cache the podcast results for less impact on APIs.
            if (!Cache.TryGetValue(cacheTerm, out IEnumerable<DomainModels.Podcast> podcasts))
            {
                podcasts = await QueryTopPodcastsAsync(limit, categoryId, allowExplicit);

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(DateTime.Now.AddDays(1));
                Cache.Set(cacheTerm, podcasts, cacheEntryOptions);
            }

            return podcasts;
        }

        /// <summary>
        /// Queries data sources for the top podacsts based on filter options.
        /// </summary>
        /// <param name="limit">Number of entries to return.</param>
        /// <param name="categoryId">ID of the category to filter.</param>
        /// <param name="allowExplicit">Filters child-friendly content.</param>
        /// <returns></returns>
        public async Task<IEnumerable<DomainModels.Podcast>> QueryTopPodcastsAsync(int limit, int? categoryId, bool allowExplicit)
        {
            Db.Category category = null;

            if (categoryId.HasValue)
                category = await DbContext.Categories.Include(c => c.Language).FirstOrDefaultAsync(c => c.ID == categoryId.Value);
            var countryCode = category?.Language.ISOCode ?? "gb";


            var fetchedPodcasts = new List<DomainModels.Podcast>();

            fetchedPodcasts.AddRange(await GetVocaliaPodcastsAsync(limit, category?.ID, countryCode, allowExplicit));
            fetchedPodcasts.AddRange(await GetiTunesPodcastsAsync(limit, category?.ITunesID, countryCode, allowExplicit));

            //GPodder can't filter explicit data so only queries if allowExplicit is true.
            if (allowExplicit && fetchedPodcasts.Count < limit)
                fetchedPodcasts.AddRange(await GetGPodderPodcastsAsync(limit, category?.GpodderTag));

           return fetchedPodcasts.Where(p => p.RssUrl != null).Distinct(new PodcastEqualityComparator()).Take(limit);
        }

        /// <summary>
        /// Queries the vocalia database for podcast entries.
        /// </summary>
        /// <param name="count">Number of items to fetch.</param>
        /// <param name="categoryId">Optional category to filter by.</param>
        /// <param name="allowExplicit">Filters child-friendly content.</param>
        /// <returns></returns>
        private async Task<IEnumerable<DomainModels.Podcast>> GetVocaliaPodcastsAsync(int count, int? categoryId, string isoCountryCode, bool allowExplicit)
        {
            var vocaliaPodcasts = DbContext.Podcasts.Where(x => x.Language.ISOCode == isoCountryCode).AsQueryable();
            if (categoryId.HasValue)
                vocaliaPodcasts = vocaliaPodcasts.Where(x => x.CategoryID == categoryId.Value);

            if (allowExplicit == false)
                vocaliaPodcasts = vocaliaPodcasts.Where(x => x.IsExplicit == false);

            return await vocaliaPodcasts.OrderByDescending(x => x.Subscribers).Take(count).Select(p => new DomainModels.Podcast()
            {
                ID = p.ID,
                Title = p.Title,
                RssUrl = p.RSS,
                ImageUrl = p.ImageUrl
            }).ToListAsync();
        }

        /// <summary>
        /// Queries the iTunes service for podcast entries.
        /// </summary>
        /// <param name="count">Number of podcasts to fetch.</param>
        /// <param name="iTunesId">Optional ID for filtering.</param>
        /// <param name="allowExplicit">Filters child-friendly content.</param>
        /// <returns></returns>
        private async Task<IEnumerable<DomainModels.Podcast>> GetiTunesPodcastsAsync(int count, int? iTunesId, string isoCountryCode, bool allowExplicit)
        {
            var iTunesPodcasts = await ITunesService.GetTopPodcastsAsync(count, isoCountryCode, allowExplicit, iTunesId);
            return iTunesPodcasts.Select(p => new DomainModels.Podcast()
            {
                Title = p.Name,
                RssUrl = p.RssUrl,
                ImageUrl = p.ImageUrl
            });
        }

        /// <summary>
        /// Queries the GPodder service for podcast entries.
        /// </summary>
        /// <param name="count">Number of podcasts to fetch.</param>
        /// <param name="gpodderTag">Optional tag for category filtering.</param>
        /// <returns></returns>
        private async Task<IEnumerable<DomainModels.Podcast>> GetGPodderPodcastsAsync(int count, string gpodderTag)
        {
            var gpodderPodcasts = await GPodderService.GetTopPodcastsAsync(count, gpodderTag);
            return gpodderPodcasts.Select(p => new DomainModels.Podcast()
            {
                Title = p.Name,
                RssUrl = p.RssUrl,
                ImageUrl = p.ImageUrl
            });
        }

        #endregion

        #region RSS Feed

        /// <summary>
        /// Parses an RSS feed into C# DTOs to serialize, allowing additional information to be added such as listen times and listen info.
        /// </summary>
        /// <param name="rssUrl">URL to parse.</param>
        /// <returns></returns>
        public async Task<DTOs.Feed> GetFeedFromUrl(string rssUrl)
        {
            var cacheTerm = CacheKeys.Feed + rssUrl;
            if (!Cache.TryGetValue(cacheTerm, out DTOs.Feed feedEntry))
            {
                var feed = await FeedReader.ReadAsync(rssUrl);
                if (feed == null)
                    return null;

                feedEntry = new DTOs.Feed()
                {
                    Title = feed.Title,
                    Link = feed.Link,
                    Description = feed.Description,
                    Copyright = feed.Copyright,
                    ImageUrl = feed.ImageUrl,
                    Items = feed.Items.Select(i => new DTOs.FeedItem()
                    {
                        Title = i.Title,
                        Link = i.Link,
                        ImageUrl = feed.ImageUrl,
                        Description = i.Description,
                        PublishingDate = i.PublishingDate,
                        Author = feed.Title,
                        Id = i.Id,
                        Content = i.SpecificItem.Element.Elements("enclosure").FirstOrDefault().Attribute("url").Value
                    })
                };

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(DateTime.Now.AddHours(2));
                Cache.Set(cacheTerm, feedEntry, cacheEntryOptions);
            }

            return feedEntry;
        }

        #endregion
    }

    /// <summary>
    /// Comparitor for checking if two podcast entries are the same when using the .Distinct() LINQ equality comparitor expression.
    /// </summary>
    internal class PodcastEqualityComparator : IEqualityComparer<DomainModels.Podcast>
    {
        public bool Equals(DomainModels.Podcast x, DomainModels.Podcast y)
        {
            return x.Title == y.Title || x.RssUrl == y.RssUrl;
        }

        public int GetHashCode(DomainModels.Podcast obj)
        {
            return obj.GetHashCode();
        }
    }
}
