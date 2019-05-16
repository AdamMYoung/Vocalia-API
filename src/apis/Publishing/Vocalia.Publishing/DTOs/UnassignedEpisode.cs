using System;

namespace Vocalia.Publishing.DTOs
{
    public class UnassignedEpisode
    {
        public Guid UID { get; set; }
        public Guid PodcastUID { get; set; }
        public DateTime Date { get; set; }
        public string ImageUrl { get; set; }
    }
}
