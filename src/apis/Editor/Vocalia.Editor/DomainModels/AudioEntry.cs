using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Editor.DomainModels
{
    public class AudioEntry
    {
        public int ID { get; set; }
        public Guid UID { get; set; }
        public string Url { get; set; }
    }
}
