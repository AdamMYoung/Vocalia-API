using System;

namespace Vocalia.Ingest.DTOs
{
    public class PodcastUpload
    {
        public Guid UID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageData { get; set; }
        public string FileType { get; set; }
    }
}
