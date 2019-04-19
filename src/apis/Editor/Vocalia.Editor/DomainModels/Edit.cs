using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Editor.DomainModels
{
    public class Edit
    {
        public int ID { get; set; }
        public Guid ClipUID { get; set; }
        public int StartTrim { get; set; }
        public int EndTrim { get; set; }  
    }
}
