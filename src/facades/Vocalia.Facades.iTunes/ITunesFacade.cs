using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vocalia.Facades.iTunes;

namespace Vocalia.Podcast.Facades.iTunes
{
    public class ITunesFacade : IITunesFacade
    {
        private iTunesApi Service { get; set; }

        public ITunesFacade()
        {
            Service = RestService.For<iTunesApi>("https://itunes.apple.com/search");
        }


    }
}
