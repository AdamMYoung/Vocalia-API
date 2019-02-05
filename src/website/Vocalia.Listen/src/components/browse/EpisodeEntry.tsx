import React, { Component } from "react";
import ExpansionPanel from "@material-ui/core/ExpansionPanel";
import ExpansionPanelSummary from "@material-ui/core/ExpansionPanelSummary";
import ExpansionPanelDetails from "@material-ui/core/ExpansionPanelDetails";
import ExpandMoreIcon from "@material-ui/icons/ExpandMore";
import PlayArrowIcon from "@material-ui/icons/PlayArrow";
import PauseIcon from "@material-ui/icons/Pause";
import Typography from "@material-ui/core/Typography";
import IconButton from "@material-ui/core/IconButton";
import { removeTags } from "../../utility/FormatUtils";
import { PodcastEpisode } from "../../types";

interface IState {}

interface IProps {
  episode: PodcastEpisode;
  selectedEpisode: PodcastEpisode;
  onEpisodeSelected: (episode: PodcastEpisode) => void;
}

const styles = {
  button: {
    width: 32,
    height: 32,
    padding: 0
  }
};

class EpisodeEntry extends Component<IProps, IState> {
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

    let icon =
      episode.content == selectedEpisode.content ? (
        <PauseIcon />
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
