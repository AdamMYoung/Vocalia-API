import React, { Component } from "react";
import ExpansionPanel from "@material-ui/core/ExpansionPanel";
import ExpansionPanelSummary from "@material-ui/core/ExpansionPanelSummary";
import ExpansionPanelDetails from "@material-ui/core/ExpansionPanelDetails";
import ExpandMoreIcon from "@material-ui/icons/ExpandMore";
import PlayArrowIcon from "@material-ui/icons/PlayArrow";
import Typography from "@material-ui/core/Typography";
import IconButton from "@material-ui/core/IconButton";
import { removeTags } from "../../utils/TextUtils.js";

const styles = {
  button: {
    width: 32,
    height: 32,
    padding: 0
  },
  icon: {
    fontSize: 40,
    color: "#fffff"
  },
  tooltip: {
    marginLeft: 7
  },
  content: {
    lineHeight: 600
  }
};

/**
 * Episode entry to provide playback, title and description for the element.
 * Prop List:
 * episode:
 */
class EpisodeEntry extends Component {
  constructor(props) {
    super(props);

    this.state = {
      episode: {}
    };
  }

  componentWillMount() {
    this.setState({ episode: this.props.episode });
  }

  render() {
    const { episode } = this.state;

    return (
      <ExpansionPanel key={episode.guid}>
        <ExpansionPanelSummary expandIcon={<ExpandMoreIcon />}>
          <IconButton
            style={styles.button}
            iconStyle={styles.icon}
            tooltipStyles={styles.tooltip}
            onClick={this.props.onPlay}
          >
            <PlayArrowIcon />
          </IconButton>
          <Typography style={{ marginTop: "7px", marginLeft: "3px" }}>
            {episode.title}
          </Typography>
        </ExpansionPanelSummary>
        <ExpansionPanelDetails>
          <Typography>{removeTags(episode.content)}</Typography>
        </ExpansionPanelDetails>
      </ExpansionPanel>
    );
  }
}

export default EpisodeEntry;
