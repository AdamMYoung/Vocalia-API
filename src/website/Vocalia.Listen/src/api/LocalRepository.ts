import { PodcastEpisode, Listen, Category, Podcast } from "../utility/types";

const PLAYBACK_POSITION = "|position";
const CURRENT = "|current";
const CATEGORIES = "|categories";
const PODCASTS = "|podcasts";

export default class LocalRepository {
  /**
   * Sets the local playback time for the provided url.
   * @param url URL to save to.
   * @param time Time to save.
   */
  setPlaybackTime(listenInfo: Listen) {
    let key = listenInfo.rssUrl + PLAYBACK_POSITION;
    localStorage.setItem(key, JSON.stringify(listenInfo));
  }

  /**
   * Gets the playback time for the provided url from the local storage.
   * @param url Podcast URL for querying.
   */
  getPlaybackTime(rssUrl: string): Listen | null {
    let key = rssUrl + PLAYBACK_POSITION;
    return this.ParseKey(key);
  }

  /**
   * Sets the podcast URL as the current podcast.
   * @param url Podcast to save as current.
   */
  setCurrentPodcast(episode: PodcastEpisode | null) {
    if (episode == null) {
      localStorage.removeItem(CURRENT);
    } else {
      localStorage.setItem(CURRENT, JSON.stringify(episode));
    }
  }

  /**
   * Gets the saved podcast, or null if not available.
   */
  getCurrentPodcast(): PodcastEpisode | null {
    return this.ParseKey(CURRENT);
  }

  /**
   * Stores the passed categories in the local storage.
   */
  setCategories(categories: Category[]) {
    localStorage.setItem(CATEGORIES, JSON.stringify(categories));
  }

  /**
   * Gets the stored categories if available.
   */
  getCategories(): Category[] | null {
    return this.ParseKey(CATEGORIES);
  }

  /**
   * Sets the podcasts belonging to the specified category.
   * @param podcasts Podcasts to store.
   * @param category Category the podcasts belong to.
   */
  setCategoryPodcasts(podcasts: Podcast[], category: string) {
    let key = category + PODCASTS;
    localStorage.setItem(key, JSON.stringify(podcasts));
  }

  /**
   * Gets the podcasts belonging to the specified category.
   * @param category Category to filter podcasts by.
   */
  getCategoryPodcasts(category: string) {
    let key = category + PODCASTS;
    return this.ParseKey(key);
  }

  /**
   * Returns items from local storage, or null if nothing is found.
   * @param key Key to parse.
   */
  private ParseKey(key: string): any | null {
    let item = localStorage.getItem(key);
    if (item != null) return JSON.parse(item);
    else return null;
  }
}
