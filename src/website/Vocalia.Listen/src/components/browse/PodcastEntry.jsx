import React, { Component } from "react";
import PropTypes from "prop-types";
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

class PodcastEntry extends Component {
  constructor(props) {
    super(props);

    this.state = {
      data: this.props.data,
      callback: this.props.clickCallback
    };
  }

  cardClicked(rss) {
    this.state.callback(rss);
  }

  render() {
    const { classes, data } = this.props;
    return (
      <Card
        className={classes.paper + " card"}
        onClick={() => this.cardClicked(data.rssUrl)}
      >
        <img src={data.imageUrl} alt={data.title} />
      </Card>
    );
  }
}

PodcastEntry.propTypes = {
  classes: PropTypes.object.isRequired
};

export default withStyles(styles)(PodcastEntry);
