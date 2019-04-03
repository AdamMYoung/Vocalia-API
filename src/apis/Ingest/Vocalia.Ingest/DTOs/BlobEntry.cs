using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Ingest.DTOs
{
    public class BlobEntry
    {
        public int Timestamp { get; set; }
        public Guid SessionUID { get; set; }
        public string Url { get; set; }
    }
}
