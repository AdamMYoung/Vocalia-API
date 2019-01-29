using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Facades.GPodder.DTOs
{
    class Episode
    {
        public string Title { get; set; }

        public string URL { get; set; }

        [JsonProperty(PropertyName = "podcast_title")]
        public string PodcastTitle { get; set; }

        [JsonProperty(PropertyName = "podcast_url")]
        public string PodcastUrl { get; set; }

        public string Description { get; set; }

        public string Website { get; set; }

        public DateTime Released { get; set; }

        [JsonProperty(PropertyName = "mygpo_link")]
        public string MyGPOLink { get; set; }
    }
}
