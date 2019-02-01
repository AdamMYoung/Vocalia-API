import React, { Component } from "react";
import PropTypes from "prop-types";
import { withStyles } from "@material-ui/core/styles";
import Card from "@material-ui/core/Card";
import { CardMedia, CardContent, Typography } from "@material-ui/core";

const styles = theme => ({
  paper: {
    [theme.breakpoints.down("sm")]: {
      height: 100,
      width: 100,
      margin: 4
    },
    [theme.breakpoints.up("lg")]: {
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
      data: this.props.data
    };
  }

  render() {
    const { classes, data } = this.props;
    return (
      <Card className={classes.paper}>
        <img src={data.imageUrl} alt={data.title} />
      </Card>
    );
  }
}

PodcastEntry.propTypes = {
  classes: PropTypes.object.isRequired
};

export default withStyles(styles)(PodcastEntry);
