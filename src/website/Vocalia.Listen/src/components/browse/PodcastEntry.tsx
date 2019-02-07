import React, { Component } from "react";
import { Podcast } from "../../types";
import { LinkContainer } from "react-router-bootstrap";
import Card from "@material-ui/core/Card";
import "./PodcastEntry.css";
import {
  createStyles,
  Theme,
  withStyles,
  WithStyles,
  Fade
} from "@material-ui/core";

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
 * Properties for a podcast entry.
 */
interface IEntryProps extends WithStyles<typeof styles> {
  podcast: Podcast;
}

interface IEntryState {
  isLoaded: boolean;
}

/**
 * Single podcast entry for display in the browser.
 * @param props Properties belonging to the podcast entry.
 */
class PodcastEntry extends Component<IEntryProps, IEntryState> {
  constructor(props: IEntryProps) {
    super(props);

    this.state = {
      isLoaded: false
    };
  }

  render() {
    const { classes, podcast } = this.props;
    const { isLoaded } = this.state;

    return (
      <LinkContainer to={"/detail/" + encodeURIComponent(podcast.rssUrl)}>
        <Card className={classes.paper + " card"}>
          <Fade in={isLoaded} timeout={300}>
            <img
              src={podcast.imageUrl}
              alt={podcast.title}
              onLoad={() => this.setState({ isLoaded: true })}
              style={{ width: "100%", height: "100%" }}
            />
          </Fade>
        </Card>
      </LinkContainer>
    );
  }
}

export default withStyles(styles)(PodcastEntry);