using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vocalia.Podcast.DomainModels;

namespace Vocalia.Podcast.Repositories
{
    public class PodcastRepository : IPodcastRepository
    {
        public Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DomainModels.Podcast>> GetPodcastsByCategoryAsync(Guid categoryId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DomainModels.Podcast>> GetTopPodcastsAsync(int? limit, bool? allowExplicit)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DomainModels.Podcast>> GetUserPodcastsAsync(string userId)
        {
            throw new NotImplementedException();
        }
    }
}
