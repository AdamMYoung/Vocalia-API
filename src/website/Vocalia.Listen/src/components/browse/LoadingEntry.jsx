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

class LoadingEntry extends Component {
  render() {
    const { classes } = this.props;
    return <Card className={classes.paper + " loadingCard"} />;
  }
}

LoadingEntry.propTypes = {
  classes: PropTypes.object.isRequired
};

export default withStyles(styles)(LoadingEntry);
