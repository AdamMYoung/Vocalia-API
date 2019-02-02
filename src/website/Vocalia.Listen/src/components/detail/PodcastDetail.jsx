import React, { Component } from "react";
import Dialog from "@material-ui/core/Dialog";
import { withMobileDialog } from "@material-ui/core";

class PodcastDetail extends Component {
  render() {
    return (
      <Dialog
        open={this.props.open}
        onClose={this.props.onClose}
        fullScreen={this.props.fullScreen}
      >
        <p>{this.props.rssUrl}</p>
      </Dialog>
    );
  }
}

export default withMobileDialog()(PodcastDetail);
