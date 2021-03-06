﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vocalia.Editor.Repository
{
    public interface IEditorRepository
    {
        /// <summary>
        /// Returns all podcasts editable by the user.
        /// </summary>
        /// <param name="userUID">User to get podcasts for.</param>
        /// <returns></returns>
        Task<IEnumerable<DomainModels.Podcast>> GetPodcastsAsync(string userUID);

        /// <summary>
        /// Gets podcast info about the specified podcastUID if the user is an admin.
        /// </summary>
        /// <param name="userUID">UID of the user.</param>
        /// <param name="podcastUid">UID of the podcast.</param>
        /// <returns></returns>
        Task<DomainModels.Podcast> GetPodcastDetailAsync(Guid podcastUid, string userUID);

        /// <summary>
        /// Deletes the specified session from the database.
        /// </summary>
        /// <param name="userUid">UID of the user.</param>
        /// <param name="sessionUid">UID of the session.</param>
        /// <returns></returns>
        Task<bool> DeleteSessionAsync(Guid sessionUid, string userUid);

        /// <summary>
        /// Applies the specified edit to the audio stream attached to the sessionUID and userUID
        /// </summary>
        /// <param name="sessionUid">UID of the session.</param>
        /// <param name="userUid">UID of the user.</param>
        /// <param name="edit">Edit to apply.</param>
        /// <returns></returns>
        Task<bool> AddEditAsync(string userUid, DomainModels.Edit edit);

        /// <summary>
        /// Gets the current timeline from the database.
        /// </summary>
        /// <param name="sessionUid">UID of the session.</param>
        /// <param name="userUid">UID of the user.</param>
        /// <returns></returns>
        Task<IEnumerable<DomainModels.Clip>> GetTimelineAsync(Guid sessionUid, string userUid);

        /// <summary>
        /// Sets the timeline to the provided clips.
        /// </summary>
        /// <param name="sessionUid">UID of the session.</param>
        /// <param name="userUid">UID of the user.</param>
        /// <param name="clips">Clips to set as the timeline.</param>
        /// <returns></returns>
        Task<bool> SetTimelineAsync(Guid sessionUid, string userUid, IEnumerable<DomainModels.Clip> clips);

        /// <summary>
        /// Gets all clips from the database.
        /// </summary>
        /// <param name="sessionUid">UID of the session.</param>
        /// <param name="userUid">UID of the user.</param>
        /// <returns></returns>
        Task<IEnumerable<DomainModels.Clip>> GetUnassignedClipsAsync(Guid sessionUid, string userUid);

        /// <summary>
        /// Compiles the edit files into one streamable file and sends it to the publisher.
        /// </summary>
        /// <param name="sessionUid">UID of the session.</param>
        /// <param name="userUid">User requesting the action.</param>
        /// <returns></returns>
        Task<bool> FinishEditingAsync(Guid sessionUid, string userUid);
    }
}
