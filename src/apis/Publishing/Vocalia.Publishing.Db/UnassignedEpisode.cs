using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Publishing.Db
{
    public class UnassignedEpisode
    {
        public virtual int ID { get; set; }
        public virtual int UnassignedPodcastID { get; set; }
        public virtual Guid UID { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual bool IsCompleted { get; set; }

        public virtual IEnumerable<UnassignedEpisodeClip> Clips { get; set; }
        public virtual UnassignedPodcast Podcast { get; set; }
    }
}
