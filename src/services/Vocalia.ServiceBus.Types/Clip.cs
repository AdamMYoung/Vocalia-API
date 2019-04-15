using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.ServiceBus.Types
{
    public class Clip
    {
        /// <summary>
        /// UID of the session.
        /// </summary>
        public Guid SessionUID { get; private set; }

        /// <summary>
        /// UID of the podcast.
        /// </summary>
        public Guid PodcastUID { get; private set; }

        /// <summary>
        /// UID of the user.
        /// </summary>
        public string UserUID { get; private set; }

        /// <summary>
        /// UID of the clip.
        /// </summary>
        public Guid UID { get; private set; }

        /// <summary>
        /// Date of the recording chunk.
        /// </summary>
        public DateTime Date { get; private set; }

        /// <summary>
        /// URL to the media chunk.
        /// </summary>
        public string MediaUrl { get; private set; }
        
        /// <summary>
        /// Instantaites a new RecordingChunk.
        /// </summary>
        public Clip(Guid sessionUid, Guid podcastUid, string userUid, string mediaUrl, DateTime date, Guid uid)
        {
            SessionUID = sessionUid;
            PodcastUID = podcastUid;
            UserUID = userUid;
            UID = uid;
            MediaUrl = mediaUrl;
            Date = date;
        }
    }
}
