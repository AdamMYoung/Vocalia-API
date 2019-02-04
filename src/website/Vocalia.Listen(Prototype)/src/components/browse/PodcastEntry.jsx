import React, { Component } from "react";
import { withStyles } from "@material-ui/core/styles";
import Card from "@material-ui/core/Card";
import "./Entry.css";

const styles = theme => ({
  paper: {
    [theme.breakpoints.down("sm")]: {
      height: 100,
      width: 100,
      margin: 4
    },
    [theme.breakpoints.up("md")]: {
      height: 160,
      width: 160,
      margin: 4
    }
  }
});

/**
 * React object for a podcast entry. Contains an RSS url and image for display within the PodcastBrowse component.
 */
class PodcastEntry extends Component {
  /**
   * Called when the entry is clicked.
   */
  onClick(rss, img) {
    this.props.onClick(rss, img);
  }

  render() {
    const { classes, data } = this.props;
    return (
      <Card
        className={classes.paper + " card"}
        onClick={() => this.onClick(data.rssUrl, data.imageUrl)}
      >
        <img src={data.imageUrl} alt={data.title} />
      </Card>
    );
  }
}

export default withStyles(styles)(PodcastEntry);
