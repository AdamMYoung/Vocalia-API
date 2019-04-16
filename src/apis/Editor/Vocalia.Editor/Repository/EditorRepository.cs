using Microsoft.EntityFrameworkCore;
using ObjectBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vocalia.Editor.Db;
using Vocalia.Editor.DomainModels;
using Vocalia.Editor.Media;
using Vocalia.ServiceBus.Types;
using Vocalia.Streams;

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
             IStreamBuilder streamBuilder, IObjectBus<Vocalia.ServiceBus.Types.Clip> recordBus,
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
        public Task AddEditAsync(Guid sessionUid, string userUid, DomainModels.Edit edit)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the current timeline from the database.
        /// </summary>
        /// <param name="sessionUid">UID of the session.</param>
        /// <param name="userUid">UID of the user.</param>
        /// <returns></returns>
        public async Task<IEnumerable<DomainModels.Clip>> GetTimelineAsync(Guid sessionUid, string userUid)
        {
            var session = await DbContext.Sessions
               .Include(x => x.TimelineEntries).ThenInclude(c => c.Clip)
               .ThenInclude(c => c.Media)
               .Include(x => x.Podcast).ThenInclude(x => x.Members)
               .FirstOrDefaultAsync(x => x.UID == sessionUid && x.IsActive);

            if (session.Podcast.Members.Any(x => x.UserUID == userUid && x.IsAdmin))
            {
                var clips = session.TimelineEntries.Select(c => c.Clip).Select(x => new DomainModels.Clip
                {
                    UID = x.UID,
                    ID = x.ID,
                    Date = x.Date,
                    SessionID = x.SessionID,
                    Name = x.Name,
                    Media = x.Media.Select(c => new DomainModels.Media
                    {
                        ID = c.ID,
                        Date = c.Date,
                        UID = c.UID,
                        MediaUrl = c.MediaUrl,
                        UserUID = c.UserUID
                    })
                });

                return clips;
            }

            return null;
        }

        /// <summary>
        /// Gets all clips from the database.
        /// </summary>
        /// <param name="sessionUid">UID of the session.</param>
        /// <param name="userUid">UID of the user.</param>
        /// <returns></returns>
        public async Task<IEnumerable<DomainModels.Clip>> GetAllClipsAsync(Guid sessionUid, string userUid)
        {
            var session = await DbContext.Sessions
               .Include(x => x.Clips).ThenInclude(c => c.Media)
               .Include(x => x.Podcast).ThenInclude(x => x.Members)
               .FirstOrDefaultAsync(x => x.UID == sessionUid && x.IsActive);

            if (session.Podcast.Members.Any(x => x.UserUID == userUid && x.IsAdmin))
            {
                var clips = session.Clips.Select(x => new DomainModels.Clip
                {
                    UID = x.UID,
                    ID = x.ID,
                    Date = x.Date,
                    SessionID = x.SessionID,
                    Name = x.Name,
                    Media = x.Media.Select(c => new DomainModels.Media
                    {
                        ID = c.ID,
                        Date = c.Date,
                        UID = c.UID,
                        MediaUrl = c.MediaUrl,
                        UserUID = c.UserUID
                    })
                });

                return clips;
            }

            return null;
        }

        #region Podcast

        /// <summary>
        /// Returns all podcasts editable by the user.
        /// </summary>
        /// <param name="userUID">User to get podcasts for.</param>
        /// <returns></returns>
        public async Task<IEnumerable<DomainModels.Podcast>> GetPodcastsAsync(string userUID)
        {
            var podcasts = await DbContext.Podcasts
                .Include(x => x.Members)
                .Include(x => x.Sessions)
                .Where(x => x.Members.Any(c => c.UserUID == userUID && c.IsAdmin) && x.Sessions.Any(s => !s.IsFinishedEditing && s.IsActive))
                .ToListAsync();

            if (podcasts == null)
                return null;

            return podcasts.Select(x => new DomainModels.Podcast
            {
                ID = x.ID,
                UID = x.UID,
                Name = x.Name,
                ImageUrl = x.ImageUrl,
                Sessions = x.Sessions.Where(c => c.IsActive && !c.IsFinishedEditing).Select(c => new DomainModels.Session
                {
                    ID = c.ID,
                    UID = c.UID,
                    PodcastID = c.PodcastID,
                    Date = c.Date
                })
            });
        }

        /// <summary>
        /// Gets podcast info about the specified podcastUID if the user is an admin.
        /// </summary>
        /// <param name="userUID">UID of the user.</param>
        /// <param name="podcastUid">UID of the podcast.</param>
        /// <returns></returns>
        public async Task<DomainModels.Podcast> GetPodcastDetailAsync(string userUID, Guid podcastUid)
        {
            var podcast = await DbContext.Podcasts
                .Include(x => x.Members)
                .Include(x => x.Sessions)
                .FirstOrDefaultAsync(x => x.Members.Any(c => c.UserUID == userUID && c.IsAdmin) && x.UID == podcastUid);

            if (podcast == null)
                return null;

            return new DomainModels.Podcast
            {
                ID = podcast.ID,
                UID = podcast.UID,
                Name = podcast.Name,
                ImageUrl = podcast.ImageUrl,
                Sessions = podcast.Sessions.Where(x => x.IsActive).Select(c => new DomainModels.Session
                {
                    ID = c.ID,
                    UID = c.UID,
                    PodcastID = c.PodcastID,
                    Date = c.Date
                })
            };
        }

        #endregion

        #region Session

        /// <summary>
        /// Deletes the specified session from the database.
        /// </summary>
        /// <param name="userUid">UID of the user.</param>
        /// <param name="sessionUid">UID of the session.</param>
        /// <returns></returns>
        public async Task<bool> DeleteSessionAsync(string userUid, Guid sessionUid)
        {
            var session = await DbContext.Sessions.FirstOrDefaultAsync(x => x.UID == sessionUid &&
                x.Podcast.Members.Any(c => c.UserUID == userUid && c.IsAdmin));

            if (session == null)
                return false;

            session.IsActive = false;
            await DbContext.SaveChangesAsync();
            return true;
        }

        #endregion
    }
}
