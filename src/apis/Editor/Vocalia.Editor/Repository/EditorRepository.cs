using Microsoft.EntityFrameworkCore;
using ObjectBus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Vocalia.Editor.Db;
using Vocalia.Editor.Media;
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
        /// Database reference.
        /// </summary>
        private EditorContext DbContext { get; }

        /// <summary>
        /// Instantiates a new EditorRepository.
        /// </summary>
        public EditorRepository(IObjectBus<RecordingChunk> editorBus, IMediaStorage mediaStorage, 
            EditorContext editorDb, IUserFacade userFacade)
        {
            EditorBus = editorBus;
            MediaStorage = mediaStorage;
            DbContext = editorDb;
            EditorBus.MessageRecieved += OnRecordingChunkRecieved;
        }

        public Task AddEditAsync(Guid sessionUid, string userUid, Edit edit)
        {
            throw new NotImplementedException();
        }

        public Task<FileStream> GetEditStreamAsync(Guid sessionUid, string userUid)
        {
            throw new NotImplementedException();
        }

        private async void OnRecordingChunkRecieved(object sender, MessageEventArgs<RecordingChunk> e)
        {
            var message = e.Object;

            Db.User user = await DbContext.Users.FirstOrDefaultAsync(x => x.UserUID == message.UserUID);
            if (user == null)
            {
                var userId = await CreateUserAsync(message);
                user = await DbContext.Users.FindAsync(userId);
            }

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
        private async Task<int> CreateUserAsync(RecordingChunk recordingChunk)
        {
            var session = await DbContext.Sessions.FirstOrDefaultAsync(x => x.UID == recordingChunk.SessionUID);
            var userRef = await UserFacade.GetUserInfoAsync(recordingChunk.UserUID);
            var user = new Db.User
            {
                Name = userRef.name,
                UserUID = recordingChunk.UserUID,
                SessionID = session.ID
            };

            await DbContext.Users.AddAsync(user);
            await DbContext.SaveChangesAsync();

            return user.ID;
        }
    }
}
