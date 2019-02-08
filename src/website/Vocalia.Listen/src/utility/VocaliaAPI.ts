import { Category, Podcast, PodcastFeed, Subscription } from "../utility/types";
var request = require("request");

const API = "http://localhost:54578/podcast/";
const CATEGORIES = "categories";
const SUBSCRIBED = "subscribed";
const TOP = "top";
const PARSE = "parse";
const SEARCH = "search";
const SUBS = "subscriptions";

class VocaliaAPI {
  /**
   * Gets all categories from the Vocalia API.
   */
  async getCategories(): Promise<Category[]> {
    return await fetch(API + CATEGORIES)
      .then(response => response.json())
      .then(data => data as Category[])
      .catch(() => Promise.reject("Failed fetching categories."));
  }

  /**
   * Gets the top podcasts from the Vocalia API.
   */
  async getTopPodcasts(): Promise<Podcast[]> {
    return await fetch(API + TOP)
      .then(response => response.json())
      .then(data => data as Podcast[])
      .catch(() => Promise.reject("Failed fetching top podcasts."));
  }

  /**
   * Gets the subscribed podcasts from the Vocalia API.
   */
  async getSubscribedPodcasts(): Promise<Podcast[]> {
    return await fetch(API + SUBSCRIBED)
      .then(response => response.json())
      .then(data => data as Podcast[])
      .catch(() => Promise.reject("Failed fetching subscribed podcasts"));
  }

  /**
   * Gets the subscribed podcasts from the Vocalia API.
   */
  async searchPodcasts(query: string): Promise<Podcast[]> {
    return await fetch(API + SEARCH + "?term=" + query)
      .then(response => response.json())
      .then(data => data as Podcast[])
      .catch(() => Promise.reject("Failed searching for term: " + query));
  }

  /**
   * Gets the top podcasts from the provided category from the Vocalia API.
   * @param categoryId ID of the category to filter by.
   */
  async getPodcastByCategory(categoryId: number): Promise<Podcast[]> {
    return await fetch(API + TOP + "?categoryId=" + categoryId)
      .then(response => response.json())
      .then(data => data as Podcast[])
      .catch(() =>
        Promise.reject("Failed fetching category. ID: " + categoryId)
      );
  }

  /**
   * Parses the RSS URL into a JSON formatted object with
   * additional usage data using the Vocalia API.
   * @param rssUrl URL to parse.
   */
  async parsePodcastFeed(rssUrl: string): Promise<PodcastFeed> {
    if (rssUrl != "undefined") {
      return await fetch(API + PARSE + "?rssUrl=" + rssUrl)
        .then(response => response.json())
        .then(data => data as PodcastFeed)
        .catch(() =>
          Promise.reject("Failed parsing RSS feed. Feed: " + rssUrl)
        );
    }
    return Promise.reject("Bad Request");
  }

  /**
   * Gets the subscriptions belonging to the user.
   * @param accessToken Authentication token for API validation
   */
  async getSubscriptions(accessToken: string): Promise<Subscription[]> {
    console.log(this.getHeaders(accessToken));
    return await fetch(API + SUBS, { headers: this.getHeaders(accessToken) })
      .then(response => response.json())
      .then(data => data as Subscription[])
      .catch(() => Promise.reject("Failed parsing subscription feed."));
  }

  async addSubscription(authToken: string, sub: Subscription) {}

  async deleteSubscription(authToken: string, guid: string) {}

  getHeaders(accessToken: string): Headers {
    return new Headers({
      "content-type": "application/json",
      Authorization: "Bearer " + accessToken
    });
  }
}

export default VocaliaAPI;
