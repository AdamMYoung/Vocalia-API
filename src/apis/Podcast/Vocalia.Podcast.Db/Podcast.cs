using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Podcast.Db
{
    public class Podcast
    {
        public virtual int ID { get; set; }
        public virtual Guid UID { get; set; }
        public virtual int CategoryID { get; set; }
        public virtual int LanguageID { get; set; }
        public virtual string RSS { get; set; }
        public virtual bool Active { get; set; }
        public virtual bool IsExplicit { get; set; }
        public virtual string Title { get; set; }
        public virtual string ImageUrl { get; set; }
        

        public virtual Category Category { get; set; }
        public virtual Language Language { get; set; }
        public virtual IEnumerable<PodcastIntegration> Integrations { get; set; }
    }
}
