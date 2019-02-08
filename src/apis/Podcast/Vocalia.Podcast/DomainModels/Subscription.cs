using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Podcast.DomainModels
{
    public class Subscription
    {
        public int ID { get; set; }
        public string GUID { get; set; }
        public string UserUID { get; set; }
        public string RssUrl { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
    }
}
