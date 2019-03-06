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
        /// Creates a group for the specified user.
        /// </summary>
        /// <param name="userUID">User to add the group to.</param>
        /// <param name="name">Name of the group.</param>
        /// <param name="description">Description of the group.</param>
        /// <returns></returns>
        Task CreateGroupAsync(string userUID, string name, string description);

        /// <summary>
        /// Gets all podcasts belonging to the specified group UID.
        /// </summary>
        /// <param name="groupUID">UID of the group.</param>
        /// <returns></returns>
        Task<IEnumerable<DomainModels.Podcast>> GetGroupPodcastsAsync(Guid groupUID, string userUID);

        /// <summary>
        /// Creates a new podcast for the specified user using the provided information.
        /// </summary>
        /// <param name="userUID">ID to create the podcast for.</param>
        /// <param name="groupId">Group to insert the podcast into.</param>
        /// <param name="name">Name of the podcast.</param>
        /// <returns></returns>
        Task CreateGroupPodcastAsync(string userUID, Guid groupId, string name);

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

        /// <summary>
        /// Creates an invite link for the specified groupUID.
        /// </summary>
        /// <param name="groupUID">Group to create link for.</param>
        /// <param name="userUID">User ID to add the group to.</param>
        /// <returns></returns>
        Task<Guid?> CreateInviteLinkAsync(Guid groupUID, string userUID, DateTime? expiry);

        /// <summary>
        /// Accepts the invite link. Returns true if the user was added, false if not.
        /// </summary>
        /// <param name="inviteLink">GUID to accept.</param>
        /// <returns></returns>
        Task<bool> AcceptInviteLinkAsync(Guid inviteLink, string userUID);
    }
}
