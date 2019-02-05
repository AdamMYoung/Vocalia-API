import React, { Component } from "react";
import Divider from "@material-ui/core/Divider";
import Drawer from "@material-ui/core/Drawer";
import Hidden from "@material-ui/core/Hidden";
import List from "@material-ui/core/List";
import {
  ListItem,
  ListItemText,
  ListItemIcon,
  createStyles,
  Theme,
  withStyles,
  WithStyles
} from "@material-ui/core";
import PersonIcon from "@material-ui/icons/Person";
import BarChartIcon from "@material-ui/icons/BarChart";
import StarIcon from "@material-ui/icons/Star";
import { LinkContainer } from "react-router-bootstrap";
import { drawerWidth } from "../../constants";
import { Category } from "../../types";

const styles = (theme: Theme) =>
  createStyles({
    root: {
      display: "flex"
    },
    drawer: {
      [theme.breakpoints.up("sm")]: {
        width: drawerWidth,
        flexShrink: 0
      }
    },
    appBar: {
      marginLeft: drawerWidth,
      [theme.breakpoints.up("sm")]: {
        width: `calc(100% - ${drawerWidth}px)`
      }
    },
    menuButton: {
      marginRight: 20,
      [theme.breakpoints.up("sm")]: {
        display: "none"
      }
    },
    toolbar: theme.mixins.toolbar,
    drawerPaper: {
      width: drawerWidth
    },
    content: {
      flexGrow: 1,
      padding: theme.spacing.unit * 3
    }
  });

interface IProps extends WithStyles<typeof styles> {
  theme: Theme;
  mobileOpen: boolean;
  categories: Category[];
  handleDrawerToggle: () => void;
}

interface IState {}

class NavDrawer extends Component<IProps, IState> {
  /**
   * Closes the drawer if the mobile varient has been opened.
   */
  closeDrawer = () => {
    if (this.props.mobileOpen) this.props.handleDrawerToggle();
  };

  render() {
    const { classes, theme } = this.props;

    const drawer = (
      <div>
        <div className={classes.toolbar}>
          <List>
            <LinkContainer to="/login">
              <ListItem button>
                <ListItemIcon>
                  <PersonIcon />
                </ListItemIcon>
                <ListItemText primary="Sign-in" onClick={this.closeDrawer} />
              </ListItem>
            </LinkContainer>
          </List>
        </div>
        <Divider />
        <List>
          <LinkContainer to="/top">
            <ListItem button>
              <ListItemIcon>
                <BarChartIcon />
              </ListItemIcon>
              <ListItemText primary="Top" onClick={this.closeDrawer} />
            </ListItem>
          </LinkContainer>
          <LinkContainer to="/subscribed">
            <ListItem button>
              <ListItemIcon>
                <StarIcon />
              </ListItemIcon>
              <ListItemText primary="Subscribed" onClick={this.closeDrawer} />
            </ListItem>
          </LinkContainer>
        </List>
        <Divider />
        <List>
          {this.props.categories.map(category => (
            <LinkContainer key={category.id} to={"/browse/" + category.id}>
              <ListItem button>
                <ListItemText
                  primary={category.title}
                  onClick={this.closeDrawer}
                />
              </ListItem>
            </LinkContainer>
          ))}
        </List>
      </div>
    );

    return (
      <div className={classes.root}>
        <nav className={classes.drawer}>
          {/* The implementation can be swapped with js to avoid SEO duplication of links. */}
          <Hidden smUp implementation="css">
            <Drawer
              variant="temporary"
              anchor={theme.direction === "rtl" ? "right" : "left"}
              open={this.props.mobileOpen}
              onClose={this.props.handleDrawerToggle}
              classes={{
                paper: classes.drawerPaper
              }}
            >
              {drawer}
            </Drawer>
          </Hidden>
          <Hidden xsDown implementation="css">
            <Drawer
              classes={{
                paper: classes.drawerPaper
              }}
              variant="permanent"
              open
            >
              {drawer}
            </Drawer>
          </Hidden>
        </nav>
      </div>
    );
  }
}

export default withStyles(styles, { withTheme: true })(NavDrawer);
