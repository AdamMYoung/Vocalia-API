﻿using System;

namespace Vocalia.Podcast.DomainModels
{
    public class FeedItem
    {

        /// <summary>
        /// URL of the image.
        /// </summary>
        public string ImageUrl { get; internal set; }

        /// <summary>
        /// The title of the feed item
        /// </summary>
        public string Title { get; internal set; }

        /// <summary>
        /// RSS url of the episode.
        /// </summary>
        public string RssUrl { get; internal set; }

        /// <summary>
        /// The description of the feed item
        /// </summary>
        public string Description { get; internal set; }

        /// <summary>
        /// The published date as a datetime.
        /// </summary>
        public DateTime? PublishingDate { get; internal set; }

        /// <summary>
        /// The author of the feed item
        /// </summary>
        public string Author { get; internal set; }

        /// <summary>
        /// The id of the feed item
        /// </summary>
        public string Id { get; internal set; }

        /// <summary>
        /// The content of the feed item
        /// </summary>
        public string Content { get; internal set; }

        /// <summary>
        /// Time to start playing at.
        /// </summary>
        public int Time { get; internal set; } = 0;

        /// <summary>
        /// Duration of the episode.
        /// </summary>
        public int Duration { get; internal set; } = 0;

        /// <summary>
        /// Indicates if the podcast has been completed previously.
        /// </summary>
        public bool IsCompleted { get; internal set;  } = false;

        /// <summary>
        /// Indicates if the podcast data should be stored for local playback.
        /// </summary>
        public bool StoreLocally { get; internal set; } = false;
    }
}
