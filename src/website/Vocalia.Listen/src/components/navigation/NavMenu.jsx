import React, { Component } from "react";
import "./NavMenu.css";
import { LinkContainer } from "react-router-bootstrap";
import AppBar from "@material-ui/core/AppBar";
import Toolbar from "@material-ui/core/Toolbar";
import Typography from "@material-ui/core/Typography";
import Button from "@material-ui/core/Button";
import IconButton from "@material-ui/core/IconButton";
import MenuIcon from "@material-ui/icons/Menu";

class NavMenu extends Component {
  state = {
    mobileOpen: false
  };

  handleDrawerToggle = () => {
    this.setState(state => ({ mobileOpen: !state.mobileOpen }));
  };

  render() {
    return (
      <div>
        <AppBar position="static">
          <Toolbar>
            <IconButton color="inherit" aria-label="Menu">
              <MenuIcon onClick={this.handleDrawerToggle} />
            </IconButton>
            <Typography variant="h5" color="inherit">
              Vocalia
            </Typography>

            <Button color="inherit">Login</Button>
          </Toolbar>
        </AppBar>
        https://material-ui.com/demos/drawers/
      </div>
    );
  }
}

export default NavMenu;
