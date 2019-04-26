using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.ServiceBus.Types.Podcast
{
    public class Podcast
    {
        public Guid UID { get; set; }
        public string Title { get; set; }
        public string RssUrl { get; set; }
        public string ImageUrl { get; set; }
        public int CategoryID { get; set; }
        public int LanguageID { get; set; }
        public bool IsExplicit { get; set; }
    }
}
