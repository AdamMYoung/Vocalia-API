using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Editor.DTOs
{
    public class Clip
    {
        public Guid UID { get; set; }
        public int SessionID { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }

        public IEnumerable<Media> Media { get; set; } = new List<Media>();
    }
}
