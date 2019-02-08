using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Podcast.Db
{
    public class Subscription
    {
        public virtual int ID { get; set; }
        public virtual Guid GUID { get; set; }
        public virtual string UserUID { get; set; }
        public virtual string RssUrl { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual string ImageUrl { get; set; }

        public virtual IEnumerable<Listen> Listens { get; set; }
    }
}
