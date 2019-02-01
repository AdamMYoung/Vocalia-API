import React, { Component } from "react";
import PropTypes from "prop-types";
import { withStyles } from "@material-ui/core/styles";
import Grid from "@material-ui/core/Grid";
import PodcastEntry from "./PodcastEntry";
import Fade from "@material-ui/core/Fade";

const API = "http://localhost:54578/podcast/";
const TOP = "top";

const styles = theme => ({
  root: {
    flexGrow: 1
  }
});

class TopPodcastsBrowse extends Component {
  constructor(props) {
    super(props);

    this.state = {
      podcasts: [],
      loading: false
    };
  }

  componentDidMount() {
    this.setState({ loading: true });
    fetch(API + TOP)
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

TopPodcastsBrowse.propTypes = {
  classes: PropTypes.object.isRequired
};

export default withStyles(styles)(TopPodcastsBrowse);
