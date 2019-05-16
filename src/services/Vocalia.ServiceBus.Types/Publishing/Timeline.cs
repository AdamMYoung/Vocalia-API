using System;
using System.Collections.Generic;

namespace Vocalia.ServiceBus.Types.Publishing
{
    public class Timeline
    {
        public Guid UID { get; set; }
        public DateTime Date { get; set; }
        public Guid PodcastUID { get; set; }
        public IList<string> TimelineEntries { get; set; } = new List<string>();
    }
}
