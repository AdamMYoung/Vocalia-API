import React, { Component } from "react";
import zIndex from "@material-ui/core/styles/zIndex";
import Card from "@material-ui/core/Card";
import AppBar from "@material-ui/core/AppBar";

export default class MediaPlayer extends Component {
  render() {
    return (
      <Card
        style={{
          width: "100%",
          height: 80,
          position: "sticky",
          bottom: 0
        }}
      >
        Test
      </Card>
    );
  }
}
