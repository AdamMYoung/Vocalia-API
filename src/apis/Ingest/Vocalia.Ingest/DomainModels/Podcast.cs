using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Ingest.DomainModels
{
    public class Podcast
    {
        public int ID { get; set; }
        public Guid UID { get; set; }
        public int GroupID { get; set; }
        public string Name { get; set; }
    }
}
