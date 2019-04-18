using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Ingest.Db
{
    public class Media
    {
        public virtual int ID { get; set; }
        public virtual int ClipID { get; set; }
        public virtual string UserUID { get; set; }
        public virtual string MediaUrl { get; set; }
        public virtual Guid UID { get; set; }

        public virtual Clip Clip { get; set; }
    }
}
