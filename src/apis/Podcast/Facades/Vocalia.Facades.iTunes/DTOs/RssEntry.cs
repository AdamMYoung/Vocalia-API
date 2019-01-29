using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Facades.iTunes.DTOs
{
    internal class RssEntry
    {
        public string ArtistName { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        [JsonProperty(PropertyName = "artworkUrl100")]
        public string ArtworkUrl { get; set; }
    }
}
