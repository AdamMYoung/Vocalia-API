using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Podcast.DomainModels
{
    public class Feed
    {
        /// <summary>
        /// The title of the field
        /// </summary>
        public string Title { get; internal set; }

        /// <summary>
        /// The link (url) to the feed
        /// </summary>
        public string Link { get; internal set; }

        /// <summary>
        /// The description of the feed
        /// </summary>
        public string Description { get; internal set; }

        /// <summary>
        /// The copyright of the feed
        /// </summary>
        public string Copyright { get; internal set; }

        /// <summary>
        /// The url of the image
        /// </summary>
        public string ImageUrl { get; internal set; }

        /// <summary>
        /// Collections of items.
        /// </summary>
        public IList<FeedItem> Items { get; internal set; }

        /// <summary>
        /// Indicates the user is subscribed to the podcast.
        /// </summary>
        public bool IsSubscribed { get; internal set; }
    }
}
