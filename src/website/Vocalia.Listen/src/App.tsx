import React, { Component } from "react";
import { Category, Podcast, PodcastEpisode } from "./types";
import Navigation from "./components/navigation/Navigation";
import MediaPlayer from "./components/player/MediaPlayer";
import { Grid } from "@material-ui/core";
import VocaliaAPI from "./utility/VocaliaAPI";
import PodcastBrowser from "./components/browse/PodcastBrowser";
import "./App.css";
import { Route } from "react-router";

interface IAppState {
  podcastData: { [key: string]: Podcast[] };
  categories: Category[];
}

interface IAppProps {}

class App extends Component<IAppProps, IAppState> {
  displayName = App.name;

  constructor(props: any) {
    super(props);

    this.state = {
      podcastData: { top: [] },
      categories: []
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
      //loadedPodcasts["subscribed"] = await loader.getSubscribedPodcasts();
      this.setState({ podcastData: loadedPodcasts });
    })();
  }

  onEpisodeSelected = (episode: PodcastEpisode) => {};

  render() {
    const { podcastData } = this.state;

    return (
      <Navigation categories={this.state.categories}>
        <React.Fragment>
          <PodcastBrowser
            podcasts={podcastData["top"]}
            onEpisodeSelected={this.onEpisodeSelected}
          />
          <MediaPlayer />
        </React.Fragment>
      </Navigation>
    );
  }
}

export default App;
