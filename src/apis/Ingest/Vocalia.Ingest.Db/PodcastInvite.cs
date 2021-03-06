﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vocalia.Ingest.Db
{
    public class PodcastInvite
    {
        public virtual int ID { get; set; }
        public virtual int PodcastID { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual Guid InviteUID { get; set; }
        public virtual DateTime? Expiry { get; set; }

        public virtual Podcast Podcast { get; set; }
    }
}
