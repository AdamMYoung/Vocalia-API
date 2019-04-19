using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Editor.Db
{
    public class Stream
    {
        public virtual int ID { get; set; }
        public virtual int MediaID { get; set; }
        public virtual string MediaUrl { get; set; }

        public virtual Media Media { get; set; }
    }
}
