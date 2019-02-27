﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Ingest.Db
{
    public class Session
    {
        public virtual int ID { get; set; }
        public virtual Guid UID { get; set; }
        public virtual int PodcastID { get; set; }
        public virtual DateTime Date { get; set; }

        public virtual Podcast Podcast { get; set; }
    }
}