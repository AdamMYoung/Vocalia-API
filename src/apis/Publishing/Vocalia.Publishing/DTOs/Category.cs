using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Publishing.DTOs
{
    public class Category
    {
        public int ID { get; set; }
        public int ITunesID { get; set; }
        public string GPodderTag { get; set; }
        public string Title { get; set; }
    }
}
