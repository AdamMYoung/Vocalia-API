using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Podcast.DTOs
{
    public class Category
    {
        public int ID { get; internal set; }
        public string Title { get; internal set; }
        public string IconUrl { get; internal set; }
    }
}
