using Refit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Vocalia.Facades.GPodder.DTOs;

namespace Vocalia.Facades.GPodder
{
    interface GPodderApi
    {
        [Get("/api/2/tags/{count}.json")]
        Task<IEnumerable<CategoryTag>> GetCategoriesAsync(int count);

        [Get("/toplist/{number}.json")]
        Task<IEnumerable<Podcast>> GetTopPodcastsAsync(int number);

        [Get("/api/2/tag/{tag}/{count}.json")]
        Task<IEnumerable<Podcast>> GetPodcastsByTagAsync(string tag, int count);
    }
}
