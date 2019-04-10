using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Editor.DomainModels
{
    public class EditStream
    {
        public int ID { get; set; }
        public Guid SessionUID { get; set; }
        public string UserUID { get; set; }
        public string UserName { get; set; }
        public string MediaUrl { get; set; }
    }
}
