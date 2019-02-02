import React, { Component } from "react";
import { Route, Switch } from "react-router";
import Layout from "./components/Layout";
import PodcastBrowse from "./components/browse/PodcastBrowse";

export default class App extends Component {
  displayName = App.name;

  render() {
    return (
      <Layout>
        <Route exact path="/subscribed" component={PodcastBrowse} />
        <Route
          exact
          path="/category/:id"
          render={props => <PodcastBrowse {...props} />}
        />
      </Layout>
    );
  }
}
