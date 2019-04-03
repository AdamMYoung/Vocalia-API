using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Editor.Db
{
    public class Podcast
    {
        public virtual int ID { get; set; }
        public virtual Guid UID { get; set; }
        public virtual string Name { get; set; }
        public virtual string ImageUrl { get; set; }

        public virtual IEnumerable<Session> Sessions { get; set; }
        public virtual IEnumerable<Member> Members { get; set; }
    }
}
