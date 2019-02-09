import { Category, Podcast, PodcastFeed } from "../utility/types";
var request = require("request");

const API = "http://localhost:54578/podcast/";
const CATEGORIES = "categories";
const TOP = "top";
const PARSE = "parse";
const SEARCH = "search";
const SUBS = "subscriptions";

export default class ApiRepository {
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
  async parsePodcastFeed(
    rssUrl: string,
    accessToken: string
  ): Promise<PodcastFeed> {
    if (rssUrl != "undefined") {
      var path = API + PARSE + "?rssUrl=" + rssUrl;
      return await this.getInjectedFetch(path, accessToken)
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
  async getSubscriptions(accessToken: string): Promise<Podcast[]> {
    return await this.getInjectedFetch(API + SUBS, accessToken)
      .then(response => response.json())
      .then(data => data as Podcast[])
      .catch(() => Promise.reject("Failed parsing subscription feed."));
  }

  /**
   * Adds the specified podcast to the user's subscribed database.
   * @param accessToken Access token used for API authentication.
   * @param podcast Podcast to subscribe to.
   */
  async addSubscription(accessToken: string, podcast: Podcast) {}

  /**
   * Rempoves the specified podcast from the user's subscribed database.
   * @param accessToken Access token used for API authentication.
   * @param rssUrl RSS url of the podcast to unsubsribe from.
   */
  async deleteSubscription(accessToken: string, rssUrl: string) {}

  /**
   * Injects a fetch object with access token headers and returns it.
   * @param url Path to query.
   * @param accessToken Access token to verify users.
   */
  private getInjectedFetch(url: string, accessToken: string) {
    var headers = new Headers({
      "content-type": "application/json",
      Authorization: "Bearer " + accessToken
    });

    return fetch(url, { headers: headers });
  }
}
