﻿using System.Collections.Generic;

namespace Vocalia.Podcast.Db
{
    public class Category
    {
        public virtual int ID { get; set; }
        public virtual int ITunesID { get; set; }
        public virtual string GpodderTag { get; set; }
        public virtual string Title { get; set; }
        public virtual string IconUrl { get; set; }

        public virtual IEnumerable<Podcast> Podcasts { get; set; }
    }
}
