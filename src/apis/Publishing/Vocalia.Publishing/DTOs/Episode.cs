using System;

namespace Vocalia.Publishing.DTOs
{
    public class Episode
    {
        public Guid UID { get; set; }
        public string Title { get; set; }
        public Guid PodcastUID { get; set; }
        public string Description { get; set; }
        public string RssUrl { get; set; }
        public DateTime PublishDate { get; set; }
        public string MediaUrl { get; set; }
        public bool IsActive { get; set; }
    }
}
