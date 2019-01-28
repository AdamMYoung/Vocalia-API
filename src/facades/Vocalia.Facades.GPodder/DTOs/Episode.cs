using Refit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Facades.GPodder.DTOs
{
    class Episode
    {
        [AliasAs("title")]
        public string Title { get; set; }

        [AliasAs("url")]
        public string URL { get; set; }

        [AliasAs("podcast_title")]
        public string PodcastTitle { get; set; }

        [AliasAs("podcast_url")]
        public string PodcastUrl { get; set; }

        [AliasAs("description")]
        public string Description { get; set; }

        [AliasAs("website")]
        public string Website { get; set; }

        [AliasAs("released")]
        public DateTime Released { get; set; }

        [AliasAs("mygpo_link")]
        public string MyGPOLink { get; set; }
    }
}
