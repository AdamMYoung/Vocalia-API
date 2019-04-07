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
using Vocalia.Ingest.Streams;
using Vocalia.ServiceBus.Types;
using Vocalia.UserFacade;

namespace Vocalia.Editor.Repository
{
    public class EditorRepository : IEditorRepository
    {
        /// <summary>
        /// ObjectBus to handle editor data.
        /// </summary>
        private IObjectBus<RecordingChunk> EditorBus { get; }

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
        public EditorRepository(IObjectBus<RecordingChunk> editorBus, IMediaStorage mediaStorage, 
            EditorContext editorDb, IUserFacade userFacade, IStreamBuilder streamBuilder)
        {
            EditorBus = editorBus;
            MediaStorage = mediaStorage;
            DbContext = editorDb;
            UserFacade = userFacade;
            StreamBuilder = streamBuilder;
            EditorBus.MessageRecieved += OnRecordingChunkRecieved;
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
        /// Gets an audio stream with the current edits applied.
        /// </summary>
        /// <param name="sessionUid">UID of the session.</param>
        /// <param name="userUid">UID of the user.</param>
        /// <returns></returns>
        public async Task<string> GetEditStreamAsync(Guid sessionUid, string userUid)
        {
            var audioStream = await GetAudioStreamAsync(sessionUid, userUid);
            //Apply editing adjustments.
            return audioStream;
        }

        /// <summary>
        /// Builds an audio stream from the recieved stream chunks.
        /// </summary>
        /// <param name="sessionUid">UID of the session.</param>
        /// <param name="userUid">UID of the user.</param>
        /// <returns></returns>
        private async Task<string> GetAudioStreamAsync(Guid sessionUid, string userUid)
        {
            var user = await DbContext.Users.SingleOrDefaultAsync(x => x.UserUID == userUid && x.Session.UID == sessionUid);
            var media = user.Media.OrderBy(x => x.Timestamp).Select(c => c.MediaUrl);

            var stream = await StreamBuilder.ConcatenateUrlMediaAsync(media);
            return await MediaStorage.UploadStreamAsync(userUid, sessionUid, stream);
        }

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
            await DbContext.UserMedia.AddAsync(new UserMedia
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

            await DbContext.Users.AddAsync(user);
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
            var podcastRef = DbContext.Podcasts.FirstOrDefaultAsync(x => x.UID == recordingChunk.PodcastUID);
            var session = new Session()
            {
                IsFinishedEditing = false,
                PodcastID = podcastRef.Id,
                UID = recordingChunk.SessionUID
            };

            await DbContext.Sessions.AddAsync(session);
            await DbContext.SaveChangesAsync();

            return session.ID;
        }

        #endregion
    }
}
