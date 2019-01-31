using Newtonsoft.Json.Linq;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
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
            RssService = RestService.For<ITunesRssApi>("https://rss.itunes.apple.com/api/v1");
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
        /// <param name="count">Number of items to return.</param>
        /// <param name="isExplicit">Toggles filtering of explicit content.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Vocalia.Facades.iTunes.DTOs.Podcast>> GetTopPodcastsAsync(int count, string languageISOCode, bool isExplicit, int? genreId = null)
        {
            var rssResult = isExplicit ? 
                await RssService.GetTopPodcastsExplicitAsync(count, languageISOCode) : 
                await RssService.GetTopPodcastsChildFriendlyAsync(count, languageISOCode);

            var filteredPodcasts = genreId.HasValue ? 
                rssResult.Feed.Results.Where(x => x.Genres.Any(c => c.GenreId == genreId.Value)).ToList() :
                rssResult.Feed.Results.ToList();

            var podcasts = new List<Vocalia.Facades.iTunes.DTOs.Podcast>();
            for(int i = 0; i < filteredPodcasts.Count(); i++)
            {
                var feedItem = filteredPodcasts[i];
                podcasts.Add(new Vocalia.Facades.iTunes.DTOs.Podcast()
                {
                    Name = feedItem.Name,
                    ArtistName = feedItem.ArtistName,
                    ImageUrl = feedItem.ArtworkUrl,
                    RssUrl = ParseRssUrl(await SearchService.GetRssFeedByIdAsync(feedItem.Id))
                });
            }

            return podcasts;
        }

        /// <summary>
        /// Searches the iTunes database for the query term, optionally sorting by genre.
        /// </summary>
        /// <param name="query">Term to search for.</param>
        /// <param name="count">Number of results to return.</param>
        /// <param name="genreCode">Optional genre to sort by.</param>
        /// <param name="isExplicit">Toggles filtering of explicit content.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Vocalia.Facades.iTunes.DTOs.Podcast>> SearchPodcastsAsync(string query, int count, string languageISOCode, bool isExplicit, int? genreCode = null)
        {
            var podcasts = await SearchService.SearchPodcastsAsync(query, count, languageISOCode, genreCode, isExplicit);
            return podcasts;
        }
    }
}
