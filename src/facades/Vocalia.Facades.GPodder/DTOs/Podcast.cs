using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Facades.GPodder.DTOs
{
    public class Podcast
    {
        [JsonProperty(PropertyName = "title")]
        public string Name { get; set; }

        public string Description { get; set; }

        [JsonProperty(PropertyName = "logo_url")]
        public string ImageUrl { get; set; }

        public int Subscribers { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string RssUrl { get; set; }
    }
}
