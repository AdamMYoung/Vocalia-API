﻿using System;

namespace Vocalia.ServiceBus.Types
{
    public class Media
    {
        public Guid UID { get; set; }
        public string UserUID { get; set; }
        public string MediaUrl { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
