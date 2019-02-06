import React, { Component } from "react";
import { Category, Podcast, PodcastEpisode } from "../types";
import Navigation from "./navigation/Navigation";
import MediaPlayer from "./player/MediaPlayer";
import { Slide } from "@material-ui/core";
import VocaliaAPI from "../utility/VocaliaAPI";
import PodcastBrowser from "./browse/PodcastBrowser";
import { Route, RouteComponentProps, withRouter } from "react-router";
import PodcastDetail from "./detail/PodcastDetail";

/**
 * State information for the application.
 */
interface ILayoutState {
  podcastData: { [key: string]: Podcast[] };
  categories: Category[];
  dialogOpen: boolean;
  selectedEpisode: PodcastEpisode;
}

/**
 * Required properties for the application.
 */
interface ILayoutProps extends RouteComponentProps {
  isMobile: boolean;
}

/**
 * UI entry point into the application, handles routing and player interaction.
 */
export class Layout extends Component<ILayoutProps, ILayoutState> {
  displayName = Layout.name;

  constructor(props: ILayoutProps) {
    super(props);

    this.state = {
      podcastData: { top: [] },
      categories: [],
      dialogOpen: false,
      selectedEpisode: { time: 0 } as PodcastEpisode
    };
  }
  /**
   * Called before the component finishes mounting,
   * and loads all categories and podcasts into memory.
   */
  componentWillMount() {
    var loader = new VocaliaAPI();

    //Load category list and category data asynchronously.
    (async () => {
      let categories = await loader.getCategories();
      this.setState({ categories: categories });

      categories.forEach(async category => {
        let id = category.id;
        console.log(category);
        let podcasts = await loader.getPodcastByCategory(id);

        let loadedPodcast = this.state.podcastData;
        loadedPodcast[id] = podcasts;
        this.setState({ podcastData: loadedPodcast });
      });
    })();

    //Load top podcast data asynchronously.
    (async () => {
      let podcasts = await loader.getTopPodcasts();

      let loadedPodcasts = this.state.podcastData;
      loadedPodcasts["top"] = podcasts;
      this.setState({ podcastData: loadedPodcasts });
    })();
  }

  /**
   * Called when an episode has been selected for playback.
   */
  onEpisodeSelected = (episode: PodcastEpisode) => {
    this.setState({ selectedEpisode: episode });
  };

  onDialogClose = () => {
    let history = this.props.history;
    history.goBack();
  };

  render() {
    const { podcastData, selectedEpisode, categories, dialogOpen } = this.state;
    const { isMobile } = this.props;

    /**
     * Elements that can be routed to.
     */
    const RoutingContents = (
      <React.Fragment>
        <Route
          exact
          path="/top"
          render={() => <PodcastBrowser podcasts={podcastData["top"]} />}
        />

        <Route
          exact
          path="/browse/:id/"
          render={props => (
            <PodcastBrowser podcasts={podcastData[props.match.params.id]} />
          )}
        />

        <Route
          path="/detail/:rss"
          render={props => (
            <PodcastDetail
              open={true}
              rssFeed={props.match.params.rss}
              selectedEpisode={selectedEpisode}
              isMobile={isMobile}
              onClose={() => this.onDialogClose()}
              onEpisodeSelected={episode =>
                this.setState({ selectedEpisode: episode })
              }
            />
          )}
        />
      </React.Fragment>
    );

    return (
      <Navigation categories={categories} isMobile={isMobile}>
        <React.Fragment>
          {RoutingContents}
          {selectedEpisode.link != null && (
            <Slide direction={"up"} in={selectedEpisode.link != null}>
              <MediaPlayer media={selectedEpisode} isMobile={isMobile} />
            </Slide>
          )}
        </React.Fragment>
      </Navigation>
    );
  }
}

export default withRouter(Layout);
