using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Vocalia.Ingest.Db
{
    public class Podcast
    {
        public virtual int ID { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual Guid UID { get; set; }
        public virtual int GroupID { get; set; }
        public virtual string Name { get; set; }

        public virtual Group Group { get; set; }
        public virtual IEnumerable<Session> Sessions { get; set; }
    }
}
