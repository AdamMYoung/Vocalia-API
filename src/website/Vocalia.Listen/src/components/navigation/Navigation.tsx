import React, { Component } from "react";
import {
  AppBar,
  Toolbar,
  IconButton,
  Typography,
  createStyles,
  Theme,
  withStyles,
  WithStyles,
  CssBaseline
} from "@material-ui/core";
import MenuIcon from "@material-ui/icons/Menu";
import NavDrawer from "./NavDrawer";
import { Category } from "../../utility/types";
import { drawerWidth } from "../../utility/constants";
import Search from "../search/Search";

/**
 * CSS styles of the top AppBar.
 */
const styles = (theme: Theme) =>
  createStyles({
    root: {
      width: "100%"
    },
    grow: {
      flexGrow: 1
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
    title: {
      display: "none",
      [theme.breakpoints.up("sm")]: {
        display: "block"
      }
    },
    content: {
      flexGrow: 1,
      [theme.breakpoints.up("sm")]: {
        marginLeft: drawerWidth,
        width: `calc(100% - ${drawerWidth}px)`
      }
    }
  });

/**
 * Required properties of the top AppBar.
 */
interface INavigationProps extends WithStyles<typeof styles> {
  categories: Category[];
  isMobile: boolean;
}

/**
 * State information of the top AppBar.
 */
interface INavigationState {
  mobileOpen: boolean;
}

/**
 * Provides a top AppBar component as well as drawer integration for search.
 * Child properties are displayed within the content area.
 */
class Navigation extends Component<INavigationProps, INavigationState> {
  constructor(props: INavigationProps) {
    super(props);

    this.state = {
      mobileOpen: false
    };
  }

  /**
   * Opens/closes the navigation drawer depending on it's current state.
   */
  onDrawerToggle = () => {
    this.setState(state => ({ mobileOpen: !state.mobileOpen }));
  };

  render() {
    const { classes, isMobile } = this.props;

    return (
      <div className={classes.root}>
        <CssBaseline />

        {/* Top AppBar */}
        <AppBar position="fixed" className={classes.appBar}>
          <Toolbar variant={isMobile ? "regular" : "dense"}>
            <IconButton
              color="inherit"
              aria-label="Open drawer"
              className={classes.menuButton}
              onClick={this.onDrawerToggle}
            >
              <MenuIcon />
            </IconButton>
            <Typography
              className={classes.title}
              variant="h6"
              color="inherit"
              noWrap
            >
              Vocalia
            </Typography>
            <div className={classes.grow} />
            <Search />
          </Toolbar>
        </AppBar>

        {/* Navigation drawer. */}
        <NavDrawer
          handleDrawerToggle={this.onDrawerToggle}
          mobileOpen={this.state.mobileOpen}
          categories={this.props.categories}
        />

        {/* Content to display. */}
        <main className={classes.content}>
          <Toolbar />
          {this.props.children}
        </main>
      </div>
    );
  }
}

export default withStyles(styles)(Navigation);
