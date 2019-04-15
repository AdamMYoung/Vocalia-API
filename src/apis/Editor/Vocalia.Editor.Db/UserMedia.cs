using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Editor.Db
{
    public class UserMedia
    {
        public virtual int ID { get; set; }
        public virtual Guid UID { get; set; }
        public virtual int UserID { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual string MediaUrl { get; set; }

        public virtual User User { get; set; }
    }
}
