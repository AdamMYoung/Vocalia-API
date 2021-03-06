﻿namespace Vocalia.Podcast.DomainModels
{
    public class Listen
    {
        /// <summary>
        /// ID of the listen entry.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// ID of the user.
        /// </summary>
        public string UserUID { get; set; }

        /// <summary>
        /// RSS URL of the feed.
        /// </summary>
        public string RssUrl { get; set; }

        /// <summary>
        /// RSS URl of the episode.
        /// </summary>
        public string EpisodeUrl { get; set; }

        /// <summary>
        /// Name of the episode.
        /// </summary>
        public string EpisodeName { get; set; }

        /// <summary>
        /// Indicates if the episode has been completed.
        /// </summary>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// Current duration through the episode.
        /// </summary>
        public int Time { get; set; }

        /// <summary>
        /// Total duration through the episode.
        /// </summary>
        public int Duration { get; set; }
    }
}
