import React, { Component } from "react";
import Grid from "@material-ui/core/Grid";
import { Podcast } from "../../types";
import PodcastEntry from "./PodcastEntry";

/**
 * Properties passed into the browser.
 */
interface IBrowserProps {
  podcasts: Podcast[]; //Podcast entries to display.
}

/**
 * State of the browser.
 */
interface IBrowserState {}

/**
 * Component to display and interact with podcasts passed into it.
 */
class PodcastBrowser extends Component<IBrowserProps, IBrowserState> {
  constructor(props: IBrowserProps) {
    super(props);

    this.state = {
      dialogOpen: false,
      selectedPodcast: {} as Podcast
    };
  }

  /**
   * Called when a podcast is clicked.
   */
  onPodcastClick = (podcast: Podcast) => {
    this.setState({ selectedPodcast: podcast, dialogOpen: true });
  };

  render() {
    const { podcasts } = this.props;

    return (
      <React.Fragment>
        <Grid container justify="space-evenly">
          {podcasts != null &&
            podcasts.map(podcast => (
              <PodcastEntry key={podcast.rssUrl} podcast={podcast} />
            ))}
        </Grid>
      </React.Fragment>
    );
  }
}

export default PodcastBrowser;
