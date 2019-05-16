using System.Collections.Generic;

namespace Vocalia.Podcast.Db
{
    public class Language
    {
        public virtual int ID { get; set; }
        public virtual string Name { get; set; }
        public virtual string ISOCode { get; set; }

        public virtual IEnumerable<Podcast> Podcasts { get; set; }
    }
}
