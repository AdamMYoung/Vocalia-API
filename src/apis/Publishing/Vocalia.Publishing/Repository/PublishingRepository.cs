using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vocalia.Ingest.Db;
using Vocalia.Publishing.DomainModels;

namespace Vocalia.Publishing.Repository
{
    public class PublishingRepository : IPublishingRepository
    {
        /// <summary>
        /// Database repository.
        /// </summary>
        private IngestContext DbContext { get; }

        public PublishingRepository(IngestContext ingestContext)
        {
            DbContext = ingestContext;
        }

        /// <summary>
        /// Adds or updates the specified episode object to the database, if an unassigned episode already exists with the same UID.
        /// </summary>
        /// <param name="userUid">UID of the user.</param>
        /// <param name="episode">Episode to add to the database.</param>
        /// <returns></returns>
        public Task<bool> UpdateEpisode(string userUid, Episode episode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds or updates the specified podcast object to the database, if an unassigned podcast already exists with the same UID.
        /// </summary>
        /// <param name="userUid">UID of the user.</param>
        /// <param name="podcast">Podcast to add to the database.</param>
        /// <returns></returns>
        public Task<bool> UpdatePodcast(string userUid, DomainModels.Podcast podcast)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes the specified podcast and episodes from the database, if the user is an admin.
        /// </summary>
        /// <param name="userUid">UID of the user.</param>
        /// <param name="podcastUid">UID of the podcast.</param>
        /// <returns></returns>
        public Task<bool> DeletePodcast(string userUid, Guid podcastUid)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets all published podcasts belonging to the user.
        /// </summary>
        /// <param name="userUid">UID of the user.</param>
        /// <returns></returns>
        public Task<IEnumerable<DomainModels.Podcast>> GetAllPodcasts(string userUid)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets all unassigned episodes from the database where the user is an admin.
        /// </summary>
        /// <param name="userUid">UID of the user.</param>
        /// <returns></returns>
        public Task<IEnumerable<UnassignedEpisode>> GetAllUnassignedEpisodes(string userUid)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets all unassigned podcasts from the database where the user is an admin.
        /// </summary>
        /// <param name="userUid">UID of the user.</param>
        /// <returns></returns>
        public Task<IEnumerable<UnassignedPodcast>> GetAllUnassignedPodcasts(string userUid)
        {
            throw new NotImplementedException();
        }
    }
}
