using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vocalia.Ingest.DomainModels;

namespace Vocalia.Ingest.MediaService
{
    public interface IMediaStorageService
    {
        /// <summary>
        /// Uploads a media blob to the database.
        /// </summary>
        /// <param name="blob">Blob to upload.</param>
        /// <returns></returns>
        Task<string> UploadMediaAsync(BlobUpload blob);
    }
}
