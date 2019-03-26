using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Ingest.DTOs
{
    public class BlobUpload
    {
        public string Timestamp { get; set; }
        public Guid SessionUID { get; set; }
        public IFormFile Data { get; set; }
    }
}
