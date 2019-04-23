using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Publishing.DomainModels
{
    public class UnassignedEpisode
    {
        public virtual int ID { get; set; }
        public virtual Guid UnassignedPodcastUID { get; set; }
        public virtual Guid UID { get; set; }
        public virtual string Name { get; set; }
        public virtual bool IsCompleted { get; set; }
    }
}
