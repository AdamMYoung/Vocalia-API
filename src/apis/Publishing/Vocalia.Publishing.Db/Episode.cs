using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Publishing.Db
{
    public class Episode
    {
        public virtual int ID { get; set; }
        public virtual int PodcastID { get; set; }
        public virtual Guid UID { get; set; }
        public virtual string Title { get; set; }
        public virtual string Description { get; set; }
        public virtual string RssUrl { get; set; }
        public virtual DateTime PublishDate { get; set; }
        public virtual string MediaUrl { get; set; }
        public virtual bool IsActive { get; set; }

        public virtual Podcast Podcast { get; set; }
    }
}
