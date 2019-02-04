import React, { Component } from "react";
import Grid from "@material-ui/core/Grid";
import Fade from "@material-ui/core/Fade";
import PodcastEntry from "./PodcastEntry";
import LoadingEntry from "./LoadingEntry";
import PodcastDetail from "../detail/PodcastDetail";
import PubSub from "pubsub-js";

const API = "http://localhost:54578/podcast/";
const TOP = "top";

class PodcastBrowse extends Component {
  constructor(props) {
    super(props);

    this.state = {
      podcasts: [],
      category: this.props.match.params.id,
      loading: false
    };
  }

  /**
   * Loads the podcasts when the component mounts.
   */
  componentDidMount = () => {
    this.loadPodcasts();
  };

  /**
   * Called when the category parameters change, updating the state with the new properties.
   */
  componentWillReceiveProps = newProps => {
    this.setState({ category: newProps.match.params.id });
    this.loadPodcasts();
  };

  /**
   * Queries the Vocalia API for podcasts.
   */
  loadPodcasts = () => {
    const { category } = this.state;
    this.setState({ loading: true });

    fetch(this.buildQueryString(category))
      .then(response => response.json())
      .then(data => this.setState({ podcasts: data, loading: false }));
  };

  /**
   * Builds a query string from the passed category.
   */
  buildQueryString = category => {
    if (isNaN(category)) {
      switch (category) {
        case "top":
          return API + TOP;
        default:
          return API + TOP;
      }
    }

    return API + TOP + "?categoryId=" + category;
  };

  /**
   * Called when a podcast is clicked.
   */
  onPodcastClick = (rss, imgUrl) => {
    console.log(imgUrl);
    this.setState({ rssUrl: rss, imgUrl: imgUrl, dialogOpen: true });
  };

  /**
   * Closes the current dialog box.
   */
  handleClose = () => {
    this.setState({ dialogOpen: false });
  };

  /**
   * Called when an episode is selected to play.
   */
  onPlay = data => {
    const { imgUrl } = this.state;
    data["img"] = imgUrl;
    PubSub.publish("newAudio", data);
  };

  render() {
    const { loading, dialogOpen, rssUrl, podcasts } = this.state;

    return (
      <div>
        <PodcastDetail
          rssUrl={rssUrl}
          open={dialogOpen}
          onClose={this.handleClose}
          onPlay={this.onPlay}
        />
        <Grid container>
          <Grid item xs={12}>
            {loading && renderPlaceholderCards()}

            {!loading && renderPodcastCards(podcasts, this)}
          </Grid>
        </Grid>
      </div>
    );

    function renderPlaceholderCards() {
      return (
        <Fade in={loading}>
          <Grid container justify="space-evenly">
            {Array.apply(null, { length: 100 })
              .map(Function.call, Number)
              .map(num => (
                <LoadingEntry key={num} />
              ))}
          </Grid>
        </Fade>
      );
    }

    function renderPodcastCards(podcasts, thisRef) {
      return (
        <Fade in={!loading}>
          <Grid container justify="space-evenly">
            {podcasts.map(podcast => (
              <PodcastEntry
                data={podcast}
                key={podcast.title}
                onClick={thisRef.onPodcastClick}
              />
            ))}
          </Grid>
        </Fade>
      );
    }
  }
}

export default PodcastBrowse;
