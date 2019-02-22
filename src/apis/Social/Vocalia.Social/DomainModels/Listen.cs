using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Social.DomainModels
{
    public class Listen
    {
        public int ID { get; set; }
        public string UserUID { get; set; }
        public string UserName { get; set; }
        public string RssUrl { get; set; }
        public string EpisodeUrl { get; set; }
        public string EpisodeName { get; set; }
        public DateTime Date { get; set; }
        public bool IsCompleted { get; set; }
    }
}
