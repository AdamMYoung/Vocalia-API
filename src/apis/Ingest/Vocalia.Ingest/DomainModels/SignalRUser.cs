using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Ingest.DomainModels
{
    public class SignalRUser
    {
        public string ConnectionId { get; set; }
        public string CurrentGroupId { get; set; }
        public string UserTag { get; set; }
    }
}
