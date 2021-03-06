﻿using CodeHollow.FeedReader;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ObjectBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vocalia.Facades.GPodder;
using Vocalia.Podcast.Db;
using Vocalia.Podcast.Facades.iTunes;

namespace Vocalia.Podcast.Repositories
{
    public class PodcastRepository : IPodcastRepository
    {
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
        /// Namespace for parsing itunes feed links.
        /// </summary>
        private string ITunesNamespace { get; } = "http://www.itunes.com/dtds/podcast-1.0.dtd";

        /// <summary>
        /// Initializes a new PodcastRepository.
        /// </summary>
        /// <param name="context">Vocalia database reference.</param>
        /// <param name="gpodderFacade">GPodder API service.</param>
        /// <param name="iTunesFacade">iTunes API service.</param>
        /// <param name="cache">Cache object for storing pre-fetched data.</param>
        public PodcastRepository(PodcastContext context, IGPodderFacade gpodderFacade, 
            IITunesFacade iTunesFacade, IMemoryCache cache,
            IObjectBus<Vocalia.ServiceBus.Types.Podcast.Podcast> podcastBus)
        {
            DbContext = context;
            GPodderService = gpodderFacade;
            ITunesService = iTunesFacade;
            Cache = cache;

            _ = podcastBus;
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
        /// <param name="allowExplicit">Filters child-friendly content.</param>
        /// <param name="countryCode">Country to fetch data for.</param>
        /// <returns></returns>
        public async Task<IEnumerable<DomainModels.Podcast>> SearchPodcastsAsync(string query, int limit, bool alowExplicit, string countryCode)
        {
            var cacheTerm = query + alowExplicit;
            if (!Cache.TryGetValue(cacheTerm, out IEnumerable<DomainModels.Podcast> podcasts))
            {
                var fetchedPodcasts = new List<DomainModels.Podcast>();
                var iTunesPodcasts = await ITunesService.SearchPodcastsAsync(query, limit, countryCode, alowExplicit);

                var dbPodcasts = DbContext.Podcasts.Where(c => c.Active && c.Title.Contains(query)).Take(limit);
                if (!alowExplicit)
                    dbPodcasts = dbPodcasts.Where(c => c.IsExplicit == false);

                fetchedPodcasts.AddRange(dbPodcasts.Select(p => new DomainModels.Podcast
                {
                    Title = p.Title,
                    RssUrl = p.RSS,
                    ImageUrl = p.ImageUrl
                }));

                fetchedPodcasts.AddRange(iTunesPodcasts.Select(p => new DomainModels.Podcast
                {
                    Title = p.Name,
                    RssUrl = p.RssUrl,
                    ImageUrl = p.ImageUrl
                }));

                podcasts = fetchedPodcasts;

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(DateTime.Now.AddHours(1));
                Cache.Set(cacheTerm, podcasts, cacheEntryOptions);
            }

            return podcasts;
        }

        /// <summary>
        /// Gets the top podcasts from the cache, or queries data sources if not present.
        /// </summary>
        /// <param name="limit">Number of entries to return.</param>
        /// <param name="categoryId">ID of the category to filter.</param>
        /// <param name="allowExplicit">Filters child-friendly content.</param>
        /// <param name="countryCode">Country to fetch data for.</param>
        /// <returns></returns>
        public async Task<IEnumerable<DomainModels.Podcast>> GetTopPodcastsAsync(int limit, int? categoryId, bool allowExplicit, string countryCode)
        {
            var cacheTerm = (categoryId.HasValue ? CacheKeys.Podcasts + categoryId.Value : CacheKeys.Podcasts) + allowExplicit;

            //Cache the podcast results for less impact on APIs.
            if (!Cache.TryGetValue(cacheTerm, out IEnumerable<DomainModels.Podcast> podcasts))
            {
                podcasts = await QueryTopPodcastsAsync(limit, categoryId, allowExplicit, countryCode);

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(DateTime.Now.AddDays(1));
                podcasts.Select(x => ReplaceHttpWithHttps(x.ImageUrl));
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
        /// <param name="countryCode">Country to fetch data for.</param>
        /// <returns></returns>
        public async Task<IEnumerable<DomainModels.Podcast>> QueryTopPodcastsAsync(int limit, int? categoryId, bool allowExplicit, string countryCode)
        {
            Db.Category category = null;

            if (categoryId.HasValue)
            {
                category = await DbContext.Categories.FirstOrDefaultAsync(c => c.ID == categoryId.Value);
            }

            var fetchedPodcasts = new List<DomainModels.Podcast>();

            fetchedPodcasts.AddRange(await GetVocaliaPodcastsAsync(limit, category?.ID, countryCode, allowExplicit));
            fetchedPodcasts.AddRange(await GetiTunesPodcastsAsync(limit, category?.ITunesID, countryCode, allowExplicit));

            //GPodder can't filter explicit data so only queries if allowExplicit is true.
            if (allowExplicit && fetchedPodcasts.Count < limit)
            {
                fetchedPodcasts.AddRange(await GetGPodderPodcastsAsync(limit, category?.GpodderTag));
            }

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
            {
                vocaliaPodcasts = vocaliaPodcasts.Where(x => x.CategoryID == categoryId.Value);
            }

            if (allowExplicit == false)
            {
                vocaliaPodcasts = vocaliaPodcasts.Where(x => x.IsExplicit == false);
            }

            return await vocaliaPodcasts.Take(count).Select(p => new DomainModels.Podcast()
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
        public async Task<IEnumerable<DomainModels.Podcast>> GetGPodderPodcastsAsync(int count, string gpodderTag)
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
        /// <param name="userUID">Optional ID of the user to fetch customized information.</param>
        /// <returns></returns>
        public async Task<DomainModels.Feed> GetFeedFromUrlAsync(string rssUrl, string userUID = null)
        {

            var cacheTerm = CacheKeys.Feed + rssUrl;
            if (!Cache.TryGetValue(cacheTerm, out DomainModels.Feed feedEntry))
            {
                var feed = await FeedReader.ReadAsync(rssUrl);

                if (feed == null)
                {
                    return null;
                }

                feedEntry = new DomainModels.Feed()
                {
                    Title = feed.Title,
                    Link = ReplaceHttpWithHttps(rssUrl),
                    Description = feed.Description,
                    Copyright = feed.Copyright,
                    IsSubscribed = false,
                    ImageUrl = ReplaceHttpWithHttps(feed.SpecificFeed?.Element?.Element("{" + ITunesNamespace + "}image")?.Attribute("href")?.Value ?? feed.ImageUrl),

                };

                feedEntry.Items = feed.Items.Select(i => new DomainModels.FeedItem()
                {
                    Title = i.Title,
                    RssUrl = feedEntry.Link,
                    ImageUrl = feedEntry.ImageUrl,
                    Description = i.Description,
                    PublishingDate = i.PublishingDate,
                    Author = feed.Title,
                    Id = i.Id,
                    Time = 0,
                    Content = ReplaceHttpWithHttps(i?.SpecificItem?.Element?.Elements("enclosure")?.FirstOrDefault()?.Attribute("url")?.Value ?? i.Content)
                }).ToList();

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(DateTime.Now.AddHours(2));
                Cache.Set(cacheTerm, feedEntry, cacheEntryOptions);
            }

            //Assign listen history outside of cache block.
            feedEntry.IsSubscribed = userUID != null ? await DbContext.Subscriptions.AnyAsync(s => s.RssUrl == rssUrl && s.UserUID == userUID) : false;
            var listenHistory = userUID != null ? await DbContext.Listens.Where(c => c.UserUID == userUID && c.RssUrl == rssUrl).ToListAsync() : null;

            foreach (var entry in feedEntry.Items)
            {
                var listen = listenHistory?.FirstOrDefault(c => c.EpisodeUrl == entry.Content);
                entry.Time = listen?.Time ?? 0;
                entry.Duration = listen?.Duration ?? 0;
                entry.IsCompleted = listen?.IsCompleted ?? false;
            }

            return feedEntry;
        }

        /// <summary>
        /// Replaces any occurance of "Http://" with "Https://".
        /// </summary>
        /// <param name="link">:ink to replace.</param>
        /// <returns></returns>
        private string ReplaceHttpWithHttps(string link)
        {
            if (link == null)
                return "";

            return link.Replace("http://", "https://");
        }

        #endregion

        #region Subscriptions

        /// <summary>
        /// Gets all subscriptions for the user.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<DomainModels.Subscription>> GetSubscriptionsAsync(string userUID)
        {
            return await DbContext.Subscriptions.Where(x => x.UserUID == userUID).Select(s => new DomainModels.Subscription()
            {
                ID = s.ID,
                UserUID = s.UserUID,
                RssUrl = s.RssUrl,
                Name = s.Name,
                ImageUrl = s.ImageUrl
            }).ToListAsync();
        }

        /// <summary>
        /// Deletes the subscription with the specified ID in the database.
        /// </summary>
        /// <param name="id">ID of the subscription.</param>
        /// <returns></returns>
        public async Task DeleteSubscriptionAsync(string rssUrl, string userUID)
        {
            var subscription = await DbContext.Subscriptions
                .FirstOrDefaultAsync(s => s.RssUrl == rssUrl && s.UserUID == userUID);

            if (subscription != null)
            {
                DbContext.Subscriptions.Remove(subscription);
                await DbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Adds a subscription to the current user.
        /// </summary>
        /// <param name="subscription">Subscription to add</param>
        /// <returns></returns>
        public async Task AddSubscriptionAsync(DomainModels.Subscription subscription)
        {
            await DbContext.Subscriptions.AddAsync(new Db.Subscription
            {
                Name = subscription.Name,
                RssUrl = subscription.RssUrl,
                UserUID = subscription.UserUID,
                ImageUrl = subscription.ImageUrl
            });

            await DbContext.SaveChangesAsync();
        }
        #endregion

        #region Listen

        /// <summary>
        /// Gets listen info for the specified RSS url.
        /// </summary>
        /// <param name="rssUrl">RSS URL of the entry.</param>
        /// <param name="userUID">User UID to get the information for.</param>
        /// <returns></returns>
        public async Task<DomainModels.Listen> GetListenInfoAsync(string rssUrl, string userUID)
        {
            var entry = await DbContext.Listens.FirstOrDefaultAsync(l => l.EpisodeUrl == rssUrl && l.UserUID == userUID);

            if (entry == null)
            {
                return null;
            }

            return new DomainModels.Listen
            {
                ID = entry.ID,
                EpisodeName = entry.EpisodeName,
                IsCompleted = entry.IsCompleted,
                RssUrl = entry.RssUrl,
                EpisodeUrl = entry.EpisodeUrl,
                Time = entry.Time,
                UserUID = entry.UserUID,
                Duration = entry.Duration
            };
        }

        /// <summary>
        /// Sets information regarding listen information.
        /// </summary>
        /// <param name="listenInfo">Information to store about listen details.</param>
        /// <returns></returns>
        public async Task SetListenInfoAsync(DomainModels.Listen listenInfo)
        {
            var entry = await DbContext.Listens
                .FirstOrDefaultAsync(l => l.EpisodeUrl == listenInfo.EpisodeUrl && l.UserUID == listenInfo.UserUID);

            if (entry == null)
            {
                var newEntry = new Db.Listen
                {
                    EpisodeName = listenInfo.EpisodeName,
                    RssUrl = listenInfo.RssUrl,
                    EpisodeUrl = listenInfo.EpisodeUrl,
                    UserUID = listenInfo.UserUID,
                    IsCompleted = listenInfo.IsCompleted,
                    Time = listenInfo.Time,
                    LastUpdated = DateTime.UtcNow,
                    Duration = listenInfo.Duration
                };
                await DbContext.Listens.AddAsync(newEntry);
            }
            else
            {
                entry.IsCompleted = listenInfo.IsCompleted;
                entry.Time = listenInfo.Time;
                entry.LastUpdated = DateTime.UtcNow;
            }
            await DbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Gets the latest podcasts for the user.
        /// </summary>
        /// <param name="userUID">ID to check.</param>
        /// <returns></returns>
        public async Task<DomainModels.FeedItem> GetLatestPodcastListenedAsync(string userUID)
        {
            var listen = await DbContext.Listens.OrderByDescending(p => p.LastUpdated)
                .FirstOrDefaultAsync(l => l.UserUID == userUID);

            if (listen == null)
                return null;


            var rssFeed = await GetFeedFromUrlAsync(listen.RssUrl, userUID);

            return new DomainModels.FeedItem
            {
                Title = listen.EpisodeName,
                Author = rssFeed.Items[0].Author,
                RssUrl = listen.RssUrl,
                Content = listen.EpisodeUrl,
                IsCompleted = listen.IsCompleted,
                Time = listen.Time,
                ImageUrl = rssFeed.ImageUrl,
                Duration = listen.Duration
            };
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
