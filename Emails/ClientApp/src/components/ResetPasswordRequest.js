import React, { Component } from "react";
import { Button, TextField, FormControl } from "@material-ui/core";
import { getCookie, viewSuccess, viewError, testEmail } from "./util";
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

class ResetPasswordRequest extends Component {
  state = {
    email: "",
    emailErrorState: false,
    emailErrorMsg: "",
    submitting: false,
  };
  handleChange = (event) => {
    const enteredValueName = event.target.name;
    this.setState({ [enteredValueName]: event.target.value });
  };
  validateEmail() {
    let isValid = true;
    if (!this.state.email) {
      this.setState({ emailErrorState: true, emailErrorMsg: "" });
      isValid = false;
    } else if (!testEmail(this.state.email)) {
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
    return this.validateEmail();
  }
  submitRequest = async (event) => {
    event.preventDefault();
    if (this.validateForm()) {
      this.setState({ submitting: true });
      let resp = await fetch(
        "api/users/GeneratePasswordToken/?userEmail=" + this.state.email,
        {}
      );
      if (resp.status == 200) {
        let data = await resp.text();
        if (data == "-1") viewError("Email not found");
        else if (data == "-2")
          viewError("Email not confirmed", "Confirmation email has been sent");
        else viewSuccess("Email sent successfully");
      } else viewError("Error processing your request");
      this.setState({ submitting: false });
    }
  };

  render() {
    const { classes } = this.props;
    return (
      <Container component="main" maxWidth="xs">
        <CssBaseline />
        <div className={classes.paper}>
          <Avatar className={classes.avatar}>
            <SettingsBackupRestore />
          </Avatar>
          <Typography component="h1" variant="h5">
            Forgot Password
          </Typography>

          <form
            className={classes.form}
            noValidate
            onSubmit={this.submitRequest}
          >
            <TextField
              error={this.state.emailErrorState}
              helperText={this.state.emailErrorMsg}
              name="email"
              type="email"
              label="Email"
              onChange={this.handleChange}
              onBlur={this.validateEmail.bind(this)}
              required
              variant="outlined"
              margin="normal"
              fullWidth
              autoFocus
            ></TextField>
            <SubmitButton
              type="submit"
              fullWidth
              variant="contained"
              color="primary"
              className={classes.submit}
              disabled={this.state.submitting}
            >
              Submit
            </SubmitButton>
          </form>
        </div>
      </Container>
    );
  }
}
ResetPasswordRequest.propTypes = {
  classes: PropTypes.object.isRequired,
};
export default withStyles(styles)(ResetPasswordRequest);
