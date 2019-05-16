using System;
using System.Collections.Generic;

namespace Vocalia.Publishing.DomainModels
{
    public class UnassignedPodcast
    {
        public int ID { get; set; }
        public Guid UID { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public bool IsCompleted { get; set; }

        public IEnumerable<UnassignedEpisode> Episodes { get; set; } = new List<UnassignedEpisode>();
    }
}
