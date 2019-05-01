using System;
using System.Collections.Generic;

namespace Vocalia.Ingest.DomainModels
{
    public class RecordingEntry
    {
        public int ID { get; set; }
        public Guid SessionUID { get; set; }
        public string UserUID { get; set; }
        public IEnumerable<BlobEntry> Blobs = new List<BlobEntry>();
    }
}
