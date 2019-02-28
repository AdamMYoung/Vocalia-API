using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Ingest.Db
{
    public class UserGroup
    {
        public virtual int ID { get; set; }
        public virtual int GroupID { get; set; }
        public virtual string UserUID { get; set; }

        public virtual Group Group { get; set; }
    }
}
