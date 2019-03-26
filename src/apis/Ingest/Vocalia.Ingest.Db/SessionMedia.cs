using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Ingest.Db
{
    public class SessionMedia
    {
        public virtual int ID { get; set; }
        public virtual int SessionID { get; set; }
        public virtual string UserUID { get; set; }
        public virtual int Timestamp { get; set; }
        public virtual string MediaUrl { get; set; }

        public virtual Session Session { get; set; }
    }
}
