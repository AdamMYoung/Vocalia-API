using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Vocalia.Ingest.DomainModels;

namespace Vocalia.Ingest.Media
{
    public interface IMediaStorage
    {
        /// <summary>
        /// Uploads a media blob to the database.
        /// </summary>
        /// <param name="blob">Blob to upload.</param>
        /// <returns></returns>
        Task<string> UploadBlobAsync(BlobUpload blob);
    }
}
