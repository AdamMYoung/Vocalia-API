import { PodcastEpisode, Listen } from "../utility/types";

export default class LocalRepository {
  /**
   * Gets the playback time for the provided url from the local storage.
   * @param url Podcast URL for querying.
   */
  GetPlaybackTime(rssUrl: string): Listen | null {
    let key = rssUrl + "|position";
    if (localStorage.getItem(key) != null) {
      return JSON.parse(localStorage.getItem(key) as string) as Listen;
    }
    return null;
  }

  /**
   * Sets the local playback time for the provided url.
   * @param url URL to save to.
   * @param time Time to save.
   */
  SetPlaybackTime(listenInfo: Listen) {
    let key = listenInfo.rssUrl + "|position";
    localStorage.setItem(key, JSON.stringify(listenInfo));
  }

  /**
   * Sets the podcast URL as the current podcast.
   * @param url Podcast to save as current.
   */
  SetCurrentPodcast(episode: PodcastEpisode) {
    let key = "|current";
    localStorage.setItem(key, JSON.stringify(episode));
  }

  /**
   * Gets the saved podcast, or null if not available.
   */
  GetCurrentPodcast(): PodcastEpisode {
    let key = "|current";
    let item = localStorage.getItem(key);
    if (item != null) return JSON.parse(item);
    else return { time: 0 } as PodcastEpisode;
  }
}
