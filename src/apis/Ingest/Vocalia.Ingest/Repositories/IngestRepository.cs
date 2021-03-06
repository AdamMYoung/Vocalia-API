﻿using Microsoft.EntityFrameworkCore;
using ObjectBus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Vocalia.Ingest.Db;
using Vocalia.Ingest.DomainModels;
using Vocalia.Ingest.Image;
using Vocalia.Ingest.Media;
using Vocalia.ServiceBus.Types;
using Vocalia.Streams;

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
        /// Object to build media streams.
        /// </summary>
        private IStreamBuilder StreamBuilder { get; }

        /// <summary>
        /// Message bus for sending message blobs to the editor.
        /// </summary>
        private IObjectBus<IEnumerable<ServiceBus.Types.Clip>> ClipBus { get; }

        /// <summary>
        /// Message bus for sending new podcasts to listeners.
        /// </summary>
        private IObjectBus<ServiceBus.Types.Editor.Podcast> PodcastBus { get; }

        /// <summary>
        /// Repository for ingest data.
        /// </summary>
        /// <param name="context"></param>
        public IngestRepository(IngestContext context, IImageStorage imageStorage,
            IMediaStorage mediaStorage, IStreamBuilder streamBuilder,
            IObjectBus<IEnumerable<ServiceBus.Types.Clip>> clipBus, 
            IObjectBus<ServiceBus.Types.Editor.Podcast> podcastBus)
        {
            DbContext = context;
            ImageStorage = imageStorage;
            MediaStorage = mediaStorage;
            StreamBuilder = streamBuilder;
            ClipBus = clipBus;
            PodcastBus = podcastBus;
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
            var session = await DbContext.Sessions
                .Include(c => c.Podcast)
                .Include(c => c.Clips) .ThenInclude(c => c.Media)
                .FirstOrDefaultAsync(x => x.UID == sessionUID && x.Podcast.Users.Any(c => c.UserUID == userUID && c.IsAdmin));

            if (session == null)
                return false;
        
            session.IsFinished = true;
            await DbContext.SaveChangesAsync();

            var clips = session.Clips.Select(c => new Vocalia.ServiceBus.Types.Clip
            {
                SessionUID = c.Session.UID,
                Date = c.Time,
                Name = c.Name,
                PodcastUID = c.Session.Podcast.UID,
                UID = c.UID,
                Media = c.Media.Select(x => new ServiceBus.Types.Media
                {
                    MediaUrl = x.MediaUrl,
                    UID = x.UID,
                    UserUID = x.UserUID
                }).ToList()
            });

            await ClipBus.SendAsync(clips);

            return true;
            
        }

        /// <summary>
        /// Deletes the specified clip from the database.
        /// </summary>
        /// <param name="clipUid">Clip to delete.</param>
        /// <param name="userUid">User UID requesting the deletion.</param>
        /// <returns></returns>
        public async Task<bool> DeleteClipAsync(Guid clipUid, string userUid)
        {
            var clip = await DbContext.Clips.FirstOrDefaultAsync(c => c.UID == clipUid &&
                c.Session.Podcast.Users.Any(x => x.UserUID == userUid && x.IsAdmin));

            if (clip == null)
                return false;

            DbContext.Clips.Remove(clip);
            await DbContext.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Gets all clips belonging to the specified sessionUID.
        /// </summary>
        /// <param name="sessionUid">UID to get session information for. </param>
        /// <param name="userUid">User UID requesting the info.</param>
        /// <returns></returns>
        public async Task<IEnumerable<DomainModels.SessionClip>> GetClipsAsync(Guid sessionUid, string userUid)
        {
            var clips = await DbContext.Clips.Where(x => x.Session.UID == sessionUid).ToListAsync();
            return clips.Select(c => new DomainModels.SessionClip
            {

                UID = c.UID,
                MediaUrl = c.MediaUrl,
                Time = c.Time,
                Name = c.Name
            });
        }

        /// <summary>
        /// Builds the current clips stored into a single stream, then removes the clips.
        /// </summary>
        /// <param name="sessionUid">UID of the session to proces</param>
        /// <returns></returns>
        public async Task<bool> FinishClipAsync(string userUid, DomainModels.BlobUpload upload)
        {
            var session = await DbContext.Sessions.Include(x => x.Clips).ThenInclude(x => x.Media)
                .Include(x => x.Podcast).FirstOrDefaultAsync(x => x.UID == upload.SessionUid);

            if (session == null)
                return false;

            using (var stream = upload.Data.OpenReadStream())
            {
                var url = await MediaStorage.UploadStreamAsync(userUid, upload.SessionUid, upload.ClipUid, stream);

                var clip = session.Clips.FirstOrDefault(c => c.UID == upload.ClipUid);
                if (clip == null)
                {
                    var id = CreateClip(url, session.ID, upload.ClipUid, upload.Name);
                    clip = await DbContext.Clips.FindAsync(id);
                }

                var media = new Db.Media
                {
                    ClipID = clip.ID,
                    MediaUrl = url,
                    UserUID = userUid,
                    UID = Guid.NewGuid()
                };

                DbContext.Media.Add(media);

                var mediaList = new List<Stream>();
                foreach(var entry in clip.Media)
                    mediaList.Add(await StreamBuilder.GetStreamFromUrlAsync(entry.MediaUrl));

                if(mediaList.Count() > 1)
                {
                    var concatStream = Audio.AudioConcatUtils.ConcatAudioStreams(mediaList);
                    var concatUrl = await MediaStorage.UploadStreamAsync("concat", upload.SessionUid, upload.ClipUid, stream);
                    clip.MediaUrl = concatUrl;
                }
            }

            await DbContext.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Creates a new clip in the database.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionId"></param>
        /// <param name="uid"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private int CreateClip(string url, int sessionId, Guid uid, string name)
        {
            var clip = new Db.Clip
            {
                MediaUrl = url,
                SessionID = sessionId,
                Time = DateTime.Now,
                UID = uid,
                Name = name
            };

            DbContext.Clips.Add(clip);
            return clip.ID;
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

            await PodcastBus.SendAsync(new ServiceBus.Types.Editor.Podcast
            {
                Name = dbPodcast.Name,
                UID = dbPodcast.UID,
                ImageUrl = dbPodcast.ImageUrl,
                Members = new List<Member>() { new Member() { IsAdmin = true, UserUID = userUID }}
            });
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
    }
}
