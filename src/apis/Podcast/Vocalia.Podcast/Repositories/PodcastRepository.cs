using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vocalia.Facades.GPodder;
using Vocalia.Podcast.DomainModels;
using Vocalia.Podcast.Facades.iTunes;

namespace Vocalia.Podcast.Repositories
{
    public class PodcastRepository : IPodcastRepository
    {
        private readonly int defaultPodcastCount = 20;

        private IGPodderFacade GPodderService { get; }
        private IITunesFacade ITunesService { get; }

        public PodcastRepository(IGPodderFacade gpodderFacade, IITunesFacade iTunesFacade)
        {
            GPodderService = gpodderFacade;
            ITunesService = iTunesFacade;
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<DomainModels.Podcast>> GetTopPodcastsAsync(int? limit, bool allowExplicit = true)
        {
            var count = limit ?? defaultPodcastCount;

            throw new NotImplementedException();
        }
    }
}
