import React, { Component } from "react";
import {
  Button,
  TextField,
  FormControl,
  CircularProgress,
} from "@material-ui/core";
import { getCookie, viewSuccess, viewError } from "./util";
//Design
import Avatar from "@material-ui/core/Avatar";
import CssBaseline from "@material-ui/core/CssBaseline";
import FormControlLabel from "@material-ui/core/FormControlLabel";
import Checkbox from "@material-ui/core/Checkbox";
import Grid from "@material-ui/core/Grid";
import Box from "@material-ui/core/Box";
import SettingsBackupRestore from "@material-ui/icons/SettingsBackupRestore";
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
    marginTop: theme.spacing(1),
  },
  submit: {
    margin: theme.spacing(3, 0, 2),
  },
});

class ResetPassword extends Component {
  state = {
    password: "",
    confirmPassword: "",
    passwordErrorState: false,
    confirmPasswordErrorState: false,
    confirmPasswordMsg: "",
    token: "",
    submitting: false,
    loading: true,
    loadingFailure: false,
  };
  componentDidMount() {
    const token = window.location.href.split("token=")[1];
    if (!token) this.setState({ loading: false, loadingFailure: true });
    else
      fetch("api/users/ValidatePasswordToken/?token=" + token, {})
        .then((response) => {
          if (response.status == 200) response.text();
          else {
            viewError("Error processing request");
            this.setState({ loading: false, loadingFailure: true });
          }
        })
        .then((data) => {
          if (data == "-1") {
            this.setState({ token: "" });
            this.setState({ loading: false, loadingFailure: true });
          } else {
            this.setState({ token: token });
            this.setState({ loading: false });
          }
          this.setState({ loading: false });
        })
        .catch(() => viewError("Error adding user"));
  }
  handleChange = (event) => {
    const enteredValueName = event.target.name;
    this.setState({ [enteredValueName]: event.target.value });
  };
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
        confirmPasswordMsg: "",
      });
    } else if (this.state.confirmPassword !== this.state.password) {
      isValid = false;
      this.setState({
        confirmPasswordErrorState: false,
        confirmPasswordMsg: "Passwords do not match",
      });
    } else {
      this.setState({
        confirmPasswordErrorState: false,
        confirmPasswordMsg: "",
      });
    }
    return isValid;
  }
  validateForm() {
    return this.validatePassword() && this.validateConfirmPassword();
  }
  submitPassword = async (event) => {
    event.preventDefault();
    if (this.validateForm()) {
      this.setState({ submitting: true });
      let resp = await fetch("api/users/EditPassword", {
        method: "POST",
        body: JSON.stringify({
          token: this.state.token,
          newPassword: this.state.password,
        }),
        headers: {
          "Content-Type": "application/json",
          "X-CSRF-TOKEN": getCookie("CSRF-TOKEN"),
        },
      });
      if (resp.status == 200) {
        let data = await resp.text();
        if (data != "-1") {
          viewSuccess("Pasword reset successfully");
          this.props.history.push("/login");
        } else viewError("User not found");
      } else viewError("Error loging in");
      this.setState({ submitting: false });
    }
  };

  render() {
    const { classes } = this.props;
    if (this.state.token === "" && this.state.loadingFailure)
      return (
        <div>
          <h1>Invalid request</h1>
        </div>
      );
    else
      return (
        <Container component="main" maxWidth="xs">
          <CssBaseline />
          <div className={classes.paper}>
            <Avatar className={classes.avatar}>
              <SettingsBackupRestore />
            </Avatar>
            <Typography component="h1" variant="h5">
              Reset Password
            </Typography>
            {this.state.loading && (
              <CircularProgress style={{ marginTop: "2%" }} />
            )}
            {!this.state.loading && !this.state.loadingFailure && (
              <form
                className={classes.form}
                noValidate
                onSubmit={this.submitPassword}
              >
                <TextField
                  error={this.state.usernameErrorState}
                  name="password"
                  label="Password"
                  type="password"
                  onChange={this.handleChange}
                  onBlur={this.validatePassword.bind(this)}
                  required
                  variant="outlined"
                  margin="normal"
                  fullWidth
                  autoFocus
                ></TextField>
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
                  margin="normal"
                  fullWidth
                ></TextField>
                <SubmitButton
                  type="submit"
                  fullWidth
                  variant="contained"
                  color="primary"
                  className={classes.submit}
                  disabled={this.state.submitting}
                >
                  Reset password
                </SubmitButton>
              </form>
            )}
          </div>
        </Container>
      );
  }
}
ResetPassword.propTypes = {
  classes: PropTypes.object.isRequired,
};
export default withStyles(styles)(ResetPassword);
