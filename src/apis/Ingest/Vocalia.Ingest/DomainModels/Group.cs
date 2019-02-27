using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Ingest.DomainModels
{
    public class Group
    {
        public int ID { get; set; }
        public Guid UID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
