import React, { Component } from "react";
import PropTypes from "prop-types";
import { withStyles } from "@material-ui/core/styles";
import Grid from "@material-ui/core/Grid";
import PodcastEntry from "./PodcastEntry";

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
      podcasts: []
    };
  }

  componentDidMount() {
    fetch(API + TOP)
      .then(response => response.json())
      .then(data => this.setState({ podcasts: data }));
  }

  render() {
    const { classes } = this.props;

    return (
      <Grid container>
        <Grid item xs={12}>
          <Grid container justify="center" spacing={Number(16)}>
            {this.state.podcasts.map(podcast => (
              <PodcastEntry data={podcast} />
            ))}
          </Grid>
        </Grid>
      </Grid>
    );
  }
}

TopPodcastsBrowse.propTypes = {
  classes: PropTypes.object.isRequired
};

export default withStyles(styles)(TopPodcastsBrowse);
