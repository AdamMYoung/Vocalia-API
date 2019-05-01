using System.Collections.Generic;

namespace Vocalia.Facades.iTunes.DTOs
{
    class ITunesSearch
    {
        public int ResultCount { get; set; }
        
        public IEnumerable<ITunesSearchResult> Results { get; set; }

    }
}
