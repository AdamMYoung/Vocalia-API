import React, { Component } from "react";
import Grid from "@material-ui/core/Grid";
import Card from "@material-ui/core/Card";
import {
  createStyles,
  Theme,
  withStyles,
  WithStyles
} from "@material-ui/core/styles";
import { Podcast } from "../../types";
import { LinkContainer } from "react-router-bootstrap";
import "../detail/PodcastEntry.css";
import { Link } from "@material-ui/core";

/**
 * Properties passed into the browser.
 */
interface IBrowserProps extends WithStyles<typeof styles> {
  podcasts: Podcast[]; //Podcast entries to display.
}

/**
 * State of the browser.
 */
interface IBrowserState {}

/**
 * Properties for a podcast entry.
 */
interface IEntryProps extends WithStyles<typeof styles> {
  podcast: Podcast;
}

/**
 * CSS Styles of the browser
 */
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

/**
 * Single podcast entry for display in the browser.
 * @param props Properties belonging to the podcast entry.
 */
function Entry(props: IEntryProps) {
  const { classes, podcast } = props;

  return (
    <LinkContainer to={"detail/" + encodeURIComponent(podcast.rssUrl)}>
      <Card className={classes.paper + " card"}>
        <img
          src={podcast.imageUrl}
          alt={podcast.title}
          style={{ width: "100%", height: "100%" }}
        />
      </Card>
    </LinkContainer>
  );
}

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
    const { podcasts, classes } = this.props;

    return (
      <React.Fragment>
        <Grid container justify="space-evenly">
          {podcasts != null &&
            podcasts.map(podcast => (
              <Entry key={podcast.rssUrl} podcast={podcast} classes={classes} />
            ))}
        </Grid>
      </React.Fragment>
    );
  }
}

export default withStyles(styles)(PodcastBrowser);
