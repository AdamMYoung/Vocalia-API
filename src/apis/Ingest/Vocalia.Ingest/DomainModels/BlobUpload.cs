using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Ingest.DomainModels
{
    public class BlobUpload
    {
        public string UserUID { get; set; }
        public int Timestamp { get; set; }
        public Guid SessionUID { get; set; }
        public IFormFile Data { get; set; }
    }
}
