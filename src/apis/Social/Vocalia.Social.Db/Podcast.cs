using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Social.Db
{
    public class Podcast
    {
        public virtual int ID { get; set; }
        public virtual int GroupID { get; set; }
        public virtual string Name { get; set; }
        public virtual string WebsiteUrl { get; set; }

        public virtual Group Group { get; set; }
    }
}
