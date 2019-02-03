import React, { Component } from "react";
import Dialog from "@material-ui/core/Dialog";
import { withMobileDialog, Divider } from "@material-ui/core";
import DialogActions from "@material-ui/core/DialogActions";
import DialogContent from "@material-ui/core/DialogContent";
import DialogContentText from "@material-ui/core/DialogContentText";
import DialogTitle from "@material-ui/core/DialogTitle";
import ExpansionPanel from "@material-ui/core/ExpansionPanel";
import ExpansionPanelSummary from "@material-ui/core/ExpansionPanelSummary";
import ExpansionPanelDetails from "@material-ui/core/ExpansionPanelDetails";
import ExpandMoreIcon from "@material-ui/icons/ExpandMore";
import PlayArrowIcon from "@material-ui/icons/PlayArrow";
import Typography from "@material-ui/core/Typography";
import Button from "@material-ui/core/Button";
import Fade from "@material-ui/core/Fade";

let Parser = require("rss-parser");

const CORS_PROXY = "https://cors-anywhere.herokuapp.com/";

class PodcastDetail extends Component {
  constructor(props) {
    super(props);

    this.state = {
      feed: {}
    };
  }

  componentWillReceiveProps(props) {
    if (props.rssUrl !== null) {
      let parser = new Parser();
      this.setState({ loading: true, feed: {} });

      (async () => {
        let parsedFeed = await parser.parseURL(CORS_PROXY + props.rssUrl);
        this.setState({ feed: parsedFeed, loading: false });
      })();
    }
  }

  handleChange = panel => (event, expanded) => {
    this.setState({
      expanded: expanded ? panel : false
    });
  };

  render() {
    const { feed, loading } = this.state;

    function removeTags(text) {
      if (text != null) {
        return text.replace(/<\/?[^>]+(>|$)/g, "");
      }
    }

    return (
      <Dialog
        open={this.props.open}
        onClose={this.props.onClose}
        fullScreen={this.props.fullScreen}
      >
        {!loading && (
          <Fade in={!loading}>
            <div>
              <DialogTitle>{feed.title}</DialogTitle>
              <DialogContent>
                <DialogContentText>
                  <div>{removeTags(feed.description)}</div>
                </DialogContentText>
                <Divider style={{ marginTop: "5px", marginBottom: "5px" }} />

                {feed.items != null &&
                  feed.items.map(item => (
                    <ExpansionPanel
                      key={item.guid}
                      expanded={this.state.expanded === item.title}
                      onChange={this.handleChange(item.title)}
                    >
                      <ExpansionPanelSummary expandIcon={<ExpandMoreIcon />}>
                        <PlayArrowIcon />
                        <Typography
                          style={{ marginTop: "2px", marginLeft: "2px" }}
                        >
                          {item.title}
                        </Typography>
                      </ExpansionPanelSummary>
                      <ExpansionPanelDetails>
                        <Typography>{removeTags(item.content)}</Typography>
                      </ExpansionPanelDetails>
                    </ExpansionPanel>
                  ))}
              </DialogContent>
              <DialogActions>
                <Button onClick={this.props.onClose} color="primary">
                  Close
                </Button>
              </DialogActions>
            </div>
          </Fade>
        )}
      </Dialog>
    );
  }
}

export default withMobileDialog()(PodcastDetail);
