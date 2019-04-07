using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ObjectBus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vocalia.Ingest.Db;
using Vocalia.Ingest.DomainModels;
using Vocalia.Ingest.Image;
using Vocalia.Ingest.Media;
using Vocalia.ServiceBus.Types;

namespace Vocalia.Ingest.Repositories
{
    public class IngestRepository : IIngestRepository
    {
        /// <summary>
        /// Database context to the Ingest database.
        /// </summary>
        private IngestContext DbContext { get; }

        /// <summary>
        /// Blob storage library for image upload.
        /// </summary>
        private IImageStorage ImageStorage { get; }

        /// <summary>
        /// Blob storage library for media upload.
        /// </summary>
        private IMediaStorage MediaStorage { get; }


        /// <summary>
        /// Message bus for sending message blobs to the editor.
        /// </summary>
        private IObjectBus<RecordingChunk> EditorMessageBus { get; }

        /// <summary>
        /// Repository for ingest data.
        /// </summary>
        /// <param name="context"></param>
        public IngestRepository(IngestContext context, IImageStorage imageStorage,
            IMediaStorage mediaStorage, IObjectBus<RecordingChunk> editorBus)
        {
            DbContext = context;
            ImageStorage = imageStorage;
            MediaStorage = mediaStorage;
            EditorMessageBus = editorBus;
        }

        #region Session

        /// <summary>
        /// Creates a new session for the specified podcast UID.
        /// </summary>
        /// <param name="podcastUID">UID of the podcast.</param>
        /// <param name="userUID">UID of the user.</param>
        /// <returns></returns>
        public async Task<Guid?> CreateSessionAsync(Guid podcastUID, string userUID)
        {
            var podcast = await DbContext.Podcasts.SingleOrDefaultAsync(x => x.UID == podcastUID &&
                x.Users.Any(c => c.UserUID == userUID && c.IsAdmin));

            if (podcast != null)
            {
                var inProgress = await DbContext.Sessions.Where(x => x.Podcast.UID == podcastUID && !x.IsFinished)
                    .ToListAsync();

                inProgress.ForEach(x => x.IsFinished = true);
                await DbContext.SaveChangesAsync();

                var session = new Db.Session
                {
                    Date = DateTime.UtcNow,
                    PodcastID = podcast.ID,
                    IsFinished = false
                };

                await DbContext.Sessions.AddAsync(session);
                await DbContext.SaveChangesAsync();
                return session.UID;
            }
            return null;
        }

        /// <summary>
        /// Deletes the specified session from the podcast entries, if the user is an admin.
        /// </summary>
        /// <param name="sessionUID">Session to delete.</param>
        /// <param name="userUID">UID of the user.</param>
        /// <returns></returns>
        public async Task<bool> DeleteSessionAsync(Guid sessionUID, string userUID)
        {
            var session = await DbContext.Sessions.FirstOrDefaultAsync(x => x.UID == sessionUID &&
            x.Podcast.Users.Any(c => c.UserUID == userUID && c.IsAdmin));

            if (session == null)
            {
                return false;
            }
            else
            {
                DbContext.Sessions.Remove(session);
                await DbContext.SaveChangesAsync();
                return true;
            }
        }

        /// <summary>
        /// Completes the specified session in the database, if the user is an admin.
        /// </summary>
        /// <param name="sessionUID">Session to complete.</param>
        /// <param name="userUID">UID of the user.</param>
        /// <returns></returns>
        public async Task<bool> CompleteSessionAsync(Guid sessionUID, string userUID)
        {
            var session = await DbContext.Sessions.FirstOrDefaultAsync(x => x.UID == sessionUID &&
            x.Podcast.Users.Any(c => c.UserUID == userUID && c.IsAdmin));

            if (session == null)
            {
                return false;
            }
            else
            {
                session.IsFinished = true;
                await DbContext.SaveChangesAsync();
                await BuildMedia(sessionUID);
                return true;
            }

        }

        #endregion

        #region Podcast

        /// <summary>
        /// Returns general information about all podcasts belonging to the specified user.
        /// </summary>
        /// <param name="userUID">User to get podcasts for.</param>
        /// <returns></returns>
        public async Task<IEnumerable<DomainModels.Podcast>> GetPodcastsAsync(string userUID)
        {
            var podcasts = await DbContext.Podcasts
                .Include(x => x.Sessions)
                .Include(x => x.Users)
                .Where(x => x.Users.Any(c => c.UserUID == userUID)).ToListAsync();

            return podcasts.Select(x => new DomainModels.Podcast
            {
                ID = x.ID,
                UID = x.UID,
                Name = x.Name,
                ImageUrl = x.ImageUrl
            });
        }

