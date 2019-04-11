using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.ServiceBus.Types
{
    public class RecordingChunk
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
        /// Date of the recording chunk.
        /// </summary>
        public DateTime Date { get; private set; }

        /// <summary>
        /// URL to the media chunk.
        /// </summary>
        public string MediaUrl { get; private set; }

        /// <summary>
        /// Time of the recording chunk.
        /// </summary>
        public int Timestamp { get; private set; }
        
        /// <summary>
        /// Instantaites a new RecordingChunk.
        /// </summary>
        public RecordingChunk(Guid sessionUid, Guid podcastUid, string userUid, string mediaUrl, int timestamp, DateTime date)
        {
            SessionUID = sessionUid;
            PodcastUID = podcastUid;
            UserUID = userUid;
            MediaUrl = mediaUrl;
            Date = date;
            Timestamp = timestamp;
        }
    }
}
