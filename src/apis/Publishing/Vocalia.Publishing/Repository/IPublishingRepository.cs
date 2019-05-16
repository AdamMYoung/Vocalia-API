using System;
using System.Collections.Generic;
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
        Task<IEnumerable<UnassignedPodcast>> GetAllUnassignedPodcastsAsync(string userUid);

        /// <summary>
        /// Gets all unassigned episodes from the database where the user is an admin.
        /// </summary>
        /// <param name="userUid">UID of the user.</param>
        /// <returns></returns>
        Task<IEnumerable<UnassignedEpisode>> GetAllUnassignedEpisodesAsync(string userUid);

        /// <summary>
        /// Gets all languages that can be assigned.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Language>> GetLanguagesAsync();

        /// <summary>
        /// Gets all categories that can be assigned.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Category>> GetCategoriesAsync();

        /// <summary>
        /// Gets all published podcasts belonging to the user.
        /// </summary>
        /// <param name="userUid">UID of the user.</param>
        /// <returns></returns>
        Task<IEnumerable<Podcast>> GetAllPodcastsAsync(string userUid);

        /// <summary>
        /// Deletes the specified podcast and episodes from the database, if the user is an admin.
        /// </summary>
        /// <param name="userUid">UID of the user.</param>
        /// <param name="podcastUid">UID of the podcast.</param>
        /// <returns></returns>
        Task<bool> DeletePodcastAsync(string userUid, Guid podcastUid);

        /// <summary>
        /// Updates the specified podcast object in the database.
        /// </summary>
        /// <param name="userUid">UID of the user.</param>
        /// <param name="podcast">Podcast to update in the database.</param>
        /// <returns></returns>
        Task<bool> UpdatePodcastAsync(string userUid, Podcast podcast);

        /// <summary>
        /// Deletes the specified episode from the database, if the user is an admin.
        /// </summary>
        /// <param name="userUid">UID of the user.</param>
        /// <param name="episodeUid">UID of the episode.</param>
        /// <returns></returns>
        Task<bool> DeleteEpisodeAsync(string userUid, Guid episodeUid);

        /// <summary>
        /// Updates the specified episode object in the database.
        /// </summary>
        /// <param name="userUid">UID of the user.</param>
        /// <param name="episode">Episode to add update in the database.</param>
        /// <returns></returns>
        Task<bool> UpdateEpisodeAsync(string userUid, Episode episode);

        /// <summary>
        /// Gets the RSS feed for the specified podcast UID.
        /// </summary>
        /// <param name="podcastUid">UID of the podcast.</param>
        /// <returns></returns>
        Task<string> GetRssAsync(Guid podcastUid);
    }
}
