using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Social.DomainModels
{
    public class Podcast
    {
        public int ID { get; set; }
        public int GroupID { get; set; }
        public string Name { get; set; }
        public string WebsiteUrl { get; set; }
    }
}
