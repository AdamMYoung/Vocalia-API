using System;

namespace Vocalia.Ingest.DomainModels
{
    public class BlobEntry
    {
        public int ID { get; set; }
        public int Timestamp { get; set; }
        public Guid SessionUID { get; set; }
        public string Url { get; set; }
    }
}
