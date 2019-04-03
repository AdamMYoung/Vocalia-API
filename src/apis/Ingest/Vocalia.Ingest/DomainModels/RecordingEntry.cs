using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
