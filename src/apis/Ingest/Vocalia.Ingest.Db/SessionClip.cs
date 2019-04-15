using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Ingest.Db
{
    public class SessionClip
    {
        public virtual int ID { get; set; }
        public virtual Guid UID { get; set; }
        public virtual int SessionID { get; set; }
        public virtual string UserUID { get; set; }
        public virtual long Size { get; set; }
        public virtual string MediaUrl { get; set; }
        public virtual DateTime Time { get; set; }

        public virtual Session Session { get; set; }
    }
}
