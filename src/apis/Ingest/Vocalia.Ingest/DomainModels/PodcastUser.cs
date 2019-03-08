using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Ingest.DomainModels
{
    public class PodcastUser
    {
        public int ID { get; set; }
        public int PodcastID { get; set; }
        public string UserUID { get; set; }
        public bool IsAdmin { get; set; }
    }
}
