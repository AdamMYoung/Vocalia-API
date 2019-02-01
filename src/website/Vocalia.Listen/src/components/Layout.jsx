import React, { Component } from "react";
import { Container, Row, Col } from "react-bootstrap";
import NavMenu from "./navigation/NavMenu";
import MaterialPlayer from "./player/material-player";

class Layout extends Component {
  render() {
    return (
      <NavMenu>
        {this.props.children}
        <MaterialPlayer src="test" />
      </NavMenu>
    );
  }
}

export default Layout;
