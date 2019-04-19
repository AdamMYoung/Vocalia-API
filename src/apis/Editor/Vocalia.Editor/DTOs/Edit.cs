using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Editor.DTOs
{
    public class Edit
    {
        public Guid ClipUID { get; set; }
        public int StartTrim { get; set; }
        public int EndTrim { get; set; }
    }
}
