using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Editor.Db
{
    public class User
    {
        public virtual int ID { get; set; }
        public virtual int SessionID { get; set; }
        public virtual string UserUID { get; set; }
        public virtual string Name { get; set; }

        public virtual IEnumerable<Edit> Edits { get; set; }
        public virtual Session Session { get; set; }
    }
}
