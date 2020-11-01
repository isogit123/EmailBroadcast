import React, { Component } from "react";
import {
  Button,
  TextField,
  FormControl,
  createMuiTheme,
} from "@material-ui/core";
import { getCookie, viewSuccess, viewError, checkSession } from "./util";
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
import LoggedInUserName from "./Context";

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
    marginTop: theme.spacing(1),
  },
  submit: {
    margin: theme.spacing(3, 0, 2),
  },
});
class Login extends Component {
  state = {
    username: "",
    password: "",
    usernameErrorState: false,
    passwordErrorState: false,
    submitting: false,
  };
  static contextType = LoggedInUserName;
  async componentDidMount() {
    if (window.location.href.split("v=")[1] === "1")
      viewSuccess("Email verified successfully");

    fetch("api/users/checksession").then((response) => {
      if (response.status == 200) {
        this.props.history.push("/");
      }
    });
  }
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
  validateForm() {
    return this.validateUserName() && this.validatePassword();
  }
  submitUser = async (event) => {
    event.preventDefault();
    if (this.validateForm()) {
      this.setState({ submitting: true });
      let resp = await fetch("api/users/login", {
        method: "POST",
        body: JSON.stringify({
          username: this.state.username,
          password: this.state.password,
        }),
        headers: {
          "Content-Type": "application/json",
          "X-CSRF-TOKEN": getCookie("CSRF-TOKEN"),
        },
      });
      if (resp.status == 200) {
        let data = await resp.text();
        if (data != "-1") {
          this.context.setUserName(this.state.username);
          let requestedUrl = sessionStorage.getItem("requestedUrl");
          if (requestedUrl) {
            sessionStorage.removeItem("requestedUrl");
            this.props.history.push(requestedUrl);
          } else this.props.history.push("/");
        } else {
          viewError("User not found");
          this.setState({ submitting: false });
        }
      } else {
        viewError("Error loging in");
        this.setState({ submitting: false });
      }
    }
  };
  render() {
    const { classes } = this.props;
    return (
      <Container component="main" maxWidth="xs">
        <CssBaseline />
        <div className={classes.paper}>
          <Avatar className={classes.avatar}>
            <LockOutlinedIcon />
          </Avatar>
          <Typography component="h1" variant="h5">
            Sign in
          </Typography>
          <form className={classes.form} noValidate onSubmit={this.submitUser}>
            <TextField
              error={this.state.usernameErrorState}
              name="username"
              label="Username"
              onChange={this.handleChange}
              onBlur={this.validateUserName.bind(this)}
              required
              variant="outlined"
              margin="normal"
              fullWidth
              id="email"
              autoComplete="email"
              autoFocus
            ></TextField>
            <TextField
              error={this.state.passwordErrorState}
              name="password"
              type="password"
              label="Password"
              onChange={this.handleChange}
              onBlur={this.validatePassword.bind(this)}
              required
              variant="outlined"
              margin="normal"
              fullWidth
              id="password"
            ></TextField>
            <SubmitButton
              type="submit"
              fullWidth
              variant="contained"
              color="primary"
              className={classes.submit}
              disabled={this.state.submitting}
            >
              Login
            </SubmitButton>
            <Grid container>
              <Grid item xs>
                <Link
                  to={{
                    pathname: "/resetpasswordrequest",
                  }}
                >
                  Forgot password?
                </Link>
              </Grid>
              <Grid item>
                <Link
                  to={{
                    pathname: "/signup",
                  }}
                >
                  Don't have an account? Sign Up
                </Link>
              </Grid>
            </Grid>
          </form>
        </div>
      </Container>
    );
  }
}
Login.propTypes = {
  classes: PropTypes.object.isRequired,
};
export default withStyles(styles)(Login);
