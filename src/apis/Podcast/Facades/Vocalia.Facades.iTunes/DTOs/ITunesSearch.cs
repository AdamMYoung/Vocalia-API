using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Facades.iTunes.DTOs
{
    class ITunesSearch
    {
        public int ResultCount { get; set; }
        
        public IEnumerable<ITunesSearchResult> Results { get; set; }

    }
}
