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
  }
};

/**
 * Episode entry to provide playback, title and description for the element.
 * Prop List:
 * episode:
 */
class EpisodeEntry extends Component {
  render() {
    const { episode } = this.props;

    const array = {};
    array["src"] = episode.enclosure["url"];
    array["episode"] = episode.title;

    return (
      <ExpansionPanel key={episode.guid}>
        <ExpansionPanelSummary expandIcon={<ExpandMoreIcon />}>
          <IconButton
            style={styles.button}
            onClick={e => {
              this.props.onClick(array);
              e.stopPropagation();
            }}
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
