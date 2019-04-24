using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vocalia.Publishing.DomainModels;

namespace Vocalia.Publishing.Repository
{
    public interface IPublishingRepository
    {
        /// <summary>
        /// Gets all unassigned podcasts from the database where the user is an admin.
        /// </summary>
        /// <param name="userUid">UID of the user.</param>
        /// <returns></returns>
        Task<IEnumerable<UnassignedPodcast>> GetAllUnassignedPodcasts(string userUid);

        /// <summary>
        /// Gets all unassigned episodes from the database where the user is an admin.
        /// </summary>
        /// <param name="userUid">UID of the user.</param>
        /// <returns></returns>
        Task<IEnumerable<UnassignedEpisode>> GetAllUnassignedEpisodes(string userUid);

        /// <summary>
        /// Gets all languages that can be assigned.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Language>> GetLanguages();

        /// <summary>
        /// Gets all categories that can be assigned.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Category>> GetCategories();

        /// <summary>
        /// Gets all published podcasts belonging to the user.
        /// </summary>
        /// <param name="userUid">UID of the user.</param>
        /// <returns></returns>
        Task<IEnumerable<Podcast>> GetAllPodcasts(string userUid);

        /// <summary>
        /// Deletes the specified podcast and episodes from the database, if the user is an admin.
        /// </summary>
        /// <param name="userUid">UID of the user.</param>
        /// <param name="podcastUid">UID of the podcast.</param>
        /// <returns></returns>
        Task<bool> DeletePodcast(string userUid, Guid podcastUid);

        /// <summary>
        /// Updates the specified podcast object in the database.
        /// </summary>
        /// <param name="userUid">UID of the user.</param>
        /// <param name="podcast">Podcast to update in the database.</param>
        /// <returns></returns>
        Task<bool> UpdatePodcast(string userUid, Podcast podcast);

        /// <summary>
        /// Deletes the specified episode from the database, if the user is an admin.
        /// </summary>
        /// <param name="userUid">UID of the user.</param>
        /// <param name="episodeUid">UID of the episode.</param>
        /// <returns></returns>
        Task<bool> DeleteEpisode(string userUid, Guid episodeUid);

        /// <summary>
        /// Updates the specified episode object in the database.
        /// </summary>
        /// <param name="userUid">UID of the user.</param>
        /// <param name="episode">Episode to add update in the database.</param>
        /// <returns></returns>
        Task<bool> UpdateEpisode(string userUid, Episode episode);
    }
}
