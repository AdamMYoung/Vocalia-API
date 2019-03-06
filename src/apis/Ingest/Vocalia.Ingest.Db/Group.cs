using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Vocalia.Ingest.Db
{
    public class Group
    {
        public virtual int ID { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual Guid UID { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }

        public virtual IEnumerable<UserGroup> UserGroups { get; set; }
        public virtual IEnumerable<GroupInvites> Invites { get; set; }
        public virtual IEnumerable<Podcast> Podcasts { get; set; }
    }
}
