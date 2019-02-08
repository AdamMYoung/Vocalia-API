import { PodcastEpisode } from "./types";

/**
 * Gets the playback time for the provided url from the local storage.
 * @param url Podcast URL for querying.
 */
export function GetPlaybackTime(url: string): number {
  let key = url + "|position";
  if (localStorage.getItem(key) != null) {
    return parseInt(localStorage.getItem(key) as string);
  }
  return 0;
}

/**
 * Sets the local playback time for the provided url.
 * @param url URL to save to.
 * @param time Time to save.
 */
export function SetPlaybackTime(url: string, time: number) {
  let key = url + "|position";
  localStorage.setItem(key, time.toString());
}

/**
 * Sets the podcast URL as the current podcast.
 * @param url Podcast to save as current.
 */
export function SetCurrentPodcast(episode: PodcastEpisode) {
  let key = "|current";
  localStorage.setItem(key, JSON.stringify(episode));
}

/**
 * Gets the saved podcast, or null if not available.
 */
export function GetCurrentPodcast(): PodcastEpisode {
  let key = "|current";
  let item = localStorage.getItem(key);
  if (item != null) return JSON.parse(item);
  else return { time: 0 } as PodcastEpisode;
}
