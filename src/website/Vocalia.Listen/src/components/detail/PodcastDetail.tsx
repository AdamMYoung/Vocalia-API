import React, { PureComponent } from "react";
import Dialog from "@material-ui/core/Dialog";
import DialogActions from "@material-ui/core/DialogActions";
import DialogContent from "@material-ui/core/DialogContent";
import DialogContentText from "@material-ui/core/DialogContentText";
import DialogTitle from "@material-ui/core/DialogTitle";
import Card from "@material-ui/core/Card";
import Button from "@material-ui/core/Button";
import { PodcastFeed, PodcastEpisode } from "../../types";
import EpisodeEntry from "./EpisodeEntry";
import VocaliaAPI from "../../utility/VocaliaAPI";
import { removeTags } from "../../utility/FormatUtils";
import { Typography } from "@material-ui/core";

/**
 * Properties required for the detail modal window.
 */
interface IDetailProps {
  rssFeed: string; //Feed to load.
  open: boolean; //External control for dialog visibility.
  isMobile: boolean; //Indicates if the device is a mobile device.
  selectedEpisode: PodcastEpisode; //The currently playing episode.
  onClose: () => void; //Called when the dialog requests to be closed.
  onEpisodeSelected: (episode: PodcastEpisode) => void; //Called when an episode isselected.
}

/**
 * State information for the detail modal window.
 */
interface IDetailState {
  feed: PodcastFeed; //Currently loaded feed.
  visibleEpisodes: number; //Number of visible episodes.
  loading: boolean; //Indicates the feed is loading.
}

/**
 * Modal window displaying title, description and episode information for a specific podcast.
 */
class PodcastDetail extends PureComponent<IDetailProps, IDetailState> {
  constructor(props: IDetailProps) {
    super(props);

    this.state = {
      feed: {} as PodcastFeed,
      visibleEpisodes: 20,
      loading: true
    };
  }

  /**
   * Updates the detail dialog. Only refreshes if the RSS URL has changed to prevent unnecessary re-renders.
   */
  componentWillMount = () => {
    var loader = new VocaliaAPI();
    const { rssFeed } = this.props;

    if (rssFeed !== null) {
      (async () => {
        this.setState({ loading: true });
        let feed = await loader.parsePodcastFeed(rssFeed);
        this.setState({ feed: feed, loading: false });
      })();
    }
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
      <Card
        style={{
          height: window.outerHeight,
          width: window.outerWidth,
          margin: -100
        }}
      >
        <Dialog
          open={open}
          onClose={onClose}
          fullScreen={isMobile}
          maxWidth="md"
        >
          {/* Requires a nested dialog to have the two stage screen dim and fade in on content load */}
          <Dialog
            open={!loading && this.props.open}
            onClose={onClose}
            fullScreen={isMobile}
            maxWidth="md"
          >
            {!loading && (
              <React.Fragment>
                <DialogTitle disableTypography={true}>
                  <Typography>
                    <div style={{ display: "flex" }}>
                      <div
                        style={{ height: 80, width: 80, alignSelf: "center" }}
                      >
                        <img src={feed.imageUrl} />
                      </div>

                      <div style={{ display: "inline", paddingLeft: 15 }}>
                        <h2>{feed.title}</h2>
                        <p>{removeTags(feed.description)}</p>
                      </div>
                    </div>
                  </Typography>
                </DialogTitle>
                <DialogContent style={{ paddingTop: 5 }}>
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
                    <Button
                      onClick={() =>
                        this.setState(oldState => ({
                          visibleEpisodes: oldState.visibleEpisodes + 20
                        }))
                      }
                      color="primary"
                    >
                      Load More...
                    </Button>
                  )}
                </DialogContent>

                {/* Close button */}
                {!loading && (
                  <DialogActions>
                    <Button onClick={this.props.onClose} color="primary">
                      Close
                    </Button>
                  </DialogActions>
                )}
              </React.Fragment>
            )}
          </Dialog>
        </Dialog>
      </Card>
    );
  }
}

export default PodcastDetail;
