import React from "react";
import PropTypes from "prop-types";
import Divider from "@material-ui/core/Divider";
import List from "@material-ui/core/List";
import ListItem from "@material-ui/core/ListItem";
import ListItemIcon from "@material-ui/core/ListItemIcon";
import ListItemText from "@material-ui/core/ListItemText";
import ListSubheader from "@material-ui/core/ListSubheader";
import PersonIcon from "@material-ui/icons/Person";
import BarChartIcon from "@material-ui/icons/BarChart";
import { withStyles } from "@material-ui/core/styles";
import { LinkContainer } from "react-router-bootstrap";

const API = "http://localhost:54578/podcast/";
const CATEGORIES = "categories";

const styles = theme => ({
  toolbar: theme.mixins.toolbar
});

class NavDrawer extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      categories: [],
      loading: false,
      error: null
    };
  }

  componentDidMount() {
    this.setState({ isLoading: true });

    fetch(API + CATEGORIES)
      .then(response => response.json())
      .then(data => this.setState({ categories: data, isLoading: false }))
      .catch(error => this.setState({ error, isLoading: false }));
  }

  navMenuSelected(key) {
    const { callback } = this.state;
    callback(key);
  }

  render() {
    const { categories, isLoading } = this.state;

    return (
      <div>
        <List>
          <LinkContainer to="/top">
            <ListItem button key="top">
              <ListItemIcon>
                <BarChartIcon />
              </ListItemIcon>
              <ListItemText primary="Top" />
            </ListItem>
          </LinkContainer>

          <LinkContainer to="/subscribed">
            <ListItem button key="personal">
              <ListItemIcon>
                <PersonIcon />
              </ListItemIcon>
              <ListItemText primary="Subscriptions" />
            </ListItem>
          </LinkContainer>
        </List>

        <Divider />

        <List
          subheader={
            <ListSubheader component="div" color="primary" disableSticky={true}>
              Categories
            </ListSubheader>
          }
        >
          {isLoading && (
            <ListItem>
              <ListItemText primary="Loading..." />
            </ListItem>
          )}
          {categories.map(category => (
            <LinkContainer to={"/category/" + category.id}>
              <ListItem button key={category.id}>
                <ListItemIcon>
                  <img src={category.iconUrl} alt="" />
                </ListItemIcon>
                <ListItemText primary={category.title} />
              </ListItem>
            </LinkContainer>
          ))}
        </List>
      </div>
    );
  }
}

NavDrawer.propTypes = {
  container: PropTypes.object,
  theme: PropTypes.object.isRequired
};

export default withStyles(styles, { withTheme: true })(NavDrawer);
