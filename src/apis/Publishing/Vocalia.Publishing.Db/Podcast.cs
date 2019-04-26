using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Publishing.Db
{
    public class Podcast
    {
        public virtual int ID { get; set; }
        public virtual Guid UID { get; set; }
        public virtual int CategoryID { get; set; }
        public virtual int LanguageID { get; set; }
        public virtual string Title { get; set; }
        public virtual string Description { get; set; }
        public virtual string ImageUrl { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual bool IsExplicit { get; set; }

        public virtual IEnumerable<Episode> Episodes { get; set; }
        public virtual IEnumerable<Member> Members { get; set; }
        public virtual Category Category { get; set; }
        public virtual Language Language { get; set; }
    }
}
