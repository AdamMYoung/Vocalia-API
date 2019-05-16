using System;

namespace Vocalia.Ingest.DomainModels
{
    public class SessionClip
    {
        public Guid UID { get; set; }
        public int SessionID { get; set; }
        public string UserUID { get; set; }
        public long Size { get; set; }
        public DateTime Time { get; set; }
        public string MediaUrl { get; set; }
        public string Name { get; set; }
    }
}
