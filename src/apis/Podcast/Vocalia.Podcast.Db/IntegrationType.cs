using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Podcast.Db
{
    public class IntegrationType
    {
        public virtual int ID { get; set; }
        public virtual string Name { get; set; }
        public virtual string LogoUrl { get; set; }

        public virtual IEnumerable<PodcastIntegration> Integrations { get; set; }
    }
}