        /// <summary>
        /// Gets detailed podcast info for the specified podcast UID.
        /// </summary>
        /// <param name="userUID">UID of the user.</param>
        /// <param name="podcastUid">UID of the podcast.</param>
        /// <returns></returns>
        public async Task<DomainModels.Podcast> GetPodcastDetailAsync(string userUID, Guid podcastUid)
        {
            var podcast = await DbContext.Podcasts
                .Include(x => x.Sessions)
                .Include(x => x.Users)
                .FirstOrDefaultAsync(x => x.Users.Any(c => c.UserUID == userUID) && x.UID == podcastUid);

            if (podcast == null)
                return null;

            return new DomainModels.Podcast
            {
                ID = podcast.ID,
                UID = podcast.UID,
                Name = podcast.Name,
                Description = podcast.Description,
                ImageUrl = podcast.ImageUrl,
                Members = podcast.Users.Select(m => new DomainModels.PodcastUser
                {
                    ID = m.ID,
                    UID = m.UserUID,
                    IsAdmin = m.IsAdmin,
                    PodcastID = m.PodcastID
                }),
                Sessions = podcast.Sessions.Where(x => !x.IsFinished)
                .OrderByDescending(c => c.Date)
                .Select(s => new DomainModels.Session
                {
                    ID = s.ID,
                    PodcastID = s.PodcastID,
                    UID = s.UID,
                    Date = s.Date,
                    IsFinished = s.IsFinished
                })
            };
        }

        /// <summary>
        /// Gets general podcast info for the specified podcast UID.
        /// </summary>
        /// <param name="podcastUid">UID of the podcast.</param>
        /// <returns></returns>
        public async Task<DomainModels.Podcast> GetPodcastOverviewAsync(Guid podcastUid)
        {
            var podcast = await DbContext.Podcasts
               .FirstOrDefaultAsync(x => x.UID == podcastUid);

            if (podcast == null)
                return null;

            return new DomainModels.Podcast
            {
                ID = podcast.ID,
                UID = podcast.UID,
                Name = podcast.Name,
                Description = podcast.Description,
                ImageUrl = podcast.ImageUrl
            };
        }

