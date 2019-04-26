using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Publishing.DomainModels
{
    public class UnassignedEpisode
    {
        public int ID { get; set; }
        public Guid UnassignedPodcastUID { get; set; }
        public Guid UID { get; set; }
        public DateTime Date { get; set; }
        public string ImageUrl { get; set; }
        public bool IsCompleted { get; set; }
    }
}
