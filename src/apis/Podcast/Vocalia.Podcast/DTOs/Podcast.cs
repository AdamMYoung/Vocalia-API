using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Podcast.DTOs
{
    public class Podcast
    {
        public int ID { get; set; }

        public string Title { get; set; }

        public string RssUrl { get; set; }

        public string ImageUrl { get; set; }
    }
}
