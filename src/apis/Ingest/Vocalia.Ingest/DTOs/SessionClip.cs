﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Ingest.DTOs
{
    public class SessionClip
    {
        public Guid UID { get; set; }
        public string UserUID { get; set; }
        public long Size { get; set; }
        public string MediaUrl { get; set; }
        public DateTime Time { get; set; }
        public string Name { get; set; }
    }
}
