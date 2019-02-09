import { Category, Podcast, PodcastFeed, Listen } from "../utility/types";
var request = require("request");

const API = "http://localhost:54578/podcast/";
const CATEGORIES = "categories";
const TOP = "top";
const PARSE = "parse";
const SEARCH = "search";
const SUBS = "subscriptions";
const LISTEN = "listen";

export default class ApiRepository {
  /**
   * Gets all categories from the Vocalia API.
   */
  async getCategories(): Promise<Category[] | null> {
    return await fetch(API + CATEGORIES)
      .then(response => response.json())
      .then(data => data as Category[])
      .catch(() => null);
  }

  /**
   * Gets the top podcasts from the Vocalia API.
   */
  async getTopPodcasts(): Promise<Podcast[] | null> {
    return await fetch(API + TOP)
      .then(response => response.json())
      .then(data => data as Podcast[])
      .catch(() => null);
  }

  /**
   * Gets the subscribed podcasts from the Vocalia API.
   */
  async searchPodcasts(query: string): Promise<Podcast[] | null> {
    return await fetch(API + SEARCH + "?term=" + query)
      .then(response => response.json())
      .then(data => data as Podcast[])
      .catch(() => null);
  }

  /**
   * Gets the top podcasts from the provided category from the Vocalia API.
   * @param categoryId ID of the category to filter by.
   */
  async getPodcastByCategory(categoryId: number): Promise<Podcast[] | null> {
    return await fetch(API + TOP + "?categoryId=" + categoryId)
      .then(response => response.json())
      .then(data => data as Podcast[])
      .catch(() => null);
  }

  /**
   * Parses the RSS URL into a JSON formatted object with
   * additional usage data using the Vocalia API.
   * @param rssUrl URL to parse.
   */
  async parsePodcastFeed(
    rssUrl: string,
    accessToken: string
  ): Promise<PodcastFeed | null> {
    if (rssUrl != "undefined") {
      var path = API + PARSE + "?rssUrl=" + rssUrl;
      return await this.getInjectedFetch(path, accessToken)
        .then(response => response.json())
        .then(data => data as PodcastFeed)
        .catch(() => null);
    }
    return Promise.reject("Bad Request");
  }

  /**
   * Gets the subscriptions belonging to the user.
   * @param accessToken Authentication token for API validation
   */
  async getSubscriptions(accessToken: string): Promise<Podcast[] | null> {
    return await this.getInjectedFetch(API + SUBS, accessToken)
      .then(response => response.json())
      .then(data => data as Podcast[])
      .catch(() => null);
  }

  /**
   * Adds the specified podcast to the user's subscribed database.
   * @param accessToken Access token used for API authentication.
   * @param podcast Podcast to subscribe to.
   */
  async addSubscription(accessToken: string, podcast: Podcast) {
    return await this.getInjectedFetch(
      API + SUBS,
      accessToken,
      "POST",
      podcast
    );
  }

  /**
   * Rempoves the specified podcast from the user's subscribed database.
   * @param accessToken Access token used for API authentication.
   * @param rssUrl RSS url of the podcast to unsubsribe from.
   */
  async deleteSubscription(accessToken: string, rssUrl: string) {
    return await this.getInjectedFetch(
      API + SUBS,
      accessToken,
      "DELETE",
      rssUrl
    );
  }

  /**
   * Updates the listen info for the specified podcast.
   * @param listenInfo Values to update.
   * @param accessToken Access token used for API authentication.
   */
  async setListenInfo(accessToken: string, listenInfo: Listen) {
    return await this.getInjectedFetch(
      API + LISTEN,
      accessToken,
      "POST",
      listenInfo
    );
  }

  /**
   * Gets the listen info for the specified podcast.
   * @param rssUrl URL to get the listen info for.
   * @param accessToken Access token for API authentication.
   */
  async getListenInfo(
    accessToken: string,
    rssUrl: string
  ): Promise<Listen | null> {
    return await this.getInjectedFetch(
      API + LISTEN + "?url=" + rssUrl,
      accessToken
    )
      .then(response => response.json())
      .then(data => data as Listen)
      .catch(() => null);
  }

  /**
   * Injects a fetch object with access token headers and returns it.
   * @param url Path to query.
   * @param accessToken Access token to verify users.
   */
  private getInjectedFetch(
    url: string,
    accessToken: string,
    queryType: string = "GET",
    body: {} | null = null
  ) {
    var headers = new Headers({
      "content-type": "application/json",
      Authorization: "Bearer " + accessToken
    });

    return fetch(url, {
      headers: headers,
      method: queryType,
      body: JSON.stringify(body)
    });
  }
}
