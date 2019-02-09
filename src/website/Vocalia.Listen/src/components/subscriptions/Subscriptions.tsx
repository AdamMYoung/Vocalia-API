import React, { Component } from "react";
import { Podcast } from "../../utility/types";
import Auth from "../../auth/Auth";
import VocaliaAPI from "../../utility/VocaliaAPI";
import PodcastBrowser from "../browse/PodcastBrowser";

interface ISubscriptionsState {
  subscriptions: Podcast[];
}

interface ISubscriptionProps {
  auth: Auth;
}

export default class Subscriptions extends Component<
  ISubscriptionProps,
  ISubscriptionsState
> {
  constructor(props: ISubscriptionProps) {
    super(props);

    this.state = {
      subscriptions: []
    };
  }

  async componentDidMount() {
    let loader = new VocaliaAPI();
    let accessToken = this.props.auth.getAccessToken();

    if (accessToken != null) {
      let subscriptions = await loader.getSubscriptions(accessToken);
      this.setState({ subscriptions: subscriptions });
    }
  }

  render() {
    const { subscriptions } = this.state;
    return <PodcastBrowser podcasts={subscriptions} />;
  }
}
