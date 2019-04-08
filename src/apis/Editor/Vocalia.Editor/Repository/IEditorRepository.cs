using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Vocalia.Editor.Db;

namespace Vocalia.Editor.Repository
{
    public interface IEditorRepository
    {
        /// <summary>
        /// Applies the specified edit to the audio stream attached to the sessionUID and userUID
        /// </summary>
        /// <param name="sessionUid">UID of the session.</param>
        /// <param name="userUid">UID of the user.</param>
        /// <param name="edit">Edit to apply.</param>
        /// <returns></returns>
        Task AddEditAsync(Guid sessionUid, string userUid, Edit edit);

        /// <summary>
        /// Gets an audio stream with the current edits applied.
        /// </summary>
        /// <param name="sessionUid">UID of the session.</param>
        /// <param name="userUid">UID of the user.</param>
        /// <returns></returns>
        Task<IEnumerable<string>> GetStreamsAsync(Guid sessionUid, string userUid);
    }
}
