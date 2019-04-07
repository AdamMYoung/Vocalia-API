using System.Collections.Generic;

namespace Vocalia.Editor.Db
{
    public class User
    {
        public virtual int ID { get; set; }
        public virtual int SessionID { get; set; }
        public virtual string UserUID { get; set; }
        public virtual string Name { get; set; }

        public virtual IEnumerable<Edit> Edits { get; set; }
        public virtual IEnumerable<UserMedia> Media { get; set; }
        public virtual Session Session { get; set; }
    }
}
