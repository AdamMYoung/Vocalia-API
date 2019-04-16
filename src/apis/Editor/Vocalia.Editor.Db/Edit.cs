using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Editor.Db
{
    public class Edit
    {
        public virtual int ID { get; set; }
        public virtual int UserID { get; set; }
        public virtual int EditTypeID { get; set; }
        public virtual int StartTimestamp { get; set; }
        public virtual int EndTimestamp { get; set; }

        public virtual EditType EditType { get; set; }
        public virtual Clip User { get; set; }
       
    }
}
