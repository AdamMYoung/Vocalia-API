using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Vocalia.Ingest.DomainModels;

namespace Vocalia.Ingest.MediaService
{
    public class MediaStorageService : IMediaStorageService
    {
        private IConfiguration Config { get; }

        public MediaStorageService(IConfiguration config)
        {
            Config = config;
        }

        /// <summary>
        /// Uploads a media blob to the database.
        /// </summary>
        /// <param name="blob">Blob to upload.</param>
        /// <returns></returns>
        public async Task<string> UploadMediaAsync(BlobUpload blob)
        {
            var fileName = Guid.NewGuid().ToString();
            var url = string.Concat(Config["BlobStorage:MediaURL"], blob.SessionUID,"/",
                blob.UserUID, "/", fileName);

            var creds = new StorageCredentials(Config["BlobStorage:Account"], Config["BlobStorage:Key"]);
            var newBlob = new CloudBlockBlob(new Uri(url), creds);

            using (var stream = blob.Data.OpenReadStream())
            {
                await newBlob.UploadFromStreamAsync(stream);
            }

            return url;
        }
    }
}
