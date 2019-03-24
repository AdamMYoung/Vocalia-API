using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vocalia.Ingest.Db;
using Microsoft.EntityFrameworkCore;
using Vocalia.Ingest.DomainModels;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Vocalia.Ingest.ImageService;
using System.Text;

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
        private IImageStorageService ImageStorage { get; }

        /// <summary>
        /// Repository for ingest data.
        /// </summary>
        /// <param name="context"></param>
        public IngestRepository(IngestContext context, IImageStorageService imageStorage)
        {
            DbContext = context;
            ImageStorage = imageStorage;
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
                var inProgress = await DbContext.Sessions.Where(x => x.Podcast.UID == podcastUID && x.InProgress)
                    .ToListAsync();

                inProgress.ForEach(x => x.InProgress = false);
                await DbContext.SaveChangesAsync();

                var session = new Db.Session
                {
                    Date = DateTime.Now,
                    PodcastID = podcast.ID,
                    InProgress = true
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
        public async Task DeleteSessionAsync(Guid sessionUID, string userUID)
        {
            var session = await DbContext.Sessions.FirstOrDefaultAsync(x => x.UID == sessionUID &&
            x.Podcast.Users.Any(c => c.UserUID == userUID && c.IsAdmin));

            if (session != null)
            {
                DbContext.Sessions.Remove(session);
                await DbContext.SaveChangesAsync();
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
                ImageUrl = x.ImageUrl,
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
                Sessions = podcast.Sessions.OrderByDescending(c => c.Date)
                .Select(s => new DomainModels.Session
                {
                    ID = s.ID,
                    PodcastID = s.PodcastID,
                    UID = s.UID,
                    Date = s.Date,
                    InProgress = s.InProgress
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
        public async Task CreatePodcastAsync(string userUID, DomainModels.PodcastUpload podcast)
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
        public async Task UpdatePodcastAsync(string userUID, DomainModels.PodcastUpload podcast)
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
                .FirstOrDefaultAsync(x => x.InviteUID == inviteLink && ( x.Expiry == null || x.Expiry > DateTime.Now));

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
    }
}
