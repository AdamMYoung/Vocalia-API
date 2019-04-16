using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Editor.Db
{
    public class TimelineEntry
    {
        public virtual int ID { get; set; }
        public virtual int SessionID { get; set; }
        public virtual int ClipID { get; set; }
        public virtual int Position { get; set; }

        public virtual Clip Clip { get; set; }
        public virtual Session Session { get; set; }
    }
}
