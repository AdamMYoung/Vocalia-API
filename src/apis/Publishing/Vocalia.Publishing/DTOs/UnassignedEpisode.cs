using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Publishing.DTOs
{
    public class UnassignedEpisode
    {
        public Guid UID { get; set; }
        public Guid PodcastUID { get; set; }
        public string Name { get; set; }
    }
}
