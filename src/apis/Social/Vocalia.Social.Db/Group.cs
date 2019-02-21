using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Social.Db
{
    public class Group
    {
        public virtual int ID { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual string WebsiteUrl { get; set; }

        public virtual IEnumerable<UserGroup> UserGroups { get; set; }
        public virtual IEnumerable<Podcast> Podcasts { get; set; }
    }
}
