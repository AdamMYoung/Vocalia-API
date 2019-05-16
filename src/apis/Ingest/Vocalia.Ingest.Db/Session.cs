using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vocalia.Ingest.Db
{
    public class Session
    {
        public virtual int ID { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual Guid UID { get; set; }
        public virtual int PodcastID { get; set; }
        public virtual bool IsFinished { get; set; }
        public virtual DateTime Date { get; set; }

        public virtual Podcast Podcast { get; set; }
        public virtual IEnumerable<Clip> Clips { get; set; }

    }
}
