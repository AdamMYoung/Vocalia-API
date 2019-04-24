using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Vocalia.Publishing.Db;
using Vocalia.Publishing.DomainModels;
using Vocalia.Publishing.Media;
using Vocalia.Streams;

namespace Vocalia.Publishing.Repository
{
    public class PublishingRepository : IPublishingRepository
    {
        /// <summary>
        /// Database repository.
        /// </summary>
        private PublishContext DbContext { get; }

        /// <summary>
        /// System configuration options.
        /// </summary>
        private IConfiguration Config { get; }

        /// <summary>
        /// Blob media storage manager.
        /// </summary>
        private IMediaStorage MediaStorage { get; }

        /// <summary>
        /// Handles data streams.
        /// </summary>
        private IStreamBuilder StreamBuilder { get; }

        public PublishingRepository(PublishContext ingestContext, IMediaStorage mediaStorage, 
            IStreamBuilder streamBuilder, IConfiguration config)
        {
            DbContext = ingestContext;
            MediaStorage = mediaStorage;
            StreamBuilder = streamBuilder;
        }

        /// <summary>
        /// Deletes the specified episode from the database, if the user is an admin.
        /// </summary>
        /// <param name="userUid">UID of the user.</param>
        /// <param name="episodeUid">UID of the episode.</param>
        /// <returns></returns>
        public async Task<bool> DeleteEpisode(string userUid, Guid episodeUid)
        {
            var dbEpisode = await DbContext.Episodes.FirstOrDefaultAsync(c => c.UID == episodeUid && c.Podcast.Members.Any(x => x.UserUID == userUid));
            if (dbEpisode == null)
                return false;

            dbEpisode.IsActive = false;
            await DbContext.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Adds or updates the specified episode object to the database, if an unassigned episode already exists with the same UID.
        /// </summary>
        /// <param name="userUid">UID of the user.</param>
        /// <param name="episode">Episode to add to the database.</param>
        /// <returns></returns>
        public async Task<bool> UpdateEpisode(string userUid, DomainModels.Episode episode)
        {
            var dbPodcast = await DbContext.Podcasts.Include(x => x.Episodes).FirstOrDefaultAsync(c => c.UID == episode.PodcastUID && c.Members.Any(x => x.UserUID == userUid));
            if (dbPodcast == null)
                return false;

            var dbEpisode = dbPodcast.Episodes.FirstOrDefault(c => c.UID == episode.UID);
            if (dbEpisode == null)
                dbEpisode = await CreateNewEpisode(userUid, episode);

            if (dbEpisode == null)
                return false;

            dbEpisode.Title = episode.Title;
            dbEpisode.Description = episode.Description;

            await DbContext.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Creates a new episode from the stored unassigned episode.
        /// </summary>
        /// <param name="userUid">UserUID of the current user.</param>
        /// <param name="episode">Episode to create.</param>
        /// <returns></returns>
        private async Task<Db.Episode> CreateNewEpisode(string userUid, DomainModels.Episode episode)
        {
            var dbPodcast = await DbContext.Podcasts.FirstOrDefaultAsync(c => c.UID == episode.PodcastUID && c.Members.Any(x => x.UserUID == userUid));
            var dbUnassignedEpisode = await DbContext.UnassignedEpisodes.Include(c => c.Clips).FirstOrDefaultAsync(c => c.UID == episode.UID);

            if (dbUnassignedEpisode == null)
                return null;

            var clipStreams = new List<Stream>();
            foreach (var clip in dbUnassignedEpisode.Clips)
                clipStreams.Add(await StreamBuilder.GetStreamFromUrlAsync(clip.MediaUrl));

            var compiledStream = Audio.AudioConcatUtils.SequenceAudioStreams(clipStreams);
            var url = await MediaStorage.UploadStreamAsync("rss", dbPodcast.UID, Guid.NewGuid(), compiledStream);

            var dbEpisode = new Db.Episode
            {
                UID = Guid.NewGuid(),
                Title = episode.Title,
                Description = episode.Description,
                RssUrl = string.Concat(Config.GetSection("RssUrl").ToString(), dbPodcast.UID, "/", episode.UID),
                MediaUrl = url,
                PublishDate = DateTime.Now,
                PodcastID = dbPodcast.ID
            };

            DbContext.Episodes.Add(dbEpisode);
            await DbContext.SaveChangesAsync();

            return dbEpisode;
        }

        /// <summary>
        /// Adds or updates the specified podcast object to the database, if an unassigned podcast already exists with the same UID.
        /// </summary>
        /// <param name="userUid">UID of the user.</param>
        /// <param name="podcast">Podcast to add to the database.</param>
        /// <returns></returns>
        public async Task<bool> UpdatePodcast(string userUid, DomainModels.Podcast podcast)
        {
            var dbPodcast = await DbContext.Podcasts.FirstOrDefaultAsync(c => c.UID == podcast.UID && c.Members.Any(x => x.UserUID == userUid));
            if (dbPodcast == null)
            {
                await CreateNewPodcast(userUid, podcast);
                return true;
            }

            if (dbPodcast == null)
                return false;

            dbPodcast.Title = podcast.Title;
            dbPodcast.Description = podcast.Description;
            dbPodcast.CategoryID = podcast.CategoryID;
            dbPodcast.LanguageID = podcast.LanguageID;
            dbPodcast.IsExplicit = podcast.IsExplicit;
            dbPodcast.IsActive = podcast.IsActive;

            await DbContext.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Inserts a new podcast into the database.
        /// </summary>
        /// <param name="userUid">UID of the user.</param>
        /// <param name="podcast">Podcast to insert.</param>
        /// <returns></returns>
        private async Task CreateNewPodcast(string userUid, DomainModels.Podcast podcast)
        {
            var unassignedDbContext = await DbContext.UnassignedPodcasts.Include(c => c.Members).FirstOrDefaultAsync(c => c.UID == podcast.UID && c.Members.Any(x => x.UserUID == userUid));
            if (unassignedDbContext == null)
                return;

            var dbPodcast = new Db.Podcast
            {
                UID = podcast.UID,
                Title = podcast.Title,
                Description = podcast.Description,
                CategoryID = podcast.CategoryID,
                LanguageID = podcast.LanguageID,
                ImageUrl = podcast.ImageUrl,
                IsActive = podcast.IsActive,
                IsExplicit = podcast.IsExplicit
            };

            DbContext.Podcasts.Add(dbPodcast);

            var dbMembers = unassignedDbContext.Members.Select(c => new Db.Member
            {
                UserUID = c.UserUID,
                PodcastID = dbPodcast.ID
            });

            DbContext.Members.AddRange(dbMembers);

            await DbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes the specified podcast and episodes from the database, if the user is an admin.
        /// </summary>
        /// <param name="userUid">UID of the user.</param>
        /// <param name="podcastUid">UID of the podcast.</param>
        /// <returns></returns>
        public async Task<bool> DeletePodcast(string userUid, Guid podcastUid)
        {
            var dbPodcast = await DbContext.Podcasts.FirstOrDefaultAsync(c => c.UID == podcastUid);
            if (dbPodcast == null) 
                return false;

            dbPodcast.IsActive = false;
            await DbContext.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Gets all published podcasts belonging to the user.
        /// </summary>
        /// <param name="userUid">UID of the user.</param>
        /// <returns></returns>
        public async Task<IEnumerable<DomainModels.Podcast>> GetAllPodcasts(string userUid)
        {
            var dbPodcasts = await DbContext.Podcasts.Include(c => c.Episodes).Where(c => c.Members.Any(x => x.UserUID == userUid)).ToListAsync();

            return dbPodcasts.Select(c => new DomainModels.Podcast
            {
                ID = c.ID,
                UID = c.UID,
                CategoryID = c.CategoryID,
                LanguageID = c.LanguageID,
                Title = c.Title,
                Description = c.Description,
                ImageUrl = c.ImageUrl,
                IsActive = c.IsActive,
                IsExplicit = c.IsExplicit,
                Episodes = c.Episodes.Select(x => new DomainModels.Episode
                {
                    ID = x.ID,
                    UID = x.UID,
                    PodcastUID = c.UID,
                    Title = x.Title,
                    Description = x.Description,
                    MediaUrl = x.MediaUrl,
                    RssUrl = x.RssUrl,
                    PublishDate = x.PublishDate,
                    IsActive = x.IsActive
                })
            });
        }

        /// <summary>
        /// Gets all unassigned episodes from the database where the user is an admin.
        /// </summary>
        /// <param name="userUid">UID of the user.</param>
        /// <returns></returns>
        public async Task<IEnumerable<DomainModels.UnassignedEpisode>> GetAllUnassignedEpisodes(string userUid)
        {
            var dbEpisodes = await DbContext.UnassignedEpisodes.Include(c => c.Podcast).Where(c => c.Podcast.Members.Any(x => x.UserUID == userUid)).ToListAsync();

            return dbEpisodes.Select(x => new DomainModels.UnassignedEpisode
            {
                ID = x.ID,
                UID = x.UID,
                IsCompleted = x.IsCompleted,
                UnassignedPodcastUID = x.Podcast.UID
            });
        }

        /// <summary>
        /// Gets all unassigned podcasts from the database where the user is an admin.
        /// </summary>
        /// <param name="userUid">UID of the user.</param>
        /// <returns></returns>
        public async Task<IEnumerable<DomainModels.UnassignedPodcast>> GetAllUnassignedPodcasts(string userUid)
        {
            var dbPodcasts = await DbContext.UnassignedPodcasts.Include(c => c.Episodes).Where(c => c.Members.Any(x => x.UserUID == userUid)).ToListAsync();

            return dbPodcasts.Select(c => new DomainModels.UnassignedPodcast
            {
                ID = c.ID,
                UID = c.UID,
                Name = c.Name,
                ImageUrl = c.ImageUrl,
                IsCompleted = c.IsCompleted,
                Episodes = c.Episodes.Select(x => new DomainModels.UnassignedEpisode
                {
                    ID = x.ID,
                    UID = x.UID,
                    IsCompleted = x.IsCompleted,
                    UnassignedPodcastUID = c.UID
                })
            });
        }

        /// <summary>
        /// Gets all languages that can be assigned.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<DomainModels.Language>> GetLanguages()
        {
           return await DbContext.Languages.Select(c => new DomainModels.Language
            {
                ID = c.ID,
                Name = c.Name,
                ISOCode = c.ISOCode
            }).ToListAsync();
        }

        /// <summary>
        /// Gets all categories that can be assigned.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<DomainModels.Category>> GetCategories()
        {
            return await DbContext.Categories.Select(c => new DomainModels.Category
            {
                ID = c.ID,
                ITunesID = c.ITunesID,
                Title = c.Title,
                GPodderTag = c.GPodderTag
            }).ToListAsync();
        }
    }
}
