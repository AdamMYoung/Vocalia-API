using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Podcast.Db
{
    public class Language
    {
        public virtual int ID { get; set; }
        public virtual string Name { get; set; }
        public virtual string ISOCode { get; set; }

        public virtual IEnumerable<Podcast> Podcasts { get; set; }
        public virtual IEnumerable<Category> Categories { get; set; }
    }
}
