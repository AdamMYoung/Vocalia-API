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

        /// <summary>
        /// Uploads a media stream to the database.
        /// </summary>
        /// <param name="userUid">UID of the user.</param>
        /// <param name="sessionUid">UID of the session.</param>
        /// <param name="entryUid">UID of the entry.</param>
        /// <param name="stream">Stream to upload.</param>
        /// <returns></returns>
        Task<string> UploadStreamAsync(string userUid, Guid sessionUid, Guid entryUid, Stream stream);
    }
}
