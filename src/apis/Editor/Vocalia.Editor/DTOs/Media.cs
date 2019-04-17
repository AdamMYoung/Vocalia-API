using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Editor.DTOs
{
    public class Media
    {
        public string UserUID { get; set; }
        public string UserName { get; set; }
        public string UserImageUrl { get; set; }
        public DateTime Date { get; set; }
        public string MediaUrl { get; set; }
        public Guid UID { get; set; }
    }
}
