using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Publishing.Db
{
    public class Category
    {
        public virtual int ID { get;set; }
        public virtual int ITunesID { get; set; }
        public virtual string GPodderTag { get; set; }
        public virtual string Title { get; set; }

        public virtual IEnumerable<Podcast> Podcasts { get; set; }
    }
}
