using System;

namespace Vocalia.Editor.DomainModels
{
    public class Media
    {
        public int ID { get; set; }
        public string UserUID { get; set; }
        public string UserName { get; set; }
        public string UserImageUrl { get; set; }
        public DateTime Date { get; set; }
        public string MediaUrl { get; set; }
        public Guid UID { get; set; }

        
    }
}
