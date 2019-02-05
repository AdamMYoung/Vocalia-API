import React, { Component } from "react";
import { Category, Podcast, PodcastEpisode } from "./types";
import Navigation from "./components/navigation/Navigation";
import MediaPlayer from "./components/player/MediaPlayer";
import { Slide } from "@material-ui/core";
import VocaliaAPI from "./utility/VocaliaAPI";
import PodcastBrowser from "./components/browse/PodcastBrowser";
import "./App.css";
import { isMobile } from "./utility/DeviceUtils";
import { Route } from "react-router";

/**
 * State information for the application.
 */
interface IAppState {
  podcastData: { [key: string]: Podcast[] };
  categories: Category[];
  selectedEpisode: PodcastEpisode;
  isMobile: boolean;
}

/**
 * Required properties for the application.
 */
interface IAppProps {}

/**
 * UI entry point into the application, handles routing and player interaction.
 */
class App extends Component<IAppProps, IAppState> {
  displayName = App.name;

  constructor(props: IAppProps) {
    super(props);

    this.state = {
      podcastData: { top: [] },
      categories: [],
      selectedEpisode: { time: 0 } as PodcastEpisode,
      isMobile: false
    };
  }

  /**
   * Called after the component has mounted, and sets a resize event listener for state updates.
   */
  componentDidMount() {
    this.updatePredicate();
    window.addEventListener("resize", this.updatePredicate);
  }

  /**
   * Called when the component is unloaded, and removes a resize event listener for state updates.
   */
  componentWillUnmount() {
    window.removeEventListener("resize", this.updatePredicate);
  }

  /**
   * Called before the component finishes mounting,
   * and loads all categories and podcasts into memory.
   */
  componentWillMount() {
    var loader = new VocaliaAPI();

    //Load category list and category data asynchronously.
    (async () => {
      let categories = await loader.getCategories();
      this.setState({ categories: categories });

      categories.forEach(async category => {
        let id = category.id;
        console.log(category);
        let podcasts = await loader.getPodcastByCategory(id);

        let loadedPodcast = this.state.podcastData;
        loadedPodcast[id] = podcasts;
        this.setState({ podcastData: loadedPodcast });
      });
    })();

    //Load top podcast data asynchronously.
    (async () => {
      let podcasts = await loader.getTopPodcasts();

      let loadedPodcasts = this.state.podcastData;
      loadedPodcasts["top"] = podcasts;
      this.setState({ podcastData: loadedPodcasts });
    })();
  }

  /**
   * Checks the screen state of the current device for UI management.
   */
  updatePredicate = () => {
    this.setState({ isMobile: isMobile() });
  };

  /**
   * Called when an episode has been selected for playback.
   */
  onEpisodeSelected = (episode: PodcastEpisode) => {
    this.setState({ selectedEpisode: episode });
  };

  render() {
    const { podcastData, selectedEpisode, isMobile, categories } = this.state;

    /**
     * Elements that can be routed to.
     */
    const RoutingContents = (
      <React.Fragment>
        <Route
          exact
          path="/top"
          render={() => (
            <PodcastBrowser
              isMobile={isMobile}
              selectedEpisode={selectedEpisode}
              podcasts={podcastData["top"]}
              onEpisodeSelected={this.onEpisodeSelected}
            />
          )}
        />

        <Route
          exact
          path="/browse/:id"
          render={props => (
            <div>
              <PodcastBrowser
                isMobile={isMobile}
                selectedEpisode={selectedEpisode}
                podcasts={podcastData[props.match.params.id]}
                onEpisodeSelected={this.onEpisodeSelected}
              />
            </div>
          )}
        />
      </React.Fragment>
    );

    return (
      <Navigation categories={categories} isMobile={isMobile}>
        <React.Fragment>
          {RoutingContents}
          {selectedEpisode.link != null && (
            <Slide direction={"up"} in={selectedEpisode.link != null}>
              <MediaPlayer media={selectedEpisode} isMobile={isMobile} />
            </Slide>
          )}
        </React.Fragment>
      </Navigation>
    );
  }
}

export default App;
