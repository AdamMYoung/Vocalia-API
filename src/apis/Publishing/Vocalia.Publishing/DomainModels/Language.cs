using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Publishing.DomainModels
{
    public class Language
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string ISOCode { get; set; }
    }
}
