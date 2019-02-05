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

interface IAppState {
  podcastData: { [key: string]: Podcast[] };
  categories: Category[];
  selectedEpisode: PodcastEpisode;
  isMobile: boolean;
}

interface IAppProps {}

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
   * Called after the component has mounted.
   */
  componentDidMount() {
    this.updatePredicate();
    window.addEventListener("resize", this.updatePredicate);
  }

  /**
   * Called when the component is unloaded.
   */
  componentWillUnmount() {
    window.removeEventListener("resize", this.updatePredicate);
  }

  /**
   * Called before the component finishes mounting.
   */
  componentWillMount() {
    var loader = new VocaliaAPI();

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

    (async () => {
      let podcasts = await loader.getTopPodcasts();

      let loadedPodcasts = this.state.podcastData;
      loadedPodcasts["top"] = podcasts;
      this.setState({ podcastData: loadedPodcasts });
    })();
  }

  updatePredicate = () => {
    this.setState({ isMobile: isMobile() });
  };

  onEpisodeSelected = (episode: PodcastEpisode) => {
    this.setState({ selectedEpisode: episode });
  };

  render() {
    const { podcastData, selectedEpisode, isMobile, categories } = this.state;

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
            <MediaPlayer media={selectedEpisode} isMobile={isMobile} />
          )}
        </React.Fragment>
      </Navigation>
    );
  }
}

export default App;
