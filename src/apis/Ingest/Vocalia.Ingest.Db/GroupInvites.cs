using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Ingest.Db
{
    public class GroupInvites
    {
        public virtual int ID { get; set; }
        public virtual int GroupID { get; set; }
        public virtual Guid InviteUID { get; set; }
        public virtual DateTime? Expiry { get; set; }

        public virtual Group Group { get; set; }
    }
}
