using System.IO;
using System.Threading.Tasks;

namespace Vocalia.Streams
{
    public interface IStreamBuilder
    {
        /// <summary>
        /// Returns a media stream of all urls concantenated with each other.
        /// </summary>
        /// <param name="urls">URLs to concatenate.</param>
        /// <returns></returns>
        Task<MemoryStream> GetStreamFromUrlAsync(string url);
    }
}
