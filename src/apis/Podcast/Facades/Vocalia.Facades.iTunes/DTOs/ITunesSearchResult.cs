﻿using Newtonsoft.Json;

namespace Vocalia.Facades.iTunes.DTOs
{
    class ITunesSearchResult
    {
        [JsonProperty("trackName")]
        public string Name { get; set; }

        [JsonProperty("artistName")]
        public string Author { get; set; }

        [JsonProperty("artworkUrl100")]
        public string ImageUrl { get; set; }

        [JsonProperty("feedUrl")]
        public string RssUrl { get; set; }
    }
}
