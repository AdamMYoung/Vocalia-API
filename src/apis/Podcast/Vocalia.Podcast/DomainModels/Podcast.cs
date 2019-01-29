using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Podcast.DomainModels
{
    public class Podcast
    {
        public int ID { get; set; }

        public int LanguageID { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string RssUrl { get; set; }

        public string ImageUrl { get; set; }

        public bool IsActive { get; set; }

        public bool IsExplicit { get; set; }
    }
}
