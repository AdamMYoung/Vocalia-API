using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Podcast.DomainModels
{
    public class Category
    {
        public int ID { get; set; }

        public int LanguageID { get; set; }

        public int ITunesID { get; set; }

        public int ListenNotesID { get; set; }

        public string GPodderTag { get; set; }

        public string Title { get; set; }

        public string IconUrl { get; set; }
    }
}
