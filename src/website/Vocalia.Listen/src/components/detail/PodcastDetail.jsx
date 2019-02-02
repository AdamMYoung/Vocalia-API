import React, { Component } from "react";
import Dialog from "@material-ui/core/Dialog";
import { withMobileDialog } from "@material-ui/core";
import DialogActions from "@material-ui/core/DialogActions";
import DialogContent from "@material-ui/core/DialogContent";
import DialogContentText from "@material-ui/core/DialogContentText";
import DialogTitle from "@material-ui/core/DialogTitle";
import Button from "@material-ui/core/Button";
let Parser = require("rss-parser");

class PodcastDetail extends Component {
  constructor(props) {
    super(props);

    this.state = {
      feed: null
    };
  }

  render() {
    function loadDialog(props, state) {
      return (
        <Dialog
          open={props.open}
          onClose={props.onClose}
          fullScreen={props.fullScreen}
        >
          <DialogTitle id="responsive-dialog-title">Title</DialogTitle>
          <DialogContent>
            <DialogContentText>
              This is where the description will go.
            </DialogContentText>
          </DialogContent>
          <DialogActions>
            <Button onClick={props.onClose} color="primary" autoFocus>
              Close
            </Button>
          </DialogActions>
        </Dialog>
      );
    }

    return loadDialog(this.props, this.state);
  }
}

export default withMobileDialog()(PodcastDetail);
