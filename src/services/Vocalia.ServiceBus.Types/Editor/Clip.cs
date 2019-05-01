using System;
using System.Collections.Generic;

namespace Vocalia.ServiceBus.Types
{
    public class Clip
    {
        /// <summary>
        /// UID of the session.
        /// </summary>
        public Guid SessionUID { get; set; }

        /// <summary>
        /// UID of the podcast.
        /// </summary>
        public Guid PodcastUID { get; set; }

        /// <summary>
        /// UID of the clip.
        /// </summary>
        public Guid UID { get; set; }

        /// <summary>
        /// Date of the recording chunk.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Name of the clip.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// User media belonging to the clip.
        /// </summary>
        public IList<Media> Media { get; set; } = new List<Media>();

    }
}
