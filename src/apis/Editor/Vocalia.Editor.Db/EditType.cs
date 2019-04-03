using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Editor.Db
{
    public class EditType
    {
        public virtual int ID { get; set; }
        public virtual string Name { get; set; }

        public virtual IEnumerable<Edit> Edits { get; set; }
    }
}
