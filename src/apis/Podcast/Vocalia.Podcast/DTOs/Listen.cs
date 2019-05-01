namespace Vocalia.Podcast.DTOs
{
    public class Listen
    {
        /// <summary>
        /// RSS URL of the feed.
        /// </summary>
        public string RssUrl { get; set; }

        /// <summary>
        /// RSS URL of the episode.
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
        /// Total duration of the episode.
        /// </summary>
        public int Duration { get; set; }
    }
}
