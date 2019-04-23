using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Publishing.DTOs
{
    public class UnassignedPodcast
    {
        public Guid UID { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
    }
}
