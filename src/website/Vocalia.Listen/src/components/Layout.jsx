import React, { Component } from "react";
import { Container, Row, Col } from "react-bootstrap";
import NavMenu from "./navigation/NavMenu";
import MaterialPlayer from "./player/material-player";

class Layout extends Component {
  render() {
    return (
      <NavMenu>
        <div>
          {this.props.children}
          <MaterialPlayer src="test" />
        </div>
      </NavMenu>
    );
  }
}

export default Layout;