        /// <summary>
        /// Creates a new podcast for the specified user using the provided information.
        /// </summary>
        /// <param name="userUID">ID to create the podcast for.</param>
        /// <param name="podcast">Podcast info to add.</param>
        /// <param name="fileType">File type of the image being uploaded.</param>
        /// <returns></returns>
        public async Task CreatePodcastAsync(string userUID, PodcastUpload podcast)
        {
            var imageDataByteArray = Convert.FromBase64String(podcast.ImageData);

            var dbPodcast = new Db.Podcast
            {
                Name = podcast.Name,
                Description = podcast.Description,
                ImageUrl = await ImageStorage.UploadImageAsync(imageDataByteArray, podcast.FileType)
            };

            await DbContext.Podcasts.AddAsync(dbPodcast);

            var dbPodcastUsers = new Db.PodcastUser
            {
                PodcastID = dbPodcast.ID,
                UserUID = userUID,
                IsAdmin = true
            };

            await DbContext.PodcastUsers.AddAsync(dbPodcastUsers);
            await DbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Updates the podcast with the specified info, if the user is an admin.
        /// </summary>
        /// <param name="userUID">User performing the request.</param>
        /// <param name="podcast">Podcast info to update.</param>
        /// <returns></returns>
        public async Task UpdatePodcastAsync(string userUID, PodcastUpload podcast)
        {
            var imageDataByteArray = Convert.FromBase64String(podcast.ImageData);

            var dbPodcast = await DbContext.Podcasts.FirstOrDefaultAsync(x => x.UID == podcast.UID &&
            x.Users.Any(c => c.UserUID == userUID && c.IsAdmin));

            if (dbPodcast != null)
            {
                dbPodcast.Name = podcast.Name;
                dbPodcast.Description = podcast.Description;
                dbPodcast.ImageUrl = await ImageStorage.UploadImageAsync(imageDataByteArray, podcast.FileType);

                await DbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Deletes the specified podcast if the user is an admin.
        /// </summary>
        /// <param name="podcsatUID">UID of the podcast to remove.</param>
        /// <param name="userUID">UID of the user performing the action.</param>
        /// <returns></returns>
        public async Task DeletePodcastAsync(Guid podcastUID, string userUID)
        {
            var dbPodcast = await DbContext.Podcasts.FirstOrDefaultAsync(x => x.UID == podcastUID &&
            x.Users.Any(c => c.UserUID == userUID && c.IsAdmin));

            if (dbPodcast != null)
            {
                DbContext.Podcasts.Remove(dbPodcast);
                await DbContext.SaveChangesAsync();
            }
        }

        #endregion

        #region Invite

        /// <summary>
        /// Returns the podcast assigned to the invite link.
        /// </summary>
        /// <param name="inviteLink">Invite GUID to check.</param>
        /// <returns></returns>
        public async Task<DomainModels.Podcast> GetInviteInfoAsync(Guid inviteLink)
        {
            var invite = await DbContext.PodcastInvites.Include(x => x.Podcast)
                .FirstOrDefaultAsync(x => x.InviteUID == inviteLink && (x.Expiry == null || x.Expiry > DateTime.Now));

            if (invite == null)
                return null;

            var podcast = invite.Podcast;

            return new DomainModels.Podcast
            {
                Name = podcast.Name,
                Description = podcast.Description,
                ImageUrl = podcast.ImageUrl,
                UID = podcast.UID
            };
        }

        /// <summary>
        /// Creates an invite link for the specified podcastUID.
        /// </summary>
        /// <param name="podcastUID">Podcast to create link for.</param>
        /// <param name="userUID">User ID to add the group to.</param>
        /// <param name="expiry">Expiry time of the invite.</param>
        /// <returns></returns>
        public async Task<Guid?> CreateInviteLinkAsync(Guid podcastUID, string userUID, DateTime? expiry)
        {
            var podcast = await DbContext.Podcasts.Include(x => x.Users)
                .FirstOrDefaultAsync(x => x.UID == podcastUID);

            if (podcast.Users.Any(x => x.UserUID == userUID && x.IsAdmin))
            {
                var invite = new Db.PodcastInvite
                {
                    PodcastID = podcast.ID,
                    Expiry = expiry,
                };

                await DbContext.PodcastInvites.AddAsync(invite);
                await DbContext.SaveChangesAsync();

                return invite.InviteUID;
            }

            return null;
        }

        /// <summary>
        /// Accepts the invite link. Returns true if the user was added, false if not.
        /// </summary>
        /// <param name="inviteLink">GUID to accept.</param>
        /// <param name="userUID">User accepting the invite.</param>
        /// <returns></returns>
        public async Task<bool> AcceptInviteLinkAsync(Guid inviteLink, string userUID)
        {
            var link = await DbContext.PodcastInvites.Include(x => x.Podcast)
                .ThenInclude(x => x.Users).FirstOrDefaultAsync(x => x.InviteUID == inviteLink);

            if (link != null && (link.Expiry == null || link.Expiry > DateTime.Now))
            {
                if (link.Podcast.Users.Any(x => x.UserUID == userUID))
                    return false;

                var podcastUser = new Db.PodcastUser
                {
                    UserUID = userUID,
                    PodcastID = link.PodcastID,
                    IsAdmin = false
                };

                await DbContext.PodcastUsers.AddAsync(podcastUser);
                await DbContext.SaveChangesAsync();
                return true;
            }

            return false;
        }

        #endregion

        #region Ingestion

        /// <summary>
        /// Posts a blob to the blob storage and adds a reference to the database.
        /// </summary>
        /// <param name="blob">Blob to upload.</param>
        /// <returns></returns>
        public async Task PostMediaBlobAsync(BlobUpload blob)
        {
            var url = await MediaStorage.UploadBlobAsync(blob);
            var session = await DbContext.Sessions.FirstOrDefaultAsync(x => x.UID == blob.SessionUID);

            if (session != null)
            {
                var entry = new SessionMedia
                {
                    SessionID = session.ID,
                    UserUID = blob.UserUID,
                    Timestamp = blob.Timestamp,
                    MediaUrl = url
                };

                await DbContext.SessionMedia.AddAsync(entry);
                await DbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Builds the session media files from stored media.
        /// </summary>
        /// <param name="sessionUid">UID of the session to proces</param>
        /// <returns></returns>
        private async Task BuildMedia(Guid sessionUid)
        {
            var entries = await GetSessionBlobsAsync(sessionUid);

            foreach (var entry in entries)
            {
                var session = await DbContext.Sessions.FirstOrDefaultAsync(x => x.UID == sessionUid);
                var currentEntries = DbContext.SessionMedia.Where(x => x.Session.UID == sessionUid && x.UserUID == entry.UserUID);

                await currentEntries
                    .Select(x => new RecordingChunk(x.Session.UID, x.Session.Podcast.UID, x.UserUID, x.MediaUrl, x.Timestamp))
                    .ForEachAsync(async x => await EditorMessageBus.SendAsync(x));
            }
        }

        /// <summary>
        /// Gets all user media blobs belonging to the session if authorized.
        /// </summary>
        /// <param name="sessionUID">UID of the session.</param>
        /// <param name="userUID">UID of the user.</param>
        /// <returns></returns>
        private async Task<IEnumerable<RecordingEntry>> GetSessionBlobsAsync(Guid sessionUID)
        {
            var session = await DbContext.Sessions.Include(c => c.MediaEntries)
                .FirstOrDefaultAsync(x => x.UID == sessionUID);

            var userUids = session.MediaEntries.Select(x => x.UserUID).Distinct();
            var recordingEntries = new List<RecordingEntry>();

            foreach (var user in userUids)
            {
                recordingEntries.Add(new RecordingEntry
                {
                    UserUID = user,
                    SessionUID = sessionUID,
                    Blobs = session.MediaEntries.OrderBy(c => c.Timestamp)
                    .Where(x => x.UserUID == user && x.Session.UID == sessionUID)
                    .Select(c => new BlobEntry()
                    {
                        ID = c.ID,
                        SessionUID = sessionUID,
                        Timestamp = c.Timestamp,
                        Url = c.MediaUrl
                    })
                });
            }

            return recordingEntries;
        }

        #endregion
    }
}
