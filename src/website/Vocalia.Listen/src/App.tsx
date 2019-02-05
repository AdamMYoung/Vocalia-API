import React, { Component } from "react";
import { Category, Podcast, PodcastEpisode } from "./types";
import Navigation from "./components/navigation/Navigation";
import MediaPlayer from "./components/player/MediaPlayer";
import { Slide } from "@material-ui/core";
import VocaliaAPI from "./utility/VocaliaAPI";
import PodcastBrowser from "./components/browse/PodcastBrowser";
import "./App.css";
import { Route } from "react-router";

interface IAppState {
  podcastData: { [key: string]: Podcast[] };
  categories: Category[];
  selectedEpisode: PodcastEpisode;
}

interface IAppProps {}

class App extends Component<IAppProps, IAppState> {
  displayName = App.name;

  constructor(props: any) {
    super(props);

    this.state = {
      podcastData: { top: [] },
      categories: [],
      selectedEpisode: { time: 0 } as PodcastEpisode
    };
  }

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

  Player = () => {
    return <MediaPlayer media={this.state.selectedEpisode} />;
  };

  onEpisodeSelected = (episode: PodcastEpisode) => {
    this.setState({ selectedEpisode: episode });
  };

  render() {
    const { podcastData, selectedEpisode } = this.state;

    return (
      <Navigation categories={this.state.categories}>
        <React.Fragment>
          <PodcastBrowser
            podcasts={podcastData["top"]}
            onEpisodeSelected={this.onEpisodeSelected}
          />
          {selectedEpisode.link != null && <this.Player />}
        </React.Fragment>
      </Navigation>
    );
  }
}

export default App;
