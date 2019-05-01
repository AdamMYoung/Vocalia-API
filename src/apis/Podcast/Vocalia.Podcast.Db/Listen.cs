using System;

namespace Vocalia.Podcast.Db
{
    public class Listen
    {
        public virtual int ID { get; set; }
        public virtual string UserUID { get; set; }
        public virtual string RssUrl { get; set; }
        public virtual string EpisodeUrl { get; set; }
        public virtual string EpisodeName { get; set; }
        public virtual bool IsCompleted { get; set; }
        public virtual int Time { get; set; }
        public virtual int Duration { get; set; }
        public virtual DateTime LastUpdated { get; set; }
    }
}
