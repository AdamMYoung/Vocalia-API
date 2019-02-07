import React, { Component } from "react";
import { Slide } from "@material-ui/core";
import {
  Route,
  RouteComponentProps,
  withRouter,
  Redirect,
  Switch
} from "react-router";
import { Category, Podcast, PodcastEpisode } from "../utility/types";
import Navigation from "./navigation/Navigation";
import MediaPlayer from "./player/MediaPlayer";
import VocaliaAPI from "../utility/VocaliaAPI";
import PodcastBrowser from "./browse/PodcastBrowser";
import PodcastDetail from "./detail/PodcastDetail";
import Callback from "../auth/Callback";
import Auth from "../auth/Auth";

/**
 * State information for the application.
 */
interface ILayoutState {
  podcastData: { [key: string]: Podcast[] };
  categories: Category[];
  dialogOpen: boolean;
  selectedEpisode: PodcastEpisode;
  auth: Auth;
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
      auth: new Auth(props),
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
  async componentWillMount() {
    var loader = new VocaliaAPI();

    //Load category list and category data asynchronously.
    let categories = await loader.getCategories();
    this.setState({ categories: categories });

    categories.forEach(async category => {
      let id = category.id;
      let podcasts = await loader.getPodcastByCategory(id);

      let loadedPodcast = this.state.podcastData;
      loadedPodcast[id] = podcasts;
      this.setState({ podcastData: loadedPodcast });
    });

    //Load top podcast data asynchronously.
    let podcasts = await loader.getTopPodcasts();

    let loadedPodcasts = this.state.podcastData;
    loadedPodcasts["top"] = podcasts;
    this.setState({ podcastData: loadedPodcasts });
  }

  /**
   * Called when an episode has been selected for playback.
   */
  onEpisodeSelected = (episode: PodcastEpisode) => {
    this.setState({ selectedEpisode: episode });
  };

  onDialogClose = () => {
    let history = this.props.history;
    if (history.length > 1) history.goBack();
    else history.push("/top");
  };

  handleAuthentication = (nextState: any, replace: any) => {
    if (/access_token|id_token|error/.test(nextState.location.hash)) {
      this.state.auth.handleAuthentication();
    }
  };

  render() {
    const { podcastData, selectedEpisode, categories, auth } = this.state;
    const { isMobile } = this.props;

    /**
     * Elements that can be routed to.
     */
    const RoutingContents = (
      <Switch>
        <Route
          path="/top/"
          render={() => <PodcastBrowser podcasts={podcastData["top"]} />}
        />

        <Route
          path="/browse/:id/"
          render={props => (
            <PodcastBrowser podcasts={podcastData[props.match.params.id]} />
          )}
        />

        <Route
          exact
          path="/detail/:rss/"
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

        <Route
          path="/callback"
          render={props => {
            this.handleAuthentication(props, null);
            return <Callback {...props} />;
          }}
        />

        <Redirect exact from="/" to={"/top"} />
      </Switch>
    );

    return (
      <Navigation categories={categories} isMobile={isMobile} auth={auth}>
        <React.Fragment>
          {RoutingContents}
          {selectedEpisode.content != null && (
            <Slide direction={"up"} in={selectedEpisode.content != null}>
              <MediaPlayer media={selectedEpisode} isMobile={isMobile} />
            </Slide>
          )}
        </React.Fragment>
      </Navigation>
    );
  }
}

export default withRouter(Layout);
