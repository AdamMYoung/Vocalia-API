import ApiRepository from "./ApiRepository";
import LocalRepository from "./LocalRepository";
import { Category, Podcast, PodcastFeed, Listen } from "../utility/types";

export default class DataManager {
  private api: ApiRepository = new ApiRepository();
  private local: LocalRepository = new LocalRepository();
  private accessToken: string = "";

  setAccessToken(accessToken: string) {
    this.accessToken = accessToken;
  }

  /**
   * Gets the subscribed podcasts from the Vocalia API.
   */
  async searchPodcasts(query: string): Promise<Podcast[] | null> {
    return await this.api.searchPodcasts(query);
  }

  /**
   * Parses the RSS URL into a JSON formatted object with
   * additional usage data using the Vocalia API.
   * @param rssUrl URL to parse.
   */
  async parsePodcastFeed(rssUrl: string): Promise<PodcastFeed | null> {
    var feed = await this.api.parsePodcastFeed(this.accessToken, rssUrl);

    return feed;
  }

  /**
   * Gets all categories from the Vocalia API.
   */
  async getCategories(): Promise<Category[] | null> {
    var categories = await this.api.getCategories();

    if (categories != null) {
      this.local.setCategories(categories);
      return categories;
    }

    return this.local.getCategories();
  }

  /**
   * Gets the top podcasts from the Vocalia API.
   */
  async getTopPodcasts(): Promise<Podcast[] | null> {
    var podcasts = await this.api.getTopPodcasts();

    if (podcasts != null) {
      this.local.setCategoryPodcasts(podcasts, "top");
      return podcasts;
    }

    return this.local.getCategoryPodcasts("top");
  }

  /**
   * Gets the top podcasts from the provided category from the Vocalia API.
   * @param categoryId ID of the category to filter by.
   */
  async getPodcastByCategory(categoryId: number): Promise<Podcast[] | null> {
    var podcasts = await this.api.getPodcastByCategory(categoryId);

    if (podcasts != null) {
      this.local.setCategoryPodcasts(podcasts, categoryId.toString());
      return podcasts;
    }

    return this.local.getCategoryPodcasts(categoryId.toString());
  }

  /**
   * Gets the subscriptions belonging to the user.
   */
  async getSubscriptions(): Promise<Podcast[] | null> {
    var subs = await this.api.getSubscriptions(this.accessToken);

    if (subs != null) {
      this.local.setCategoryPodcasts(subs, "subscriptions");
      return subs;
    }

    return this.local.getCategoryPodcasts("subscriptions");
  }

  /**
   * Adds the specified podcast to the user's subscribed database.
   * @param podcast Podcast to subscribe to.
   */
  async addSubscription(podcast: Podcast) {
    await this.api.addSubscription(this.accessToken, podcast);
  }

  /**
   * Rempoves the specified podcast from the user's subscribed database.
   * @param rssUrl RSS url of the podcast to unsubsribe from.
   */
  async deleteSubscription(rssUrl: string) {
    await this.api.deleteSubscription(this.accessToken, rssUrl);
  }

  /**
   * Updates the listen info for the specified podcast.
   * @param listen Values to update.
   */
  async setListenInfo(listen: Listen) {
    if (this.accessToken != null) {
      await this.api.setListenInfo(this.accessToken, listen);
    }

    this.local.setPlaybackTime(listen);
  }

  /**
   * Gets lisen info for the specified podcast.
   * @param rssUrl URL to fetch listen info for.
   */
  async getListenInfo(rssUrl: string): Promise<Listen | null> {
    if (this.accessToken != null) {
      return await this.api.getListenInfo(rssUrl, this.accessToken);
    }

    return this.local.getPlaybackTime(rssUrl);
  }
}
