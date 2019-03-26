using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Ingest.DTOs
{
    public class BlobUpload
    {
        public DateTime Timestamp { get; set; }
        public string SessionUID { get; set; }
        public IFormFile Data { get; set; }
    }
}
