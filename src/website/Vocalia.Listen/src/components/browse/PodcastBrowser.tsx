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
import PodcastDetail from "./PodcastDetail";

interface IBrowserState {
  dialogOpen: boolean;
  selectedPodcast: Podcast;
}

interface IBrowserProps extends WithStyles<typeof styles> {
  podcasts: Podcast[];
  selectedEpisode: PodcastEpisode;
  onEpisodeSelected: (episode: PodcastEpisode) => void;
}

interface IEntryProps extends WithStyles<typeof styles> {
  podcast: Podcast;
  onClick: (podcast: Podcast) => void;
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

function Entry(props: IEntryProps) {
  const { classes, podcast, onClick } = props;
  return (
    <Card className={classes.paper + " card"} onClick={() => onClick(podcast)}>
      <img
        src={podcast.imageUrl}
        alt={podcast.title}
        style={{ width: "100%", height: "100%" }}
      />
    </Card>
  );
}

function Placeholder(props: any) {
  const { classes } = props;
  return <Card className={classes.paper} />;
}

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
    const {
      podcasts,
      classes,
      onEpisodeSelected,
      selectedEpisode
    } = this.props;
    const { dialogOpen, selectedPodcast } = this.state;

    return (
      <React.Fragment>
        <PodcastDetail
          open={dialogOpen}
          podcast={selectedPodcast}
          selectedEpisode={selectedEpisode}
          onClose={() => this.setState({ dialogOpen: false })}
          onEpisodeSelected={episode => onEpisodeSelected(episode)}
        />
        <Grid container justify="space-evenly">
          {podcasts !== null &&
            podcasts.map(podcast => (
              <Entry
                key={podcast.rssUrl}
                podcast={podcast}
                classes={classes}
                onClick={podcast => this.onPodcastClick(podcast)}
              />
            ))}
        </Grid>
      </React.Fragment>
    );
  }
}

export default withStyles(styles)(PodcastBrowser);
