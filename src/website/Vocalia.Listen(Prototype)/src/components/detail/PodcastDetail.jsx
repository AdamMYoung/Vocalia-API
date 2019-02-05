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

/**
 * Dialog box to display information about a selected podcast.
 * Prop Types:
 * rssUrl: RSS URL to parse.
 */
class PodcastDetail extends Component {
  constructor(props) {
    super(props);

    this.increaseVisibleEpisodes = this.increaseVisibleEpisodes.bind(this);

    this.state = {
      feed: {},
      elementsToShow: 20,
      episodeCount: 0
    };
  }

  /**
   * Parses the provided RSS URL asyncronously, and stores the resulting feed in the state.
   */
  componentWillReceiveProps(props) {
    let parser = new Parser({
      customFields: {
        item: ["enclosure"]
      }
    });
    this.setState({
      loading: true,
      feed: {},
      elementsToShow: 20,
      episodeCount: 0
    });

    (async () => {
      console.log(props.rssUrl);
      let parsedFeed = await parser.parseURL(CORS_PROXY + props.rssUrl);
      this.setState({
        feed: parsedFeed,
        loading: false,
        episodeCount: parsedFeed.items.length
      });
    })();
  }

  /**
   * Increases the number of visible episodes.
   */
  increaseVisibleEpisodes() {
    var episodeCount = this.state.elementsToShow;
    this.setState({
      elementsToShow: episodeCount + 20
    });
  }

  /**
   * Called when an episode has been selected.
   * @param {URL of the episode selected.} episodeUrl
   */
  onPlay = data => {
    const { feed } = this.state;
    data["podcast"] = feed.title;
    this.props.onPlay(data);
  };

  render() {
    const { feed, loading, elementsToShow, episodeCount } = this.state;

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
                  .map(item => (
                    <EpisodeEntry
                      key={item.guid}
                      episode={item}
                      onClick={this.onPlay}
                    />
                  ))}

              {/* Load more button */}
              {elementsToShow < episodeCount && (
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
