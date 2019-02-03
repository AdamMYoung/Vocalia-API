import React, { Component } from "react";
import Dialog from "@material-ui/core/Dialog";
import { withMobileDialog, Divider } from "@material-ui/core";
import DialogActions from "@material-ui/core/DialogActions";
import DialogContent from "@material-ui/core/DialogContent";
import DialogContentText from "@material-ui/core/DialogContentText";
import DialogTitle from "@material-ui/core/DialogTitle";
import { removeTags } from "../../utils/TextUtils.js";
import EpisodeEntry from "./EpisodeEntry";
import Button from "@material-ui/core/Button";

let Parser = require("rss-parser");

const CORS_PROXY = "https://cors-anywhere.herokuapp.com/";

class PodcastDetail extends Component {
  constructor(props) {
    super(props);

    this.increaseVisibleEpisodes = this.increaseVisibleEpisodes.bind(this);

    this.state = {
      feed: {},
      elementsToShow: 20,
      itemsInFeed: 0
    };
  }

  componentWillReceiveProps(props) {
    let parser = new Parser();
    this.setState({
      loading: true,
      feed: {},
      elementsToShow: 20,
      itemsInFeed: 0
    });

    (async () => {
      let parsedFeed = await parser.parseURL(CORS_PROXY + props.rssUrl);
      this.setState({
        feed: parsedFeed,
        loading: false,
        itemsInFeed: parsedFeed.items.length
      });
    })();
  }

  increaseVisibleEpisodes() {
    var episodeCount = this.state.elementsToShow;
    this.setState({
      elementsToShow: episodeCount + 20
    });
  }

  render() {
    const { feed, loading, elementsToShow, itemsInFeed } = this.state;

    return (
      <Dialog
        open={this.props.open}
        onClose={this.props.onClose}
        fullScreen={this.props.fullScreen}
        maxWidth="md"
      >
        {/* Requires a nested dialog to have the two stage screen fade and fade on content load */}
        <Dialog
          open={!loading && this.props.open}
          onClose={this.props.onClose}
          fullScreen={this.props.fullScreen}
          maxWidth="md"
        >
          {!loading && <DialogTitle>{feed.title}</DialogTitle>}
          {!loading && (
            <DialogContent>
              <DialogContentText>
                {removeTags(feed.description)}
              </DialogContentText>

              {/* Episodes */}
              {feed.items != null &&
                feed.items
                  .slice(0, elementsToShow)
                  .map(item => <EpisodeEntry episode={item} onClick={this} />)}

              {/* Load more button */}
              {elementsToShow < itemsInFeed && (
                <Button onClick={this.increaseVisibleEpisodes} color="primary">
                  Load More...
                </Button>
              )}
            </DialogContent>
          )}

          {/* Close button */}
          {!loading && (
            <DialogActions>
              <Button onClick={this.props.onClose} color="primary">
                Close
              </Button>
            </DialogActions>
          )}
        </Dialog>
      </Dialog>
    );
  }
}

export default withMobileDialog()(PodcastDetail);
