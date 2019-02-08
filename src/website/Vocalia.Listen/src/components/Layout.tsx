import React, { Component } from "react";
import { Slide } from "@material-ui/core";
import {
  Route,
  RouteComponentProps,
  withRouter,
  Redirect,
  Switch
} from "react-router";
import {
  Category,
  Podcast,
  PodcastEpisode,
  MediaState
} from "../utility/types";
import Navigation from "./navigation/Navigation";
import MediaPlayer from "./player/MediaPlayer";
import VocaliaAPI from "../utility/VocaliaAPI";
import PodcastBrowser from "./browse/PodcastBrowser";
import PodcastDetail from "./detail/PodcastDetail";
import Subscriptions from "./subscriptions/Subscriptions";
import Callback from "../auth/Callback";
import Auth from "../auth/Auth";
import { SetCurrentPodcast, GetCurrentPodcast } from "../utility/PlaybackUtils";

/**
 * State information for the application.
 */
interface ILayoutState {
  podcastData: { [key: string]: Podcast[] };
  categories: Category[];
  dialogOpen: boolean;
  media: MediaState;
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
      media: { episode: GetCurrentPodcast(), autoplay: false }
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
    SetCurrentPodcast(episode);

    let media = this.state.media;
    media.autoplay = true;
    media.episode = episode;
    this.setState({ media: media });
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
    const { podcastData, media, categories, auth } = this.state;
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
          path="/subscribed/"
          render={() => <Subscriptions auth={auth} />}
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
              selectedEpisode={media.episode}
              isMobile={isMobile}
              onClose={() => this.onDialogClose()}
              onEpisodeSelected={episode => this.onEpisodeSelected(episode)}
            />
          )}
        />

        <Route
          path="/callback"
          render={() => {
            return <Callback auth={this.state.auth} />;
          }}
        />

        <Redirect exact from="/" to={"/top"} />
      </Switch>
    );

    return (
      <Navigation categories={categories} isMobile={isMobile} auth={auth}>
        <React.Fragment>
          {RoutingContents}
          {media.episode != null && (
            <Slide direction={"up"} in={media.episode.content != null}>
              <MediaPlayer media={media} isMobile={isMobile} />
            </Slide>
          )}
        </React.Fragment>
      </Navigation>
    );
  }
}

export default withRouter(Layout);
