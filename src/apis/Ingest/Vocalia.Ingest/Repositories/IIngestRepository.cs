using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vocalia.Ingest.DomainModels;

namespace Vocalia.Ingest.Repositories
{
    public interface IIngestRepository
    {
        /// <summary>
        /// Returns general information about all podcasts belonging to the specified user.
        /// </summary>
        /// <param name="userUID">User to get podcasts for.</param>
        /// <returns></returns>
        Task<IEnumerable<Podcast>> GetPodcastsAsync(string userUID);

        /// <summary>
        /// Gets detailed podcast info for the specified podcast UID.
        /// </summary>
        /// <param name="userUID">UID of the user.</param>
        /// <param name="podcastUid">UID of the podcast.</param>
        /// <returns></returns>
        Task<Podcast> GetPodcastDetailAsync(string userUID, Guid podcastUid);

        /// <summary>
        /// Gets general podcast info for the specified podcast UID.
        /// </summary>
        /// <param name="podcastUid">UID of the podcast.</param>
        /// <returns></returns>
        Task<Podcast> GetPodcastOverviewAsync(Guid podcastUid);

        /// <summary>
        /// Creates a new podcast for the specified user using the provided information.
        /// </summary>
        /// <param name="userUID">ID to create the podcast for.</param>
        /// <param name="podcast">Podcast info to add.</param>
        /// <param name="fileType">File type of the image being uploaded.</param>
        /// <returns></returns>
        Task CreatePodcastAsync(string userUID, PodcastUpload podcast);

        /// <summary>
        /// Updates the podcast with the specified info, if the user is an admin.
        /// </summary>
        /// <param name="userUID">User performing the request.</param>
        /// <param name="podcast">Podcast info to update.</param>
        /// <returns></returns>
        Task UpdatePodcastAsync(string userUID, PodcastUpload podcast);

        /// <summary>
        /// Deletes the specified podcast if the user is an admin.
        /// </summary>
        /// <param name="podcsatUID">UID of the podcast to remove.</param>
        /// <param name="userUID">UID of the user performing the action.</param>
        /// <returns></returns>
        Task DeletePodcastAsync(Guid podcastUID, string userUID);

        /// <summary>
        /// Creates a new session for the specified podcast UID.
        /// </summary>
        /// <param name="podcastUID">UID of the podcast.</param>
        /// <returns></returns>
        Task<Guid?> CreateSessionAsync(Guid podcastUID, string userUID);

        /// <summary>
        /// Deletes the specified session from the podcast entries, if the user is an admin.
        /// </summary>
        /// <param name="sessionUID">Session to delete.</param>
        /// <param name="userUID">UID of the user.</param>
        /// <returns></returns>
        Task<bool> DeleteSessionAsync(Guid sessionUID, string userUID);

        /// <summary>
        /// Completes the specified session in the database, if the user is an admin.
        /// </summary>
        /// <param name="sessionUID">Session to complete.</param>
        /// <param name="userUID">UID of the user.</param>
        /// <returns></returns>
        Task<bool> CompleteSessionAsync(Guid sessionUID, string userUID);

        /// <summary>
        /// Deletes the specified clip from the database.
        /// </summary>
        /// <param name="clipUid">Clip to delete.</param>
        /// <param name="userUid">User UID requesting the deletion.</param>
        /// <returns></returns>
        Task<bool> DeleteClipAsync(Guid clipUid, string userUid);

        /// <summary>
        /// Gets all clips belonging to the specified sessionUID.
        /// </summary>
        /// <param name="sessionUid">UID to get session information for. </param>
        /// <param name="userUid">User UID requesting the info.</param>
        /// <returns></returns>
        Task<IEnumerable<DomainModels.SessionClip>> GetClipsAsync(Guid sessionUid, string userUid);

        /// <summary>
        /// Builds the current clips stored into a single stream, then removes the clips.
        /// </summary>
        /// <param name="sessionUid">UID of the session to proces</param>
        /// <returns></returns>
        Task<bool> FinishClipAsync(Guid sessionUid, string userUid);

        /// <summary>
        /// Returns the podcast assigned to the invite link.
        /// </summary>
        /// <param name="inviteLink">Invite GUID to check.</param>
        /// <returns></returns>
        Task<Podcast> GetInviteInfoAsync(Guid inviteLink);

        /// <summary>
        /// Creates an invite link for the specified podcastUID.
        /// </summary>
        /// <param name="podcastUID">Podcast to create link for.</param>
        /// <param name="userUID">User ID to add the group to.</param>
        /// <returns></returns>
        Task<Guid?> CreateInviteLinkAsync(Guid podcastUID, string userUID, DateTime? expiry);

        /// <summary>
        /// Accepts the invite link. Returns true if the user was added, false if not.
        /// </summary>
        /// <param name="inviteLink">GUID to accept.</param>
        /// <param name="userUID">User accepting the invite.</param>
        /// <returns></returns>
        Task<bool> AcceptInviteLinkAsync(Guid inviteLink, string userUID);

        /// <summary>
        /// Posts a blob to the blob storage database.
        /// </summary>
        /// <param name="blob">Blob to upload.</param>
        /// <returns></returns>
        Task PostMediaBlobAsync(BlobUpload blob);
    }
}
