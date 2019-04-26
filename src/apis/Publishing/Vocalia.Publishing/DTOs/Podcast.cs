using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Publishing.DTOs
{
    public class Podcast
    {
        public Guid UID { get; set; }
        public int CategoryID { get; set; }
        public int LanguageID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string RssUrl { get; set; }
        public bool IsExplicit { get; set; }
        public bool IsActive { get; set; }

        public IEnumerable<Episode> Episodes { get; set; } = new List<Episode>();
    }
}
