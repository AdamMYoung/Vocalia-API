using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vocalia.Ingest.Db;
using Microsoft.EntityFrameworkCore;
using Vocalia.Ingest.DomainModels;

namespace Vocalia.Ingest.Repositories
{
    public class IngestRepository : IIngestRepository
    {
        /// <summary>
        /// Database context to the Ingest database.
        /// </summary>
        private IngestContext DbContext { get; }

        /// <summary>
        /// Repository for ingest data.
        /// </summary>
        /// <param name="context"></param>
        public IngestRepository(IngestContext context)
        {
            DbContext = context;
        }

        /// <summary>
        /// Creates a new session for the specified podcast UID.
        /// </summary>
        /// <param name="podcastUID">UID of the podcast.</param>
        /// <returns></returns>
        public async Task<Guid> CreateNewSessionAsync(Guid podcastUID)
        {
            var podcast = await DbContext.Podcasts.SingleOrDefaultAsync(x => x.UID == podcastUID);
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
            return session.UID;
        }

        /// <summary>
        /// Returns the current active session belonging to the podcast UID, or null if none is available.
        /// </summary>
        /// <param name="podcastUID">UID of the podcast.</param>
        /// <returns></returns>
        public async Task<DomainModels.Session> GetCurrentSessionAsync(Guid podcastUID)
        {
            var session = await DbContext.Sessions.SingleOrDefaultAsync(x => x.Podcast.UID == podcastUID && x.InProgress);

            return session == null ? null : new DomainModels.Session()
            {
                ID = session.ID,
                UID = session.UID,
                PodcastID = session.PodcastID,
                Date = session.Date,
                InProgress = session.InProgress
            };
        }

        /// <summary>
        /// Gets all podcasts belonging to the specified group UID.
        /// </summary>
        /// <param name="groupUID">UID of the group.</param>
        /// <returns></returns>
        public async Task<IEnumerable<DomainModels.Podcast>> GetGroupPodcastsAsync(Guid groupUID)
        {
            var podcasts = await DbContext.Podcasts.Where(x => x.Group.UID == groupUID).ToListAsync();

            return podcasts.Select(x => new DomainModels.Podcast
            {
                ID = x.ID,
                UID = x.UID,
                GroupID = x.GroupID,
                Name = x.Name
            });
        }

        /// <summary>
        /// Gets all sesions belonging to the specified podcast UID.
        /// </summary>
        /// <param name="podcastUID">UID of the podcast.</param>
        /// <returns></returns>
        public async Task<IEnumerable<DomainModels.Session>> GetPodcastSessionsAsync(Guid podcastUID)
        {
            var sessions = await DbContext.Sessions.Where(x => x.Podcast.UID == podcastUID).ToListAsync();

            return sessions.Select(x => new DomainModels.Session
            {
                ID = x.ID,
                UID = x.UID,
                PodcastID = x.PodcastID,
                Date = x.Date,
                InProgress = x.InProgress
            });
        }

        /// <summary>
        /// Gets all groups belonging to the specified user.
        /// </summary>
        /// <param name="userUID">UID of the user.</param>
        /// <returns></returns>
        public async Task<IEnumerable<DomainModels.Group>> GetUserGroupsAsync(string userUID)
        {
            var groups = await DbContext.UserGroups.Where(x => x.UserUID == userUID)
                .Select(c => c.Group).ToListAsync();

            return groups.Select(x => new DomainModels.Group
            {
                ID = x.ID,
                UID = x.UID,
                Name = x.Name,
                Description = x.Description
            });
        }
    }
}
