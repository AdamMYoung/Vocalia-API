using System;
using System.Collections.Generic;

namespace Vocalia.Publishing.DTOs
{
    public class UnassignedPodcast
    {
        public Guid UID { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }

        public IEnumerable<UnassignedEpisode> Episodes { get; set; } = new List<UnassignedEpisode>();
    }
}
