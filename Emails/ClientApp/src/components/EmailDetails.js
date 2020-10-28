import React, { Component } from "react";
import { formatDate, viewError } from "./util";
//Design
import Avatar from "@material-ui/core/Avatar";
import CssBaseline from "@material-ui/core/CssBaseline";
import Checkbox from "@material-ui/core/Checkbox";
import Grid from "@material-ui/core/Grid";
import Box from "@material-ui/core/Box";
import Email from "@material-ui/icons/Email";
import Typography from "@material-ui/core/Typography";
import { makeStyles, withStyles } from "@material-ui/core/styles";
import Container from "@material-ui/core/Container";
import PropTypes from "prop-types";
import CircularProgress from "@material-ui/core/CircularProgress";
import { Card, CardContent } from "@material-ui/core";
import "../EmailDetails.css";
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
  card: {
    width: "100%", // Fix IE 11 issue.
    marginTop: theme.spacing(1),
  },
});
class EmailDetails extends Component {
  state = {
    email: null,
    loading: true,
    loadingFailure: false,
  };
  componentDidMount() {
    const { emailId } = this.props.location.state;
    fetch("api/sentemails/GetEmailById/?emailId=" + emailId)
      .then((response) => {
        if (response.status == 200) {
          return response.json();
        } else if (response.status == 401) this.props.history.push("/login");
        else {
          viewError("Error fetching email data");
          this.setState({ loading: false, loadingFailure: true });
        }
      })
      .then((data) => {
        this.setState({ email: data, loading: false });
      });
  }
  render() {
    const { classes } = this.props;
    const { email } = this.state;
    return (
      <Container component="main" maxWidth="xs">
        <CssBaseline />
        <div className={classes.paper}>
          <Avatar className={classes.avatar}>
            <Email />
          </Avatar>
          <Typography component="h1" variant="h5">
            Email Details
          </Typography>
          {this.state.loading && !this.state.loadingFailure && (
            <CircularProgress style={{ marginTop: "2%" }} />
          )}
          {!this.state.loading && !this.state.loadingFailure && (
            <Card className={classes.card}>
              <CardContent>
                <Grid container>
                  <Grid item xs={2}>
                    <label>Subject</label>
                  </Grid>
                  <Grid item xs={10}>
                    <p>{email.subject}</p>
                  </Grid>
                  <Grid item xs={2}>
                    <label>To</label>
                  </Grid>
                  <Grid item xs={10}>
                    <p>{email.groups.name}</p>
                  </Grid>
                  <Grid item xs={2}>
                    <label>Date</label>
                  </Grid>
                  <Grid item xs={10}>
                    <p>{formatDate(email.sendingDate)}</p>
                  </Grid>
                  {email.sentEmailsFailures.length > 0 && (
                    <Grid item xs={12}>
                      <label>Failed to Reach</label>
                    </Grid>
                  )}
                  {email.sentEmailsFailures.map((item, index) => {
                    return (
                      <Grid item xs={12}>
                        <p>{item.recipient}</p>
                      </Grid>
                    );
                  })}
                </Grid>
              </CardContent>
            </Card>
          )}
        </div>
      </Container>
    );
  }
}
EmailDetails.propTypes = {
  classes: PropTypes.object.isRequired,
};
export default withStyles(styles)(EmailDetails);
