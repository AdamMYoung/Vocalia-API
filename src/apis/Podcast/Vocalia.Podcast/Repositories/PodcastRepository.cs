using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vocalia.Facades.GPodder;
using Vocalia.Podcast.Db;
using Microsoft.EntityFrameworkCore;
using Vocalia.Podcast.DomainModels;
using Vocalia.Podcast.Facades.iTunes;

namespace Vocalia.Podcast.Repositories
{
    public class PodcastRepository : IPodcastRepository
    {
        private readonly int defaultPodcastCount = 200;

        private IGPodderFacade GPodderService { get; }
        private IITunesFacade ITunesService { get; }
        private PodcastContext DbContext { get; }

        public PodcastRepository(PodcastContext context, IGPodderFacade gpodderFacade, IITunesFacade iTunesFacade)
        {
            DbContext = context;
            GPodderService = gpodderFacade;
            ITunesService = iTunesFacade;
        }
       
        /// <summary>
        /// Returns all categories from the Voalica service.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<DomainModels.Category>> GetCategoriesAsync()
        {
            return await DbContext.Categories.Select(c => new DomainModels.Category()
            {
                ID = c.ID,
                LanguageID = c.LanguageID,
                ITunesID = c.ITunesID,
                GPodderTag = c.GpodderTag,
                Title = c.Title,
                IconUrl = c.IconUrl
            }).ToListAsync();
        }

        /// <summary>
        /// Gets the top podcasts from the stored sources available.
        /// </summary>
        /// <param name="limit">Number of entries to return.</param>
        /// <param name="allowExplicit">Filters child-friendly content.</param>
        /// <returns></returns>
        public async Task<IEnumerable<DomainModels.Podcast>> GetTopPodcastsAsync(int? limit, int? categoryId, bool allowExplicit = true)
        {
            var count = limit ?? defaultPodcastCount;
            Db.Category category = null;

            if(categoryId.HasValue)
                category = await DbContext.Categories.Include(c => c.Language).FirstOrDefaultAsync(c => c.ID == categoryId.Value);

            var countryCode = category?.Language.ISOCode ?? "gb";
            var podcasts = new List<DomainModels.Podcast>();

            podcasts.AddRange(await GetVocaliaPodcastsAsync(count, category?.ID, countryCode, allowExplicit));
            podcasts.AddRange(await GetiTunesPodcastsAsync(count, category?.ITunesID, countryCode, allowExplicit));         
            
            //GPodder can't filter explicit data so only queries if allowExplicit is true.
            if (allowExplicit && podcasts.Count < count)
                podcasts.AddRange(await GetGPodderPodcastsAsync(count, category?.GpodderTag));

            return podcasts.Distinct(new PodcastEqualityComparator()).Take(count);
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
        private async Task<IEnumerable<DomainModels.Podcast>> GetGPodderPodcastsAsync(int count, string gpodderTag = null)
        {
            var gpodderPodcasts = await GPodderService.GetTopPodcastsAsync(count, gpodderTag);
            return gpodderPodcasts.Select(p => new DomainModels.Podcast()
            {
                Title = p.Name,
                RssUrl = p.RssUrl,
                ImageUrl = p.ImageUrl
            });
        }
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
