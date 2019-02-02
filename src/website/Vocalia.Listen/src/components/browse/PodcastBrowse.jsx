import React, { Component } from "react";
import Grid from "@material-ui/core/Grid";
import Fade from "@material-ui/core/Fade";
import PodcastEntry from "./PodcastEntry";
import LoadingEntry from "./LoadingEntry";
import PodcastDetail from "../detail/PodcastDetail";

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

  componentDidMount = () => {
    this.loadPodcasts();
  };

  componentWillReceiveProps = newProps => {
    this.setState({ category: newProps.match.params.id });
    this.loadPodcasts();
  };

  loadPodcasts = () => {
    this.setState({ loading: true });

    fetch(this.buildQueryString())
      .then(response => response.json())
      .then(data => this.setState({ podcasts: data, loading: false }));
  };

  buildQueryString = () => {
    var cat = this.state.category;

    if (isNaN(cat)) {
      switch (cat) {
        case "top":
          return API + TOP;
        default:
          return API + TOP;
      }
    }

    return API + TOP + "?categoryId=" + this.state.category;
  };

  podcastClickCallback = rss => {
    this.setState({ rssUrl: rss, dialogOpen: true });
  };

  handleClose = () => {
    this.setState({ dialogOpen: false });
  };

  render() {
    const { loading, dialogOpen, rssUrl, podcasts } = this.state;

    return (
      <div>
        <PodcastDetail
          rssUrl={rssUrl}
          open={dialogOpen}
          onClose={this.handleClose}
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
              .map(() => (
                <LoadingEntry key={this} />
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
                clickCallback={thisRef.podcastClickCallback}
              />
            ))}
          </Grid>
        </Fade>
      );
    }
  }
}

export default PodcastBrowse;
