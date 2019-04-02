using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Ingest.DomainModels
{
    public class BlobEntry
    {
        public int ID { get; set; }
        public string Timestamp { get; set; }
        public Guid SessionUID { get; set; }
        public string Url { get; set; }
    }
}
