import React, { Component } from "react";
import PropTypes from "prop-types";
import Grid from "@material-ui/core/Grid";
import PodcastEntry from "./PodcastEntry";
import Fade from "@material-ui/core/Fade";

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

  componentDidMount() {
    this.loadPodcasts();
    console.log(this.state.category);
  }

  componentWillReceiveProps(newProps) {
    this.setState({ category: newProps.match.params.id });
    this.loadPodcasts();
  }

  loadPodcasts() {
    this.setState({ loading: true });

    fetch(this.buildQueryString())
      .then(response => response.json())
      .then(data => this.setState({ podcasts: data, loading: false }));
  }

  buildQueryString() {
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
  }

  render() {
    const { loading } = this.state;

    return (
      <div>
        <Fade in={!loading}>
          <Grid container>
            <Grid item xs={12}>
              <Grid container justify="space-evenly">
                {this.state.podcasts.map(podcast => (
                  <PodcastEntry data={podcast} key={podcast.title} />
                ))}
              </Grid>
            </Grid>
          </Grid>
        </Fade>
      </div>
    );
  }
}

PodcastBrowse.propTypes = {
  classes: PropTypes.object.isRequired
};

export default PodcastBrowse;
