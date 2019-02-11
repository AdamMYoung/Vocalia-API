using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Podcast.DomainModels
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
        /// RSS URL of the episode.
        /// </summary>
        public string RssUrl { get; set; }

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
    }
}
