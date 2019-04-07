using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Editor.Media
{
    public interface IMediaStorage
    {
        /// <summary>
        /// Uploads a media stream to the database.
        /// </summary>
        /// <param name="userUid">UID of the user.</param>
        /// <param name="sessionUid">UID of the session.</param>
        /// <param name="stream">Stream to upload.</param>
        /// <returns></returns>
        Task<string> UploadStreamAsync(string userUid, Guid sessionUid, Stream stream);
    }
}
