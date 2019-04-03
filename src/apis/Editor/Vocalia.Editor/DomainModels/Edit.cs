using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Editor.DomainModels
{
    public class Edit
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int EditTypeID { get; set; }
        public int StartTimestamp { get; set; }
        public int EndTimestamp { get; set; }  
    }
}
