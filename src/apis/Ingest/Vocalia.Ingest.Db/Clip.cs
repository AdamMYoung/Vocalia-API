using System;
using System.Collections.Generic;

namespace Vocalia.Ingest.Db
{
    public class Clip
    {
        public virtual int ID { get; set; }
        public virtual Guid UID { get; set; }
        public virtual int SessionID { get; set; }
        public virtual string MediaUrl { get; set; }
        public virtual DateTime Time { get; set; }
        public virtual string Name { get; set; }

        public virtual Session Session { get; set; }
        public virtual IEnumerable<Media> Media { get; set; }
    }
}
