import React, { Component } from "react";
import {
  Card,
  List,
  ListItem,
  ListItemText,
  ListItemAvatar,
  Avatar
} from "@material-ui/core";
import { Podcast } from "../../types";
import VocaliaAPI from "../../utility/VocaliaAPI";

interface ISearchProps {
  term: string;
  podcastSelected;
}

interface ISearchState {
  podcasts: Podcast[];
}

interface IEntryProps {
  podcast: Podcast;
}

class Search extends Component<ISearchProps, ISearchState> {
  componentWillMount() {
    this.querySearch(this.props.term);
  }

  componentWillReceiveProps(props: ISearchProps) {
    this.querySearch(props.term);
  }

  querySearch = (term: string) => {
    let loader = new VocaliaAPI();

    //Load top podcast data asynchronously.
    (async () => {
      this.setState({ podcasts: await loader.searchPodcasts(term) });
    })();
  };

  render() {
    const { podcasts } = this.state;

    function Entry(props: IEntryProps) {
      const { podcast } = props;

      return (
        <ListItem alignItems="flex-start">
          <ListItemAvatar>
            <Avatar alt={podcast.title} src={podcast.imageUrl} />
          </ListItemAvatar>
          <ListItemText primary={podcast.title} />
        </ListItem>
      );
    }

    return (
      <Card>
        <List>
          {podcasts.map(podcast => (
            <Entry podcast={podcast} />
          ))}
        </List>
      </Card>
    );
  }
}
