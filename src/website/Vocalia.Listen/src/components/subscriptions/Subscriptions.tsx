import React, { Component } from "react";
import { Subscription } from "../../utility/types";
import Auth from "../../auth/Auth";
import VocaliaAPI from "../../utility/VocaliaAPI";

interface ISubscriptionsState {
  subscriptions: Subscription[];
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
    console.log(accessToken);

    if (accessToken != null) {
      let subscriptions = await loader.getSubscriptions(accessToken);
      console.log("got stuff lol");
      this.setState({ subscriptions: subscriptions });
    }
  }

  render() {
    return <div />;
  }
}
