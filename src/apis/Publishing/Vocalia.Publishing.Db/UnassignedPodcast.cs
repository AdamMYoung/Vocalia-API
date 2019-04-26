using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Publishing.Db
{
    public class UnassignedPodcast
    {
        public virtual int ID { get; set; }
        public virtual Guid UID { get; set; }
        public virtual string Name { get; set; }
        public virtual string ImageUrl { get; set; }
        public virtual bool IsCompleted { get; set; }

        public virtual IEnumerable<UnassignedPodcastMember> Members { get; set; }
        public virtual IEnumerable<UnassignedEpisode> Episodes { get; set; }
    }
}
