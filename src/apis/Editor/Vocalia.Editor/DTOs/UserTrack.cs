using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Editor.DTOs
{
    public class UserTrack
    {
        public string UserName { get; set; }
        public string UserUid { get; set; }
        public Guid SessionUid { get; set; }
        public IEnumerable<AudioEntry> Entries { get; set; } = new List<AudioEntry>();
    }
}
