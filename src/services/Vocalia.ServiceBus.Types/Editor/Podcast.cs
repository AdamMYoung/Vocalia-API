﻿using System;
using System.Collections.Generic;

namespace Vocalia.ServiceBus.Types.Editor
{
    public class Podcast
    {
        public Guid UID { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public IList<Member> Members { get; set; } = new List<Member>();
    }
}
