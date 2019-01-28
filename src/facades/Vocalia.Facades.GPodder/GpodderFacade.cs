using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vocalia.Facades.GPodder.DTOs;

namespace Vocalia.Facades.GPodder
{
    public class GPodderFacade : IGPodderFacade
    {
        private GPodderApi Service { get; set; } 

        public GPodderFacade()
        {
            Service = RestService.For<GPodderApi>("https://gpodder.net");
        }

        public async Task<IEnumerable<CategoryTag>> GetCategoriesAsync(int count)
        {
            var categories = await Service.GetCategoriesAsync(count);
            return categories.OrderByDescending(x => x.Usage);
        }

        public async Task<IEnumerable<Podcast>> GetTopPodcastsAsync(int limit)
        {
            var podcasts = await Service.GetTopPodcastsAsync(limit);
            return podcasts.OrderByDescending(x => x.Subscribers);
        }

        public async Task<IEnumerable<Podcast>> GetPodcastsByCategoryAsync(string tag, int count)
        {
            var podcasts = await Service.GetPodcastsByTagAsync(tag, count);
            return podcasts.OrderByDescending(x => x.Subscribers);
        }
    }
}
