using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Publishing.Media
{
    public class MediaStorage : IMediaStorage
    {
        private IConfiguration Config { get; }

        public MediaStorage(IConfiguration config)
        {
            Config = config;
        }

        /// <summary>
        /// Uploads a media stream to the database.
        /// </summary>
        /// <param name="userUid">UID of the user.</param>
        /// <param name="sessionUid">UID of the session.</param>
        /// <param name="entryUid">UID of the entry.</param>
        /// <param name="stream">Stream to upload.</param>
        /// <returns></returns>
        public async Task<string> UploadStreamAsync(string userUid, Guid sessionUid, Guid entryUid, Stream stream)
        {
            var url = string.Concat(Config["BlobStorage:RssUrl"], sessionUid, "/",
                userUid, "/", entryUid, ".wav");

            var creds = new StorageCredentials(Config["BlobStorage:Account"], Config["BlobStorage:Key"]);
            var newBlob = new CloudBlockBlob(new Uri(url), creds);

            stream.Position = 0;
            await newBlob.UploadFromStreamAsync(stream);

            return url;
        }
    }
}
