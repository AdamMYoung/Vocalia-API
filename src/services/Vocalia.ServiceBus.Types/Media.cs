using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.ServiceBus.Types
{
    public class Media
    {
        public Guid UID { get; set; }
        public string UserUID { get; set; }
        public DateTime Date { get; set; }
        public string MediaUrl { get; set; }
    }
}
