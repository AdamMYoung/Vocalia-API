using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.ServiceBus.Types
{
    public class RecordingChunk
    {
        /// <summary>
        /// ID of the session.
        /// </summary>
        public Guid SessionUID { get; private set; }

        /// <summary>
        /// UID of the user.
        /// </summary>
        public string UserUID { get; private set; }

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
        public RecordingChunk(Guid sessionUid, string userUid, string mediaUrl, int timestamp)
        {
            SessionUID = sessionUid;
            UserUID = userUid;
            MediaUrl = mediaUrl;
            Timestamp = timestamp;
        }
    }
}
