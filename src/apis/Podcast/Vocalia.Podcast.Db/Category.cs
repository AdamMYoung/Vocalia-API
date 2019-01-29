using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Podcast.Db
{
    public class Category
    {
        public virtual int ID { get; set; }
        public virtual int LanguageID { get; set; }
        public virtual int ITunesID { get; set; }
        public virtual int ListenNotesID { get; set; }
        public virtual string GpodderTag { get; set; }
        public virtual string Title { get; set; }
        public virtual string IconUrl { get; set; }

        public virtual Language Language { get; set; }
        public virtual IEnumerable<Podcast> Podcasts { get; set; }
    }
}
