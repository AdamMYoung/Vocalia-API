import React, { Component } from "react";
import { Container, Row, Col } from "react-bootstrap";
import NavMenu from "./navigation/NavMenu";
import MaterialPlayer from "./player/material-player";

class Layout extends Component {
  render() {
    return (
      <Container className="noPadding noMargin" fluid>
        <Row className="noPadding noMargin">
          <Col className="noPadding noMargin">
            <NavMenu />
          </Col>
        </Row>
        <Row className="noPadding noMargin">
          <Col className="noPadding noMargin">{this.props.children}</Col>
        </Row>
        <Row className="noPadding noMargin">
          <Col className="noPadding noMargin">
            <MaterialPlayer />
          </Col>
        </Row>
      </Container>
    );
  }
}

export default Layout;
