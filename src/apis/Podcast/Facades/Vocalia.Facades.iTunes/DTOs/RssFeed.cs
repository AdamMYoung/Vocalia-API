using Refit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Facades.iTunes.DTOs
{
    internal class RssFeed
    {
        public string Title { get; set; }

        public string Id { get; set; }

        public string Country { get; set; }

        public DateTime Updated { get; set; }

        public IList<RssEntry> Results { get; set; }
    }
}
