using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Ingest.DTOs
{
    public class Podcast
    {
        public Guid UID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }

        public IEnumerable<User> Members { get; set; } = new List<User>();
        public IEnumerable<Session> Sessions { get; set; } = new List<Session>();
    }
}
