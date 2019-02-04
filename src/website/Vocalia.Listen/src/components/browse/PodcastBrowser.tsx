import React, { Component } from "react";
import Grid from "@material-ui/core/Grid";
import Card from "@material-ui/core/Card";
import {
  createStyles,
  Theme,
  withStyles,
  WithStyles
} from "@material-ui/core/styles";
import { Podcast, PodcastEpisode } from "../../types";

interface IBrowserState {}

interface IBrowserProps extends WithStyles<typeof styles> {
  podcasts: Podcast[];
  onEpisodeSelected: (episode: PodcastEpisode) => void;
}

const styles = (theme: Theme) =>
  createStyles({
    paper: {
      [theme.breakpoints.down("sm")]: {
        height: 100,
        width: 100,
        margin: 4
      },
      [theme.breakpoints.up("md")]: {
        height: 140,
        width: 140,
        margin: 4
      }
    }
  });

function Entry(props: any) {
  const { classes, podcast } = props;
  return (
    <Card className={classes.paper + " card"}>
      <img
        src={podcast.imageUrl}
        alt={podcast.title}
        style={{ width: "100%", height: "100%" }}
      />
    </Card>
  );
}

class PodcastBrowser extends Component<IBrowserProps, IBrowserState> {
  render() {
    const { podcasts, classes } = this.props;

    return (
      <Grid container justify="space-evenly">
        {podcasts.map(podcast => (
          <Entry key={podcast.id} podcast={podcast} classes={classes} />
        ))}
      </Grid>
    );
  }
}

export default withStyles(styles)(PodcastBrowser);
