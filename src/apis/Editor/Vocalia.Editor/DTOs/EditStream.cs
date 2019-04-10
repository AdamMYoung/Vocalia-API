using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Editor.DTOs
{
    public class EditStream
    {
        public Guid SessionUID { get; set; }
        public string UserUID { get; set; }
        public string UserName { get; set; }
        public string MediaUrl { get; set; }
    }
}
