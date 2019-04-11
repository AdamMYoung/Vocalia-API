using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Editor.DTOs
{
    public class Session
    {
        public Guid UID { get; set; }
        public int PodcastID { get; set; }
        public DateTime Date { get; set; }
    }
}
