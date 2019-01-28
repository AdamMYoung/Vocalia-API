using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Podcast.Repositories
{
    interface IPodcastRepository
    {
        #region Podcasts

        Task<IEnumerable<DomainModels.Podcast>> GetTopPodcastsAsync(int? limit, bool? allowExplicit);

        Task<IEnumerable<DomainModels.Podcast>> GetUserPodcastsAsync(string userId);

        Task<IEnumerable<DomainModels.Podcast>> GetPodcastsByCategoryAsync(Guid categoryId);

        #endregion

        #region Categories

        Task<IEnumerable<DomainModels.Category>> GetCategoriesAsync();

        #endregion

    }
}
