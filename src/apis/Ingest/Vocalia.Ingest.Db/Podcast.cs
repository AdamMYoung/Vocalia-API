using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vocalia.Ingest.Db
{
    public class Podcast
    {
        public virtual int ID { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual Guid UID { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual string ImageUrl { get; set; }

        public virtual IEnumerable<Session> Sessions { get; set; }
        public virtual IEnumerable<PodcastUser> Users { get; set; }
        public virtual IEnumerable<PodcastInvite> Invites { get; set; }
    }
}
