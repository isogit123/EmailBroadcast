﻿import React, { Component } from "react";
import { Link } from "react-router-dom";
import {
  removeConfirmation,
  viewError,
  getCookie,
  formatDate,
  checkSession,
} from "./util";
//Design
import Avatar from "@material-ui/core/Avatar";
import Grid from "@material-ui/core/Grid";
import CssBaseline from "@material-ui/core/CssBaseline";
import CircularProgress from "@material-ui/core/CircularProgress";
import Email from "@material-ui/icons/Email";
import ErrorOutline from "@material-ui/icons/ErrorOutline";
import Typography from "@material-ui/core/Typography";
import { makeStyles, withStyles } from "@material-ui/core/styles";
import Container from "@material-ui/core/Container";
import PropTypes from "prop-types";
import {
  Fab,
  LinearProgress,
  List,
  ListItem,
  ListItemIcon,
  ListItemSecondaryAction,
  ListItemText,
} from "@material-ui/core";
import Card from "@material-ui/core/Card";
import InfiniteScroll from "react-infinite-scroll-component";
import "./EmailsList.css";

const styles = (theme) => ({
  paper: {
    marginTop: theme.spacing(8),
    display: "flex",
    flexDirection: "column",
    alignItems: "center",
  },
  avatar: {
    margin: theme.spacing(1),
    backgroundColor: theme.palette.secondary.main,
  },
  list: {
    margin: theme.spacing(1),
    backgroundColor: theme.palette.background.paper,
  },
  errorIcon: {
    marginLeft: theme.spacing(5),
  },
});

class EmailsList extends Component {
  constructor() {
    super();
  }
  state = {
    emails: [],
    loading: true,
    hasMore: false,
  };

  async getEmails() {
    checkSession();
    let url = `api/sentemails/GetEmails/`;
    if (this.state.emails.length > 0)
      url +=
        "?" +
        new URLSearchParams({
          sendingDateStart:
            this.state.emails[this.state.emails.length - 1].sendingDate,
        });
    const response = await fetch(url);
    if (response.status == 200) {
      const data = await response.json();
      const newState = this.state;
      newState.emails = newState.emails.concat(data.emails);
      newState.hasMore = data.hasMore;
      console.log(newState);
      this.setState(newState);
    } else if (response.status != 401)
      viewError("Website not available", "Please try again later");
  }
  async componentDidMount() {
    await this.getEmails();
    this.setState({ loading: false });
  }

  render() {
    const { classes } = this.props;
    return (
      <Container component="main" maxWidth="">
        <CssBaseline />
        <div className={classes.paper}>
          <Avatar className={classes.avatar}>
            <Email />
          </Avatar>
          <Typography component="h1" variant="h5">
            Sent Emails
          </Typography>
          {this.state.loading && (
            <CircularProgress style={{ marginTop: "2%" }} />
          )}

          <Grid container>
            <InfiniteScroll
              dataLength={this.state.emails.length}
              next={this.getEmails.bind(this)}
              hasMore={this.state.hasMore}
              loader={
                <div style={{ textAlign: "center" }}>
                  <CircularProgress />
                </div>
              }
            >
              <Grid item xs={12}>
                {!this.state.loading && this.state.emails.length > 0 && (
                  <List className={classes.list} component={Card}>
                    {this.state.emails.map((item, index) => {
                      return (
                        <ListItem
                          button
                          component={Link}
                          to={{
                            pathname: "/EmailDetails/" + item.idFromMailService,
                          }}
                        >
                          <ListItemText
                            primary={
                              <div>
                                <div>{item.groupName}</div>
                                <div>{item.subject}</div>
                              </div>
                            }
                            secondary={formatDate(item.sendingDate)}
                          />
                          {item.failedToReachEmails.length > 0 && (
                            <ListItemIcon className={classes.errorIcon}>
                              <ErrorOutline color="error" />
                            </ListItemIcon>
                          )}
                        </ListItem>
                      );
                    })}
                  </List>
                )}
              </Grid>
            </InfiniteScroll>
          </Grid>
        </div>
      </Container>
    );
  }
}
EmailsList.propTypes = {
  classes: PropTypes.object.isRequired,
};
export default withStyles(styles)(EmailsList);
