import React, { Component } from "react";
import ExpansionPanel from "@material-ui/core/ExpansionPanel";
import ExpansionPanelSummary from "@material-ui/core/ExpansionPanelSummary";
import ExpansionPanelDetails from "@material-ui/core/ExpansionPanelDetails";
import ExpandMoreIcon from "@material-ui/icons/ExpandMore";
import PlayArrowIcon from "@material-ui/icons/PlayArrow";
import StopIcon from "@material-ui/icons/Stop";
import Typography from "@material-ui/core/Typography";
import IconButton from "@material-ui/core/IconButton";
import { removeTags } from "../../utility/FormatUtils";
import { PodcastEpisode } from "../../types";

/**
 * CSS styles for the entry.
 */
const styles = {
  button: {
    width: 32,
    height: 32,
    padding: 0
  }
};

/**
 * Required properties for the episode entry.
 */
interface IEpisodeProps {
  episode: PodcastEpisode;
  selectedEpisode: PodcastEpisode;
  onEpisodeSelected: (episode: PodcastEpisode) => void;
}

/**
 * State information for the episode entry.
 */
interface IEpisodeState {}

/**
 * Contains title, description and details for a specific episode item.
 */
class EpisodeEntry extends Component<IEpisodeProps, IEpisodeState> {
  /**
   * Called when an episode is selected, either setting the episode to null
   *  or the selected episode depending on what is currently playing.
   */
  onEpisodeSelect = () => {
    const { episode, onEpisodeSelected, selectedEpisode } = this.props;

    let selectedItem =
      episode.content == selectedEpisode.content
        ? ({ time: 0 } as PodcastEpisode)
        : episode;
    onEpisodeSelected(selectedItem);
  };

  render() {
    const { episode, selectedEpisode } = this.props;

    //Toggles between a stop button or play button depending if the
    // current episode matches the object being represented.
    let icon =
      episode.content == selectedEpisode.content ? (
        <StopIcon />
      ) : (
        <PlayArrowIcon />
      );

    return (
      <ExpansionPanel>
        <ExpansionPanelSummary expandIcon={<ExpandMoreIcon />}>
          <IconButton
            style={styles.button}
            onClick={e => {
              this.onEpisodeSelect();
              e.stopPropagation();
            }}
          >
            {icon}
          </IconButton>
          <Typography style={{ marginTop: "7px", marginLeft: "3px" }}>
            {episode.title}
          </Typography>
        </ExpansionPanelSummary>
        <ExpansionPanelDetails>
          <Typography>{removeTags(episode.description)}</Typography>
        </ExpansionPanelDetails>
      </ExpansionPanel>
    );
  }
}

export default EpisodeEntry;
