using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vocalia.Facades.GPodder;
using Vocalia.Podcast.Db;
using Vocalia.Podcast.DomainModels;
using Vocalia.Podcast.Facades.iTunes;

namespace Vocalia.Podcast.Repositories
{
    public class PodcastRepository : IPodcastRepository
    {
        private readonly int defaultPodcastCount = 20;

        private IGPodderFacade GPodderService { get; }
        private IITunesFacade ITunesService { get; }
        private PodcastContext DbContext { get; }

        public PodcastRepository(PodcastContext context, IGPodderFacade gpodderFacade, IITunesFacade iTunesFacade)
        {
            DbContext = context;
            GPodderService = gpodderFacade;
            ITunesService = iTunesFacade;
        }

        public async Task<IEnumerable<DomainModels.Category>> GetCategoriesAsync()
        {
            var categories = await DbContext.Categories.Select(c => new DomainModels.Category()
            {
                ID = c.ID,
                LanguageID = c.LanguageID,
                ITunesID = c.ITunesID,
                ListenNotesID = c.ListenNotesID,
                GPodderTag = c.GpodderTag,
                Title = c.Title,
                IconUrl = c.IconUrl
            }).ToAsyncEnumerable();
            
            return categories;
        }

        public async Task<IEnumerable<DomainModels.Podcast>> GetTopPodcastsAsync(int? limit, bool allowExplicit = true)
        {
            var count = limit ?? defaultPodcastCount;

            throw new NotImplementedException();
        }
    }
}
