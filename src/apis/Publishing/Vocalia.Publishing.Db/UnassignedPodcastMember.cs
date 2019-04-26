using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Publishing.Db
{
    public class UnassignedPodcastMember
    {
        public virtual int ID { get; set; }
        public virtual int UnassignedPodcastID { get; set; }
        public virtual string UserUID { get; set; }

        public virtual UnassignedPodcast Podcast { get; set; }
    }
}
