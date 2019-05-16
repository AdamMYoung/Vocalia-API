using System;

namespace Vocalia.Editor.DomainModels
{
    public class Session
    {
        public int ID { get; set; }
        public Guid UID { get; set; }
        public int PodcastID { get; set; }
        public DateTime Date { get; set; }
    }
}
