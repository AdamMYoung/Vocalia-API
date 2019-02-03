import React, { Component } from "react";
import Dialog from "@material-ui/core/Dialog";
import { withMobileDialog } from "@material-ui/core";
import DialogActions from "@material-ui/core/DialogActions";
import DialogContent from "@material-ui/core/DialogContent";
import DialogContentText from "@material-ui/core/DialogContentText";
import DialogTitle from "@material-ui/core/DialogTitle";
import Button from "@material-ui/core/Button";
import Fade from "@material-ui/core/Fade";

let Parser = require("rss-parser");

const CORS_PROXY = "https://cors-anywhere.herokuapp.com/";

class PodcastDetail extends Component {
  constructor(props) {
    super(props);

    this.state = {
      feed: {}
    };
  }

  componentWillReceiveProps(props) {
    if (props.rssUrl !== null) {
      let parser = new Parser();
      this.setState({ loading: true, feed: {} });

      (async () => {
        let parsedFeed = await parser.parseURL(CORS_PROXY + props.rssUrl);
        this.setState({ feed: parsedFeed, loading: false });
      })();
    }
  }

  render() {
    const { feed, loading } = this.state;

    return (
      <Dialog
        open={this.props.open}
        onClose={this.props.onClose}
        fullScreen={this.props.fullScreen}
      >
        {!loading && (
          <Fade in={!loading}>
            <div>
              <DialogTitle>{feed.title}</DialogTitle>
              <DialogContent>
                <DialogContentText>{feed.description}</DialogContentText>
              </DialogContent>
              <DialogActions>
                <Button onClick={this.props.onClose} color="primary" autoFocus>
                  Close
                </Button>
              </DialogActions>
            </div>
          </Fade>
        )}
      </Dialog>
    );
  }
}

export default withMobileDialog()(PodcastDetail);
