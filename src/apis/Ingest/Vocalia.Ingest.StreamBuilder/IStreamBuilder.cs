using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Vocalia.Ingest.Streams
{
    public interface IStreamBuilder
    {
        /// <summary>
        /// Returns a media stream of all urls concantenated with each other.
        /// </summary>
        /// <param name="urls">URLs to concatenate.</param>
        /// <returns></returns>
        Task<MemoryStream> ConcatenateUrlMediaAsync(IEnumerable<string> urls);
    }
}
