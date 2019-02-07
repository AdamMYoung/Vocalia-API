using Newtonsoft.Json.Linq;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Vocalia.Facades.iTunes;
using Vocalia.Facades.iTunes.DTOs;

namespace Vocalia.Podcast.Facades.iTunes
{
    public class ITunesFacade : IITunesFacade
    {
        /// <summary>
        /// Search service for iTunes.
        /// </summary>
        private iTunesSearchApi SearchService { get; set; }

        /// <summary>
        /// RSS generator for iTunes podcast.
        /// </summary>
        private ITunesRssApi RssService { get; set; }

        /// <summary>
        /// Instantiates a new ITunesFacade.
        /// </summary>
        public ITunesFacade()
        {
            SearchService = RestService.For<iTunesSearchApi>("https://itunes.apple.com");
            RssService = RestService.For<ITunesRssApi>("https://itunes.apple.com");
        }

        /// <summary>
        /// Parses the RSS url from iTunes store data.
        /// </summary>
        /// <param name="jsonData">Data to parse.</param>
        /// <returns></returns>
        private string ParseRssUrl(string jsonData)
        {
            var json = JObject.Parse(jsonData);
            return json.Value<JArray>("results")[0].Value<string>("feedUrl");
        }

        /// <summary>
        /// Gets the top podcasts in the iTunes podcast database.
        /// </summary>
        /// <param name="count">Number of items to return. Values over 200 or under 0 will be defaulted to 200.</param>
        /// <param name="isExplicit">Toggles filtering of explicit content.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Vocalia.Facades.iTunes.DTOs.Podcast>> GetTopPodcastsAsync(int count, string languageISOCode, bool isExplicit, int? genreId = null)
        {
            if (count > 200 || count < 0)
                count = 200;

            var genre = genreId.HasValue ? genreId.Value : -1;

            var rssResult = await RssService.GetTopPodcasts(count, languageISOCode, isExplicit, genre);
            var podcasts = new List<Vocalia.Facades.iTunes.DTOs.Podcast>();
            
            var rssEntries = rssResult.Feed.Entry;

            for (int i = 0; i < rssEntries.Count(); i++)
            {
                var entry = rssEntries[i];
                podcasts.Add(new Vocalia.Facades.iTunes.DTOs.Podcast()
                {
                    Position = i,
                    PodcastId = (int)entry.Id.Attributes.ImId,
                    Name = entry.ImName.Label,
                    ArtistName = entry.ImArtist.Label,
                    ImageUrl = entry.ImImage.LastOrDefault()?.Label.AbsoluteUri
                });
            }

            Parallel.ForEach(podcasts, async (podcast) => 
            {
                try
                {
                    podcast.RssUrl = ParseRssUrl(await SearchService.GetRssFeedByIdAsync(podcast.PodcastId));
                }
                catch(Exception e)
                {
                    Console.WriteLine("ITunes Query Error: " + e);
                    podcasts.Remove(podcast);
                }

            });

            return podcasts.Where(p => p != null).OrderBy(p => p.Position);
        }

        /// <summary>
        /// Searches the iTunes database for the query term, optionally sorting by genre.
        /// </summary>
        /// <param name="query">Term to search for.</param>
        /// <param name="count">Number of results to return.</param>
        /// <param name="genreCode">Optional genre to sort by.</param>
        /// <param name="isExplicit">Toggles filtering of explicit content.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Vocalia.Facades.iTunes.DTOs.Podcast>> SearchPodcastsAsync(string query, int count, string languageISOCode, bool isExplicit)
        {
            var podcasts = await SearchService.SearchPodcastsAsync(query, count, languageISOCode, isExplicit);

            return podcasts.Results.Select(p => new Vocalia.Facades.iTunes.DTOs.Podcast()
            {
                Name = p.Name,
                ArtistName = p.Author,
                RssUrl = p.RssUrl,
                ImageUrl = p.ImageUrl
            });
        }
    }
}
