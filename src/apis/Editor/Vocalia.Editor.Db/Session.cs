﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Editor.Db
{
    public class Session
    {
        public virtual int ID { get; set; }
        public virtual string UID { get; set; }
        public virtual int PodcastID { get; set; }
        public virtual string IsFinishedEditing { get; set; }

        public virtual IEnumerable<User> Users { get; set; }
        public virtual Podcast Podcast { get; set; }
    }
}
