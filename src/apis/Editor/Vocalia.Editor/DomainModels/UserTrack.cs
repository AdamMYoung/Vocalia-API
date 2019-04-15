using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Editor.DomainModels
{
    public class UserTrack
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string UserUid { get; set; }
        public Guid SessionUid { get; set; }
        public IEnumerable<AudioEntry> Entries { get; set; } = new List<AudioEntry>();
    }
}
