﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Podcast.DTOs
{
    public class Listen
    {
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