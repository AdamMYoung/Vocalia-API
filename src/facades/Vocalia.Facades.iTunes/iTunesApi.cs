using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Vocalia.Facades.iTunes
{
    interface iTunesApi
    {
        Task<IEnumerable<DTOs.Podcast>> GetPodcastsAsync(int count);


    }
}
