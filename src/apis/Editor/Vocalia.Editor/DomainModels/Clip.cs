using System;
using System.Collections.Generic;

namespace Vocalia.Editor.DomainModels
{
    public class Clip
    {
        public int ID { get; set; }
        public Guid UID { get; set; }
        public Guid SessionUID { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }

        public IEnumerable<Media> Media { get; set; } = new List<Media>();
        public Edit Edit { get; set; }
    }
}
