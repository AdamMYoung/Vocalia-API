import { Category, Podcast, PodcastFeed } from "../types";

const API = "http://localhost:54578/podcast/";
const CATEGORIES = "categories";
const SUBSCRIBED = "subscribed";
const TOP = "top";
const PARSE = "parse";

class VocaliaAPI {
  /**
   * Gets all categories from the Vocalia API.
   */
  async getCategories(): Promise<Category[]> {
    return await fetch(API + CATEGORIES)
      .then(response => response.json())
      .then(data => data as Category[]);
  }

  /**
   * Gets the top podcasts from the Vocalia API.
   */
  async getTopPodcasts(): Promise<Podcast[]> {
    return await fetch(API + TOP)
      .then(response => response.json())
      .then(data => data as Podcast[]);
  }

  /**
   * Gets the subscribed podcasts from the Vocalia API.
   */
  async getSubscribedPodcasts(): Promise<Podcast[]> {
    return await fetch(API + SUBSCRIBED)
      .then(response => response.json())
      .then(data => data as Podcast[]);
  }

  /**
   * Gets the top podcasts from the provided category from the Vocalia API.
   * @param categoryId ID of the category to filter by.
   */
  async getPodcastByCategory(categoryId: number): Promise<Podcast[]> {
    return await fetch(API + TOP + "?categoryId" + categoryId)
      .then(response => response.json())
      .then(data => data as Podcast[]);
  }

  /**
   * Parses the RSS URL into a JSON formatted object with
   * additional usage data using the Vocalia API.
   * @param rssUrl URL to parse.
   */
  async parsePodcastFeed(rssUrl: string): Promise<PodcastFeed> {
    return await fetch(API + PARSE + "?feedUrl=" + rssUrl)
      .then(response => response.json())
      .then(data => data as PodcastFeed);
  }
}

export default VocaliaAPI;
