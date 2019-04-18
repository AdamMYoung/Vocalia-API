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
using Vocalia.UserFacade;

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
        /// Facade for user access.
        /// </summary>
        private IUserFacade UserFacade { get; }

        /// <summary>
        /// Database reference.
        /// </summary>
        private EditorContext DbContext { get; }

        /// <summary>
        /// Instantiates a new EditorRepository.
        /// </summary>
        public EditorRepository(IMediaStorage mediaStorage, EditorContext editorDb,
             IStreamBuilder streamBuilder, IObjectBus<Vocalia.ServiceBus.Types.Clip> recordBus,
             IObjectBus<Vocalia.ServiceBus.Types.Podcast> podcastBus, IUserFacade userFacade)
        {
            DbContext = editorDb;
            MediaStorage = mediaStorage;
            StreamBuilder = streamBuilder;
            UserFacade = userFacade;

            //Initializes service bus objects for handling I/O between services.
            _ = recordBus;
            _ = podcastBus;
        }

        /// <summary>
        /// Applies the specified edit to the audio stream attached to the sessionUID and userUID.
        /// </summary>
        /// <param name="sessionUid">UID of the session.</param>
        /// <param name="userUid">UID of the user.</param>
        /// <param name="edit">Edit to apply.</param>
        /// <returns></returns>
        public async Task<bool> AddEditAsync(string userUid, DomainModels.Edit edit)
        {
            var clip = await DbContext.Clips.Include(x => x.Edit).FirstOrDefaultAsync(c => c.UID == edit.ClipUID && c.Session.Podcast.Members.Any(x => x.UserUID == userUid & x.IsAdmin));

            if (clip == null)
                return false;

            if (clip.Edit != null)
                DbContext.Edits.Remove(clip.Edit);

            DbContext.Edits.Add(new Db.Edit
            {
                ClipID = clip.ID,
                StartTrim = edit.StartTrim,
                EndTrim = edit.EndTrim,
                Gain = edit.Gain
            });

            await DbContext.SaveChangesAsync();
            return true;
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
               .Include(x => x.TimelineEntries).ThenInclude(c => c.Clip).ThenInclude(c => c.Edit)
               .Include(x => x.TimelineEntries).ThenInclude(c => c.Clip).ThenInclude(c => c.Media)
               .Include(x => x.Podcast).ThenInclude(x => x.Members)
               .FirstOrDefaultAsync(x => x.UID == sessionUid && x.IsActive);

            if (session.Podcast.Members.Any(x => x.UserUID == userUid && x.IsAdmin))
            {
                var uids = session.TimelineEntries.Select(c => c.Clip.Media.Select(x => x.UserUID))
                    .SelectMany(c => c)
                    .Distinct();

                var userInfo = new List<User>();
                foreach (var uid in uids)
                    userInfo.Add(await UserFacade.GetUserInfoAsync(uid));

                var clips = session.TimelineEntries.Select(c => c.Clip).Select(x => new DomainModels.Clip
                {
                    UID = x.UID,
                    ID = x.ID,
                    Date = x.Date,
                    SessionUID = x.Session.UID,
                    Name = x.Name,
                    Media = x.Media.Select(c => new DomainModels.Media
                    {
                        ID = c.ID,
                        Date = c.Date,
                        UID = c.UID,
                        MediaUrl = c.MediaUrl,
                        UserUID = c.UserUID,
                        UserImageUrl = userInfo.FirstOrDefault(a => a.user_id == c.UserUID).picture,
                        UserName = userInfo.FirstOrDefault(a => a.user_id == c.UserUID).name
                    }),
                    Edit = x.Edit != null ? new DomainModels.Edit
                    {
                        ID = x.Edit.ID,
                        StartTrim = x.Edit.StartTrim,
                        EndTrim = x.Edit.EndTrim,
                        Gain = x.Edit.Gain,
                        ClipUID = x.UID
                    } : null
                });

                return clips.OrderBy(c => c.Date);
            }

            return null;
        }

        /// <summary>
        /// Sets the timeline to the provided clips.
        /// </summary>
        /// <param name="sessionUid">UID of the session.</param>
        /// <param name="userUid">UID of the user.</param>
        /// <param name="clips">Clips to set as the timeline.</param>
        /// <returns></returns>
        public async Task<bool> SetTimelineAsync(Guid sessionUid, string userUid, IEnumerable<DomainModels.Clip> clips)
        {
            var session = await DbContext.Sessions.Include(c => c.TimelineEntries)
                .FirstOrDefaultAsync(c => c.UID == sessionUid && c.Podcast.Members.Any(x => x.UserUID == userUid && x.IsAdmin));

            if (session == null)
                return false;

            var databaseClips = await DbContext.Clips.Where(c => clips.Any(a => a.UID == c.UID)).ToListAsync();

            DbContext.TimelineEntries.RemoveRange(session.TimelineEntries);
            foreach(var clip in databaseClips)
            {
                DbContext.TimelineEntries.Add(new TimelineEntry
                {
                    ClipID = clip.ID,
                    SessionID = session.ID,
                    Position = databaseClips.IndexOf(clip)
                });
            }

            await DbContext.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Gets all clips from the database.
        /// </summary>
        /// <param name="sessionUid">UID of the session.</param>
        /// <param name="userUid">UID of the user.</param>
        /// <returns></returns>
        public async Task<IEnumerable<DomainModels.Clip>> GetUnasignedClipsAsync(Guid sessionUid, string userUid)
        {
            var session = await DbContext.Sessions
                .Include(x => x.TimelineEntries)
               .Include(x => x.Clips).ThenInclude(c => c.Media)
               .Include(x => x.Clips).ThenInclude(c => c.Edit)
               .Include(x => x.Podcast).ThenInclude(x => x.Members)
               .FirstOrDefaultAsync(x => x.UID == sessionUid && x.IsActive);

            var sessionClips = session.Clips.Where(c => !session.TimelineEntries.Any(x => x.ClipID == c.ID));

            if (session.Podcast.Members.Any(x => x.UserUID == userUid && x.IsAdmin))
            {
                var uids = session.Clips.Select(c => c.Media.Select(x => x.UserUID))
                    .SelectMany(c => c)
                    .Distinct();

                var userInfo = new List<User>();
                foreach(var uid in uids)
                    userInfo.Add(await UserFacade.GetUserInfoAsync(uid));
                

                var clips = sessionClips.Select(x => new DomainModels.Clip
                {
                    UID = x.UID,
                    ID = x.ID,
                    Date = x.Date,
                    SessionUID = x.Session.UID,
                    Name = x.Name,
                    Media = x.Media.Select(c => new DomainModels.Media
                    {
                        ID = c.ID,
                        Date = c.Date,
                        UID = c.UID,
                        MediaUrl = c.MediaUrl,
                        UserUID = c.UserUID,
                        UserImageUrl = userInfo.FirstOrDefault(a => a.user_id == c.UserUID).picture,
                        UserName = userInfo.FirstOrDefault(a => a.user_id == c.UserUID).name,
                        
                    }),
                    Edit = x.Edit != null ? new DomainModels.Edit
                    {
                        ID = x.Edit.ID,
                        StartTrim = x.Edit.StartTrim,
                        EndTrim = x.Edit.EndTrim,
                        Gain = x.Edit.Gain,
                        ClipUID = x.UID
                    } : null
                });

                return clips.OrderBy(c => c.Date);
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
