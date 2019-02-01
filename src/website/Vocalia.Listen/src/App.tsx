import React, { Component } from "react";
import { Route, Switch } from "react-router";
import Layout from "./components/Layout";
import BrowsePodcasts from "./components/browse/BrowsePodcasts";
import CategoryBrowsePodcasts from "./components/browse/CategoryBrowsePodcasts";

export default class App extends Component {
  displayName = App.name;

  render() {
    return (
      <Layout>
        <Route
          exact
          path="/top"
          render={props => <BrowsePodcasts {...props} />}
        />
        <Route exact path="/subscribed" component={BrowsePodcasts} />
        <Route
          exact
          path="/category/:id"
          render={props => <CategoryBrowsePodcasts {...props} />}
        />
      </Layout>
    );
  }
}
