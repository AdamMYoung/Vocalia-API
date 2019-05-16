using Microsoft.AspNetCore.Http;
using System;

namespace Vocalia.Ingest.DTOs
{
    public class BlobUpload
    {
        public string Name { get; set; }
        public Guid SessionUid { get; set; }
        public Guid ClipUid { get; set; }
        public IFormFile Data { get; set; }
    }
}
