import React, { Component, PureComponent } from "react";
import { Divider, Modal, withMobileDialog } from "@material-ui/core";
import Dialog from "@material-ui/core/Dialog";
import DialogActions from "@material-ui/core/DialogActions";
import DialogContent from "@material-ui/core/DialogContent";
import DialogContentText from "@material-ui/core/DialogContentText";
import DialogTitle from "@material-ui/core/DialogTitle";
import Button from "@material-ui/core/Button";
import { Podcast, PodcastFeed, PodcastEpisode } from "../../types";
import EpisodeEntry from "./EpisodeEntry";
import VocaliaAPI from "../../utility/VocaliaAPI";
import { isMobile } from "../../utility/DeviceUtils";
import { removeTags } from "../../utility/FormatUtils";

interface IState {
  feed: PodcastFeed;
  visibleEpisodes: number;
  loading: boolean;
}

interface IProps {
  podcast: Podcast;
  open: boolean;
  isMobile: boolean;
  selectedEpisode: PodcastEpisode;
  onClose: () => void;
  onEpisodeSelected: (episode: PodcastEpisode) => void;
}

class PodcastDetail extends PureComponent<IProps, IState> {
  constructor(props: IProps) {
    super(props);

    this.state = {
      feed: {} as PodcastFeed,
      visibleEpisodes: 20,
      loading: true
    };
  }

  componentWillReceiveProps = (props: IProps) => {
    var loader = new VocaliaAPI();
    const { podcast } = this.props;

    if (
      props.podcast.rssUrl !== null &&
      props.podcast.rssUrl !== podcast.rssUrl
    ) {
      (async () => {
        this.setState({ loading: true });
        let feed = await loader.parsePodcastFeed(props.podcast.rssUrl);
        this.setState({ feed: feed, loading: false });
      })();
    }
  };

  /**
   * Increases the number of visible episodes.
   */
  increaseVisibleEpisodes = () => {
    let oldCount = this.state.visibleEpisodes;
    this.setState({ visibleEpisodes: oldCount + 20 });
  };

  render() {
    const { feed, visibleEpisodes, loading } = this.state;
    const {
      open,
      onClose,
      onEpisodeSelected,
      selectedEpisode,
      isMobile
    } = this.props;

    return (
      <Dialog open={open} onClose={onClose} fullScreen={isMobile} maxWidth="md">
        {/* Requires a nested dialog to have the two stage screen fade and fade on content load */}
        <Dialog
          open={!loading && this.props.open}
          onClose={onClose}
          fullScreen={isMobile}
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
                  .slice(0, visibleEpisodes)
                  .map(item => (
                    <EpisodeEntry
                      key={item.content}
                      episode={item}
                      selectedEpisode={selectedEpisode}
                      onEpisodeSelected={(episode: PodcastEpisode) =>
                        onEpisodeSelected(episode)
                      }
                    />
                  ))}

              {/* Load more button */}
              {visibleEpisodes < feed.items.length && (
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

//Currently no other way to export, so a cast to any is requried.
export default PodcastDetail;
