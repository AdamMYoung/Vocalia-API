﻿using System;
using System.IO;
using System.Threading.Tasks;

namespace Vocalia.Ingest.Media
{
    public interface IMediaStorage
    {
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
