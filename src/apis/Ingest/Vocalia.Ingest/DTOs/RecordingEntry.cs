using System;
using System.Collections.Generic;

namespace Vocalia.Ingest.DTOs
{
    public class RecordingEntry
    {
        public string UserUID { get; set; }
        public Guid SessionUID { get; set; }
        public IEnumerable<BlobEntry> Blobs = new List<BlobEntry>();
    }
}
