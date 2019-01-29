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
        /// <summary>
        /// GPodder service to query.
        /// </summary>
        private GPodderApi Service { get; set; }

        /// <summary>
        /// Instantiates a new GPodderFacade to query the GPodder podcast database.
        /// </summary>
        public GPodderFacade() => Service = RestService.For<GPodderApi>("https://gpodder.net");

        /// <summary>
        /// Gets the top podcasts in the GPodder database.
        /// </summary>
        /// <param name="limit">Number of podcasts to return.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Podcast>> GetTopPodcastsAsync(int limit, string tag = null)
        {
            var entries = tag == null ? await Service.GetTopPodcastsAsync(limit) : await Service.GetPodcastsByTagAsync(tag, limit);

            IList<Podcast> podcasts = new List<Podcast>();
            Parallel.ForEach(entries, async (file) => podcasts.Add(await Service.GetPodcastDataAsync(file.MyGPOLink)));

            return podcasts.OrderByDescending(x => x.Subscribers);
        }

        /// <summary>
        /// Searches the GPodder database for the provided query.
        /// </summary>
        /// <param name="query">String to search for.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Podcast>> SearchPodcastsAsync(string query)
        {
            var entries = await Service.SearchPodcastsAsync(query);

            IList<Podcast> podcasts = new List<Podcast>();
            Parallel.ForEach(entries, async (file) => podcasts.Add(await Service.GetPodcastDataAsync(file.MyGPOLink)));

            return podcasts.OrderByDescending(x => x.Subscribers);
        }

    }
}
