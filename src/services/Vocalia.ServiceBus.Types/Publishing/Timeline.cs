using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.ServiceBus.Types
{
    public class Timeline
    {
        public Guid UID { get; set; }
        public string Name { get; set; }
        public IList<string> TimelineEntries { get; set; } = new List<string>();
    }
}
