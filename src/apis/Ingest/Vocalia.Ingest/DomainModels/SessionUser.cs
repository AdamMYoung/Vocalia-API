using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Ingest.DomainModels
{
    public class SessionUser
    {
        public int ID { get; set; }
        public int SessionID { get; set; }
        public string UserUID { get; set; }
    }
}
