import React, { Component } from "react";
import PropTypes from "prop-types";
import { withStyles } from "@material-ui/core/styles";
import Grid from "@material-ui/core/Grid";
import PodcastEntry from "./PodcastEntry";
import Fade from "@material-ui/core/Fade";

const API = "http://localhost:54578/podcast/";
const TOP = "top";

class CategoryBrowsePodcasts extends Component {
  constructor(props) {
    super(props);

    this.state = {
      podcasts: [],
      categoryId: this.props.match.params.id,
      loading: false
    };
  }

  componentDidMount() {
    this.loadPodcasts();
    console.log(this.state.categoryId);
  }

  componentWillReceiveProps(newProps) {
    this.setState({ categoryId: newProps.match.params.id });
    this.loadPodcasts();
  }

  loadPodcasts() {
    this.setState({ loading: true });
    fetch(API + TOP + "?categoryId=" + this.state.categoryId)
      .then(response => response.json())
      .then(data => this.setState({ podcasts: data, loading: false }));
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

CategoryBrowsePodcasts.propTypes = {
  classes: PropTypes.object.isRequired
};

export default withStyles()(CategoryBrowsePodcasts);
