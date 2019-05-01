using System;

namespace Vocalia.Editor.Db
{
    public class Media
    {
        public virtual int ID { get; set; }
        public virtual int ClipID { get; set; }
        public virtual string UserUID { get; set; }
        public virtual Guid UID { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual string MediaUrl { get; set; }

        public virtual Clip Clip { get; set; }
        public virtual Stream Stream { get; set; }
        
    }
}
