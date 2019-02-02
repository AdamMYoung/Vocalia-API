import React, { Component } from "react";
import NavMenu from "./navigation/NavMenu";
import MaterialPlayer from "./player/material-player";
import Grid from "@material-ui/core/Grid";
import PropTypes from "prop-types";
import { withStyles } from "@material-ui/core/styles";

const styles = theme => ({
  margin: {
    [theme.breakpoints.down("sm")]: {
      marginBottom: 110
    },
    [theme.breakpoints.up("md")]: {
      paddingBottom: 70
    }
  }
});

class Layout extends Component {
  render() {
    const { classes, theme } = this.props;

    return (
      <NavMenu>
        <Grid container>
          <Grid id="content" item xs={12}>
            {this.props.children}
          </Grid>
          <Grid id="player" item xs={12} className={classes.margin}>
            <MaterialPlayer src="" />
          </Grid>
        </Grid>
      </NavMenu>
    );
  }
}

NavMenu.propTypes = {
  container: PropTypes.object,
  theme: PropTypes.object.isRequired
};

export default withStyles(styles)(Layout);
