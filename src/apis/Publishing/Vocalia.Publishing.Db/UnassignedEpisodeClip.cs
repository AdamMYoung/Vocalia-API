using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Publishing.Db
{
    public class UnassignedEpisodeClip
    {
        public virtual int ID { get; set; }
        public virtual int UnassignedEpisodeID { get; set; }
        public virtual int Position { get; set; }
        public virtual string MediaUrl { get; set; }

        public virtual UnassignedEpisode Episode { get; set; }
    }
}
