import React, { Component } from "react";

class CategoryPodcastBrowse extends Component {
  constructor(props) {
    super(props);

    this.state = {
      categoryId: this.props.match.params.id
    };
  }

  componentDidMount() {
    this.setState({ categoryId: this.props.match.params.id });
  }

  componentWillReceiveProps(newProps) {
    this.setState({ categoryId: newProps.match.params.id });
  }

  loadPodcasts() {
    fetch();
  }

  render() {
    console.log("Category: " + this.state.categoryId);
    return <p>saasdiaodaodaoius + {this.state.categoryId}</p>;
  }
}

export default CategoryPodcastBrowse;
