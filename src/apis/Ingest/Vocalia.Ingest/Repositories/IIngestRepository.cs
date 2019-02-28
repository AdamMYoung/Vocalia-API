using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Ingest.Repositories
{
    public interface IIngestRepository
    {
        /// <summary>
        /// Gets all groups belonging to the specified user.
        /// </summary>
        /// <param name="userUID">UID of the user.</param>
        /// <returns></returns>
        Task<IEnumerable<DomainModels.Group>> GetUserGroupsAsync(string userUID);

        /// <summary>
        /// Gets all podcasts belonging to the specified group UID.
        /// </summary>
        /// <param name="groupUID">UID of the group.</param>
        /// <returns></returns>
        Task<IEnumerable<DomainModels.Podcast>> GetGroupPodcastsAsync(Guid groupUID, string userUID);

        /// <summary>
        /// Gets all sesions belonging to the specified podcast UID.
        /// </summary>
        /// <param name="podcastUID">UID of the podcast.</param>
        /// <returns></returns>
        Task<IEnumerable<DomainModels.Session>> GetPodcastSessionsAsync(Guid podcastUID, string userUID);

        /// <summary>
        /// Creates a new session for the specified podcast UID.
        /// </summary>
        /// <param name="podcastUID">UID of the podcast.</param>
        /// <returns></returns>
        Task<Guid?> CreateNewSessionAsync(Guid podcastUID, string userUID);
    }
}
