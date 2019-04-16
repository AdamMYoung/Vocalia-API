using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Editor.DomainModels
{
    public class Media
    {
        public int ID { get; set; }
        public string UserUID { get; set; }
        public DateTime Date { get; set; }
        public string MediaUrl { get; set; }
        public Guid UID { get; set; }
    }
}
