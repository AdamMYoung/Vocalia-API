using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Podcast.Db
{
    public class PodcastIntegration
    {
        public virtual int ID { get; set; }
        public virtual int PodcastID { get; set; }
        public virtual int IntegrationTypeID { get; set; }
        public virtual string Url { get; set; }

        public virtual Podcast Podcast { get; set; }
        public virtual IntegrationType Type { get; set; }
    }
}
