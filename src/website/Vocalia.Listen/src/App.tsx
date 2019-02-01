import React, { Component } from "react";
import { Route, Switch } from "react-router";
import Layout from "./components/Layout";
import BrowsePodcasts from "./components/browse/BrowsePodcasts";
import CategoryPodcastBrowse from "./components/browse/CategoryBrowsePodcasts";

export default class App extends Component {
  displayName = App.name;

  render() {
    return (
      <Layout>
        <Route path="/top" component={BrowsePodcasts} />
        <Route path="/subscribed" component={BrowsePodcasts} />
        <Route path="/category/:id" component={CategoryPodcastBrowse} />
      </Layout>
    );
  }
}
