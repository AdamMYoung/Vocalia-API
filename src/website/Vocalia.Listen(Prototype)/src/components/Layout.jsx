import React, { Component } from "react";
import NavMenu from "./navigation/NavMenu";
import MaterialPlayer from "./player/material-player";
import Grid from "@material-ui/core/Grid";
import { withStyles } from "@material-ui/core/styles";
import PubSub from "pubsub-js";

const MyContext = React.createContext();
const styles = theme => ({
  margin: {
    [theme.breakpoints.down("sm")]: {
      marginBottom: 150
    },
    [theme.breakpoints.up("md")]: {
      paddingBottom: 110
    }
  }
});

class Layout extends Component {
  constructor(props) {
    super(props);

    this.state = {
      src: null,
      img: null,
      podcast: null,
      episode: null
    };
  }

  onPlay = (msg, data) => {
    console.log(data);
    this.setState({
      src: data["src"],
      img: data["img"],
      podcast: data["podcast"],
      episode: data["episode"]
    });
  };

  componentWillMount = () => {
    this.token = PubSub.subscribe("newAudio", this.onPlay);
  };

  componentWillUnmount = () => {
    PubSub.unsubscribe(this.token);
  };

  render() {
    const { classes } = this.props;
    const { src, img, podcast, episode } = this.state;

    return (
      <NavMenu>
        <Grid container>
          <Grid id="content" item xs={12}>
            <MyContext.Provider value={{ onPlay: this.onPlay }}>
              {this.props.children}
            </MyContext.Provider>
          </Grid>
          <Grid id="player" item xs={12} className={classes.margin}>
            <MaterialPlayer
              src={src}
              image={img}
              podcast={podcast}
              episode={episode}
            />
          </Grid>
        </Grid>
      </NavMenu>
    );
  }
}

export default withStyles(styles)(Layout);
