import React, { Component } from "react";
import NavMenu from "./navigation/NavMenu";
import MaterialPlayer from "./player/material-player";

class Layout extends Component {
  render() {
    return (
      <NavMenu>
        <div>{this.props.children}</div>
        <MaterialPlayer src="" />
      </NavMenu>
    );
  }
}

export default Layout;
