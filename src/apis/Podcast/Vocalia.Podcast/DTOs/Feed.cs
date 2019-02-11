using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Podcast.DTOs
{
    public class Feed
    {
        /// <summary>
        /// The title of the field
        /// </summary>
        public string Title { get;  set; }

        /// <summary>
        /// The link (url) to the feed
        /// </summary>
        public string Link { get;  set; }

        /// <summary>
        /// The description of the feed
        /// </summary>
        public string Description { get;  set; }

        /// <summary>
        /// The copyright of the feed
        /// </summary>
        public string Copyright { get;  set; }

        /// <summary>
        /// The url of the image
        /// </summary>
        public string ImageUrl { get;  set; }

        /// <summary>
        /// Collections of items.
        /// </summary>
        public IEnumerable<FeedItem> Items { get;  set; }

        /// <summary>
        /// Indicates the user is subscribed to the podcast.
        /// </summary>
        public bool IsSubscribed { get;  set; }
    }
}
