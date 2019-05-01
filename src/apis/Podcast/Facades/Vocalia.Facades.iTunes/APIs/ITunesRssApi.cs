using Refit;
using System.Threading.Tasks;

namespace Vocalia.Facades.iTunes
{
    interface ITunesRssApi
    {
        /// <summary>
        /// Returns the top podcasts from the iTunes podcast database, filtering explicit content.
        /// </summary>
        /// <param name="count">Number of items to return.</param>
        /// <param name="isoCountryCode">Country code to search for. Default is "gb".</param>
        [Get("/{isoCountryCode}/rss/topaudiopodcasts/genre={genreId}/limit={count}/explicit=true/json")]
        Task<DTOs.EntryPoint> GetTopPodcasts(int count, string isoCountryCode = "gb", bool isExplicit = true, int genreId = -1);
    }
}
