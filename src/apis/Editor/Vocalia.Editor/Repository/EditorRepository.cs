using Microsoft.EntityFrameworkCore;
using ObjectBus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Vocalia.Editor.Db;
using Vocalia.Editor.Media;
using Vocalia.Editor.Streams;
using Vocalia.ServiceBus.Types;
using Vocalia.UserFacade;

namespace Vocalia.Editor.Repository
{
    public class EditorRepository : IEditorRepository
    {
        /// <summary>
        /// ObjectBus to handle recording chunk data.
        /// </summary>
        private IObjectBus<RecordingChunk> RecordingChunkBus { get; }

        /// <summary>
        /// ObjectBus to handle podcast data.
        /// </summary>
        private IObjectBus<ServiceBus.Types.Podcast> PodcastBus { get; }

        /// <summary>
        /// Handles the storage of media stream to blob storage.
        /// </summary>
        private IMediaStorage MediaStorage { get; }

        /// <summary>
        /// Facade for handling Auth0 user info.
        /// </summary>
        private IUserFacade UserFacade { get; }

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
        public EditorRepository(IObjectBus<RecordingChunk> chunkBus, IObjectBus<ServiceBus.Types.Podcast> podcastBus,
            IMediaStorage mediaStorage, EditorContext editorDb, IUserFacade userFacade, IStreamBuilder streamBuilder)
        {
            RecordingChunkBus = chunkBus;
            PodcastBus = podcastBus;
            MediaStorage = mediaStorage;
            DbContext = editorDb;
            UserFacade = userFacade;
            StreamBuilder = streamBuilder;

            RecordingChunkBus.MessageRecieved += OnRecordingChunkRecieved;
            PodcastBus.MessageRecieved += OnPodcastRecieved;
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

        #region Podcast

        /// <summary>
        /// Called when a new podcast has been recieved.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnPodcastRecieved(object sender, MessageEventArgs<ServiceBus.Types.Podcast> e)
        {
            var podcast = e.Object;

            if (!await DbContext.Podcasts.AnyAsync(x => x.UID == podcast.UID))
            {
                DbContext.Podcasts.Add(new Db.Podcast
                {
                    Name = podcast.Name,
                    ImageUrl = podcast.ImageUrl,
                    UID = podcast.UID
                });

                await DbContext.SaveChangesAsync();
            }
        }

        #endregion

        #region Service Bus

        /// <summary>
        /// Called when a new recording chunk has been recieved.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnRecordingChunkRecieved(object sender, MessageEventArgs<RecordingChunk> e)
        {
            var message = e.Object;

            //Gets the specified session, or creates it if not available.
            var session = await DbContext.Sessions.FirstOrDefaultAsync(x => x.UID == message.SessionUID);
            if (session == null)
            {
                var sessionId = await CreateSessionAsync(message);
                session = await DbContext.Sessions.FindAsync(sessionId);
            }

            //Gets the specificed user, or creates it if not available.
            Db.User user = await DbContext.Users.FirstOrDefaultAsync(x => x.UserUID == message.UserUID && x.Session.UID == message.SessionUID);
            if (user == null)
            {
                var userId = await CreateUserAsync(message, session.ID);
                user = await DbContext.Users.FindAsync(userId);
            }

            //Adds the recieved media to the user entry.
            DbContext.UserMedia.Add(new UserMedia
            {
                UserID = user.ID,
                MediaUrl = message.MediaUrl,
                Timestamp = message.Timestamp
            });

            await DbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Creates a user in the database and returns the reference.
        /// </summary>
        /// <param name="recordingChunk">Recording chunk recieved.</param>
        /// <returns></returns>
        private async Task<int> CreateUserAsync(RecordingChunk recordingChunk, int sessionId)
        {
            var userRef = await UserFacade.GetUserInfoAsync(recordingChunk.UserUID);
            var user = new Db.User
            {
                Name = userRef.name,
                UserUID = recordingChunk.UserUID,
                SessionID = sessionId
            };

            DbContext.Users.Add(user);
            await DbContext.SaveChangesAsync();

            return user.ID;
        }

        /// <summary>
        /// Creates a session in the database and returns the reference.
        /// </summary>
        /// <param name="recordingChunk">Recording chunk recieved.</param>
        /// <returns></returns>
        private async Task<int> CreateSessionAsync(RecordingChunk recordingChunk)
        {
            var podcastRef = await DbContext.Podcasts.FirstOrDefaultAsync(x => x.UID == recordingChunk.PodcastUID);
            var session = new Session()
            {
                IsFinishedEditing = false,
                PodcastID = podcastRef.ID,
                UID = recordingChunk.SessionUID
            };

            DbContext.Sessions.Add(session);
            await DbContext.SaveChangesAsync();

            return session.ID;
        }

        #endregion
    }
}
