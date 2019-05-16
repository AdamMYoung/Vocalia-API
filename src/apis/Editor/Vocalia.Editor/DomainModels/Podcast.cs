using System;
using System.Collections.Generic;

namespace Vocalia.Editor.DomainModels
{
    public class Podcast
    {
        public int ID { get; set; }
        public Guid UID { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }

        public IEnumerable<Session> Sessions { get; set; } = new List<Session>();
    }
}
