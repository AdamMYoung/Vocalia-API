using System;
using System.Collections.Generic;

namespace Vocalia.Editor.Db
{
    public class Session
    {
        public virtual int ID { get; set; }
        public virtual Guid UID { get; set; }
        public virtual int PodcastID { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual bool IsFinishedEditing { get; set; }
        public virtual bool IsActive { get; set; } = true;

        public virtual IEnumerable<Clip> Clips { get; set; }
        public virtual IEnumerable<TimelineEntry> TimelineEntries { get; set; }
        public virtual Podcast Podcast { get; set; }
    }
}
