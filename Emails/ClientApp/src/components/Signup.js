import React, { Component } from "react";
import { Button, TextField, FormControl } from "@material-ui/core";
import { getCookie, viewSuccess, viewError, testEmail } from "./util";
import { Link } from "react-router-dom";
//Design
import Avatar from "@material-ui/core/Avatar";
import CssBaseline from "@material-ui/core/CssBaseline";
import FormControlLabel from "@material-ui/core/FormControlLabel";
import Checkbox from "@material-ui/core/Checkbox";
import Grid from "@material-ui/core/Grid";
import Box from "@material-ui/core/Box";
import LockOutlinedIcon from "@material-ui/icons/LockOutlined";
import Typography from "@material-ui/core/Typography";
import { makeStyles, withStyles } from "@material-ui/core/styles";
import Container from "@material-ui/core/Container";
import PropTypes from "prop-types";
import SubmitButton from "./SubmitButton";

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
  form: {
    width: "100%", // Fix IE 11 issue.
    marginTop: theme.spacing(3),
  },
  submit: {
    margin: theme.spacing(3, 0, 2),
  },
});

class Signup extends Component {
  state = {
    username: "",
    password: "",
    confirmPassword: "",
    email: "",
    usernameErrorState: false,
    passwordErrorState: false,
    confirmPasswordErrorState: false,
    emailErrorState: false,
    emailErrorMsg: "",
    confirmPasswordErrorMsg: "",
    submitting: false,
    isIntro: typeof this.props.intro != "undefined",
  };
  handleChange = (event) => {
    const enteredValueName = event.target.name;
    this.setState({ [enteredValueName]: event.target.value });
  };
  validateUserName() {
    let isValid = true;
    if (!this.state.username) {
      isValid = false;
      this.setState({ usernameErrorState: true });
    } else {
      this.setState({ usernameErrorState: false });
    }
    return isValid;
  }
  validatePassword() {
    let isValid = true;
    if (!this.state.password) {
      isValid = false;
      this.setState({ passwordErrorState: true });
    } else {
      this.setState({ passwordErrorState: false });
    }
    return isValid;
  }
  validateConfirmPassword() {
    let isValid = true;
    if (!this.state.confirmPassword) {
      isValid = false;
      this.setState({
        confirmPasswordErrorState: true,
        confirmPasswordErrorMsg: "",
      });
    } else if (this.state.confirmPassword !== this.state.password) {
      isValid = false;
      this.setState({
        confirmPasswordErrorState: true,
        confirmPasswordErrorMsg: "Passwords do not match",
      });
    } else {
      this.setState({
        confirmPasswordErrorState: false,
        confirmPasswordErrorMsg: "",
      });
    }
    return isValid;
  }
  validateEmail() {
    if (!this.state.email) {
      this.setState({ emailErrorState: false, emailErrorMsg: "" });
      return true;
    }
    let isValid = true;
    if (!testEmail(this.state.email)) {
      isValid = false;
      this.setState({
        emailErrorState: true,
        emailErrorMsg: "Email not valid",
      });
    } else {
      this.setState({ emailErrorState: false, emailErrorMsg: "" });
    }
    return isValid;
  }
  validateForm() {
    return (
      this.validateUserName() &&
      this.validatePassword() &&
      this.validateEmail() &&
      this.validateConfirmPassword()
    );
  }
  submitUser = (event) => {
    event.preventDefault();
    if (this.validateForm()) {
      this.setState({ submitting: true });
      fetch("api/users/add", {
        method: "POST",
        body: JSON.stringify({
          name: this.state.username,
          password: this.state.password,
          email: this.state.email,
        }),
        headers: {
          "Content-Type": "application/json",
          "X-CSRF-TOKEN": getCookie("CSRF-TOKEN"),
        },
      })
        .then((response) => {
          response.status == 200
            ? viewSuccess(
                "User added successfully",
                "Confirmation email has been sent"
              )
            : viewError("Error adding user");
          this.setState({ submitting: false });
        })
        .catch(() => {
          viewError("Error adding user");
          this.setState({ submitting: false });
        });
    }
  };

  render() {
    const { classes } = this.props;
    return (
      <Container component="main" maxWidth="xs">
        <CssBaseline />
        <div className={classes.paper}>
          <Typography
            component="h3"
            variant="h5"
            style={{ fontWeight: "bold" }}
            hidden={!this.state.isIntro}
          >
            Email a group with one click
          </Typography>
          <Avatar className={classes.avatar} hidden={this.state.isIntro}>
            <LockOutlinedIcon />
          </Avatar>
          <Typography component="h1" variant="h5" hidden={this.state.isIntro}>
            Sign up
          </Typography>
          <form className={classes.form} noValidate onSubmit={this.submitUser}>
            <Grid container spacing={2}>
              <Grid item xs={12}>
                <TextField
                  error={this.state.usernameErrorState}
                  name="username"
                  label="Username"
                  onChange={this.handleChange}
                  onBlur={this.validateUserName.bind(this)}
                  required
                  variant="outlined"
                  fullWidth
                  autoFocus
                ></TextField>
              </Grid>
              <Grid item xs={12}>
                <TextField
                  error={this.state.passwordErrorState}
                  name="password"
                  type="password"
                  label="Password"
                  onChange={this.handleChange}
                  onBlur={this.validatePassword.bind(this)}
                  required
                  variant="outlined"
                  fullWidth
                ></TextField>
              </Grid>
              <Grid item xs={12}>
                <TextField
                  error={this.state.confirmPasswordErrorState}
                  helperText={this.state.confirmPasswordErrorMsg}
                  name="confirmPassword"
                  type="password"
                  label="Confirm Password"
                  onChange={this.handleChange}
                  onBlur={this.validateConfirmPassword.bind(this)}
                  required
                  variant="outlined"
                  fullWidth
                ></TextField>
              </Grid>
              <Grid item xs={12}>
                <TextField
                  error={this.state.emailErrorState}
                  helperText={this.state.emailErrorMsg}
                  name="email"
                  type="email"
                  label="Email"
                  onChange={this.handleChange}
                  onBlur={this.validateEmail.bind(this)}
                  variant="outlined"
                  fullWidth
                ></TextField>
              </Grid>
            </Grid>
            <SubmitButton
              type="submit"
              fullWidth
              variant="contained"
              color="primary"
              className={classes.submit}
              disabled={this.state.submitting}
            >
              {this.state.isIntro ? "Get started" : "Sign up"}
            </SubmitButton>

            <Grid container justify="flex-end">
              <Grid item>
                <Link
                  to={{
                    pathname: "/login",
                  }}
                >
                  Already have an account? Sign in
                </Link>
              </Grid>
            </Grid>
          </form>
        </div>
      </Container>
    );
  }
}
Signup.propTypes = {
  classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(Signup);
