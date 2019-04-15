using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Vocalia.Streams
{
    public class StreamBuilder : IStreamBuilder
    {
        /// <summary>
        /// Streams already loaded into memory.
        /// </summary>
        private static IDictionary<string, byte[]> CachedStreams { get; } = new Dictionary<string, byte[]>();

        /// <summary>
        /// Returns a media stream of all urls concantenated with each other.
        /// </summary>
        /// <param name="urls">URLs to concatenate.</param>
        /// <returns></returns>
        public async Task<MemoryStream> ConcatenateUrlMediaAsync(IEnumerable<string> urls)
        {
            return await Task.Run(() =>
            {
                //Can't utilize async/await due to the order of queries being crucial to stream generation.
                //Instead the process is run on a background thread to prevent interrupts.
                using (var client = new WebClient())
                {
                    var ms = new MemoryStream();
                    foreach (var url in urls)
                    {
                        if (!CachedStreams.TryGetValue(url, out byte[] bytes))
                        {
                            bytes = client.DownloadData(new Uri(url));                 
                            CachedStreams.Add(url, bytes);
                        }
                        
                        ms.Write(bytes, 0, bytes.Length);             
                    }
                    return ms;
                };
            });
        }
    }
}
