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
    })();

    (async () => {
      let loadedPodcasts = this.state.podcastData;
      loadedPodcasts["top"] = await loader.getTopPodcasts();
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

    return (
      <Navigation categories={categories} isMobile={isMobile}>
        <React.Fragment>
          <PodcastBrowser
            selectedEpisode={selectedEpisode}
            podcasts={podcastData["top"]}
            onEpisodeSelected={this.onEpisodeSelected}
          />
          {selectedEpisode.link != null && (
            <MediaPlayer media={selectedEpisode} isMobile={isMobile} />
          )}
        </React.Fragment>
      </Navigation>
    );
  }
}

export default App;
