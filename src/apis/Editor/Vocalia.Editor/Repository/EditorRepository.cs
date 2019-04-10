using Microsoft.EntityFrameworkCore;
using ObjectBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vocalia.Editor.Db;
using Vocalia.Editor.Media;
using Vocalia.Editor.Streams;
using Vocalia.ServiceBus.Types;

namespace Vocalia.Editor.Repository
{
    public class EditorRepository : IEditorRepository
    {
        /// <summary>
        /// Handles the storage of media stream to blob storage.
        /// </summary>
        private IMediaStorage MediaStorage { get; }

        /// <summary>
        /// Builds media streams from audio chunks.
        /// </summary>
        private IStreamBuilder StreamBuilder { get; }

        /// <summary>
        /// Database reference.
        /// </summary>
        private EditorContext DbContext { get; }

        /// <summary>
        /// Instantiates a new EditorRepository.
        /// </summary>
        public EditorRepository(IMediaStorage mediaStorage, EditorContext editorDb,
             IStreamBuilder streamBuilder, IObjectBus<RecordingChunk> recordBus,
             IObjectBus<Vocalia.ServiceBus.Types.Podcast> podcastBus)
        {
            DbContext = editorDb;
            MediaStorage = mediaStorage;
            StreamBuilder = streamBuilder;

            //Initializes service bus objects for handling I/O between services.
            _ = recordBus;
            _ = podcastBus;
        }

        /// <summary>
        /// Applies the specified edit to the audio stream attached to the sessionUID and userUID
        /// </summary>
        /// <param name="sessionUid">UID of the session.</param>
        /// <param name="userUid">UID of the user.</param>
        /// <param name="edit">Edit to apply.</param>
        /// <returns></returns>
        public Task AddEditAsync(Guid sessionUid, string userUid, Edit edit)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Builds an audio stream from the recieved stream chunks.
        /// </summary>
        /// <param name="sessionUid">UID of the session.</param>
        /// <param name="userUid">UID of the user.</param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetStreamsAsync(Guid sessionUid, string userUid)
        {
            var session = await DbContext.Sessions
                .Include(x => x.Users).ThenInclude(x => x.Media)
                .Include(x => x.Podcast).ThenInclude(x => x.Members)
                .FirstOrDefaultAsync(x => x.UID == sessionUid);

            if (session.Podcast.Members.Any(x => x.UserUID == userUid && x.IsAdmin))
            {
                var audioStreams = new List<string>();

                foreach (var user in session.Users)
                {
                    var media = user.Media.OrderBy(x => x.Timestamp).Select(c => c.MediaUrl);
                    var stream = await StreamBuilder.ConcatenateUrlMediaAsync(media);
                    var url = await MediaStorage.UploadStreamAsync(userUid, sessionUid, stream);
                    audioStreams.Add(url);
                }

                return audioStreams;
            }

            return null;
        }
    }
}
