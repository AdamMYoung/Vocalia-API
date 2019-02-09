import ApiRepository from "./ApiRepository";
import LocalRepository from "./LocalRepository";
import { Category, Podcast, PodcastFeed } from "../utility/types";

export default class DataManager {
  api: ApiRepository = new ApiRepository();
  local: LocalRepository = new LocalRepository();
  private accessToken: string = "";

  setAccessToken(accessToken: string) {
    this.accessToken = accessToken;
  }

  /**
   * Gets all categories from the Vocalia API.
   */
  async getCategories(): Promise<Category[]> {
    return await this.api.getCategories();
  }

  /**
   * Gets the top podcasts from the Vocalia API.
   */
  async getTopPodcasts(): Promise<Podcast[]> {
    return await this.api.getTopPodcasts();
  }

  /**
   * Gets the subscribed podcasts from the Vocalia API.
   */
  async searchPodcasts(query: string): Promise<Podcast[]> {
    return await this.api.searchPodcasts(query);
  }

  /**
   * Gets the top podcasts from the provided category from the Vocalia API.
   * @param categoryId ID of the category to filter by.
   */
  async getPodcastByCategory(categoryId: number): Promise<Podcast[]> {
    return await this.api.getPodcastByCategory(categoryId);
  }

  /**
   * Parses the RSS URL into a JSON formatted object with
   * additional usage data using the Vocalia API.
   * @param rssUrl URL to parse.
   */
  async parsePodcastFeed(rssUrl: string): Promise<PodcastFeed> {
    var feed = await this.api.parsePodcastFeed(rssUrl, this.accessToken);

    return feed;
  }

  /**
   * Gets the subscriptions belonging to the user.
   * @param accessToken Authentication token for API validation
   */
  async getSubscriptions(): Promise<Podcast[]> {
    return await this.api.getSubscriptions(this.accessToken);
  }

  /**
   * Adds the specified podcast to the user's subscribed database.
   * @param accessToken Access token used for API authentication.
   * @param podcast Podcast to subscribe to.
   */
  async addSubscription(podcast: Podcast) {
    await this.api.addSubscription(this.accessToken, podcast);
  }

  /**
   * Rempoves the specified podcast from the user's subscribed database.
   * @param accessToken Access token used for API authentication.
   * @param rssUrl RSS url of the podcast to unsubsribe from.
   */
  async deleteSubscription(rssUrl: string) {
    await this.api.deleteSubscription(this.accessToken, rssUrl);
  }
}
