using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Publishing.DomainModels
{
    public class UnassignedPodcast
    {
        public virtual int ID { get; set; }
        public virtual Guid UID { get; set; }
        public virtual string Name { get; set; }
        public virtual string ImageUrl { get; set; }
        public virtual bool IsCompleted { get; set; }
    }
}
