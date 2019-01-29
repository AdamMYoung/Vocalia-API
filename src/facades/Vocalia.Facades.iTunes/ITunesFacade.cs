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
            return json[0].Value<string>("feedUrl");
        }

        public async Task<IEnumerable<Vocalia.Facades.iTunes.DTOs.Podcast>> GetTopPodcastsAsync(int count, bool isExplicit = true)
        {
            var rssResult = isExplicit ? 
                await RssService.GetTopPodcastsExplicitAsync(count) : 
                await RssService.GetTopPodcastsChildFriendlyAsync(count);

            IList<Vocalia.Facades.iTunes.DTOs.Podcast> podcasts = new List<Vocalia.Facades.iTunes.DTOs.Podcast>();
            for(int i = 0; i < rssResult.Feed.Results.Count(); i++)
            {
                var feedItem = rssResult.Feed.Results[i];
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

        public Task<IEnumerable<Vocalia.Facades.iTunes.DTOs.Podcast>> SearchPodcastsAsync(string query, int? genreCode = null)
        {
            throw new NotImplementedException();
        }
    }
}
