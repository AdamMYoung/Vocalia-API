using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Ingest.DTOs
{
    public class RecordingEntry
    {
        public string UserUID { get; set; }
        public Guid SessionUID { get; set; }
        public IEnumerable<BlobEntry> Blobs = new List<BlobEntry>();
    }
}
