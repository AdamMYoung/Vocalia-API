using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Social.Db
{
    public class Listen
    {
        public virtual int ID { get; set; }
        public virtual string UserUID { get; set; }
        public virtual string RssUrl { get; set; }
        public virtual string EpisodeUrl { get; set; }
        public virtual string EpisodeName { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual bool IsCompleted { get; set; }
    }
}
