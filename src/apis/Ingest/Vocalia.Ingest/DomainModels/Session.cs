using System;

namespace Vocalia.Ingest.DomainModels
{
    public class Session
    {
        public int ID { get; set; }
        public Guid UID { get; set; }
        public int PodcastID { get; set; }
        public DateTime Date { get; set; }
        public bool IsFinished { get; set; }
    }
}
