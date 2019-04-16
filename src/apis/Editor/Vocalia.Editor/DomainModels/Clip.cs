﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Editor.DomainModels
{
    public class Clip
    {
        public int ID { get; set; }
        public Guid UID { get; set; }
        public int SessionID { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }

        public IEnumerable<Media> Media { get; set; } = new List<Media>();
    }
}
