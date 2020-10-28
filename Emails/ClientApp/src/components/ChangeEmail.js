import React, { Component } from "react";
import { Button, TextField, FormControl } from "@material-ui/core";
import { getCookie, testEmail, viewError, viewSuccess } from "./util";
//Design
import Avatar from "@material-ui/core/Avatar";
import CssBaseline from "@material-ui/core/CssBaseline";
import FormControlLabel from "@material-ui/core/FormControlLabel";
import Checkbox from "@material-ui/core/Checkbox";
import Grid from "@material-ui/core/Grid";
import Box from "@material-ui/core/Box";
import Edit from "@material-ui/icons/Edit";
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
    width: "100%",
  },
});

export class ChangeEmail extends Component {
  state = {
    email: "",
    emailErrorState: false,
    emailValidationError: "",
    submitting: false,
  };
  handleChange = (event) => {
    const enteredValueName = event.target.name;
    this.setState({ [enteredValueName]: event.target.value });
  };
  validateEmail() {
    let isValid = true;
    if (!this.state.email) {
      this.setState({ emailErrorState: true, emailValidationError: "" });
      isValid = false;
    } else if (!testEmail(this.state.email)) {
      isValid = false;
      this.setState({
        emailErrorState: true,
        emailValidationError: "Email not valid",
      });
    } else {
      this.setState({ emailErrorState: false, emailValidationError: "" });
    }
    return isValid;
  }
  submit = (event) => {
    event.preventDefault();
    if (!this.validateEmail()) return;
    this.setState({ submitting: true });
    fetch("api/users/editemail", {
      method: "POST",
      body: JSON.stringify({ newEmail: this.state.email }),
      headers: {
        "Content-Type": "application/json",
        "X-CSRF-TOKEN": getCookie("CSRF-TOKEN"),
      },
    })
      .then((response) => {
        if (response.status == 200) response.text();
        else {
          viewError("Error changing email");
          this.setState({ submitting: false });
        }
      })
      .then((data) => {
        if (data != "-1")
          viewSuccess(
            "Email changed successfully",
            "Confirmation email has been sent"
          );
        else {
          viewError("Error changing email");
        }
        this.setState({ submitting: false });
      });
  };
  render() {
    const { classes } = this.props;
    return (
      <Container component="main" maxWidth="xs">
        <CssBaseline />
        <div className={classes.paper}>
          <Avatar className={classes.avatar}>
            <Edit />
          </Avatar>
          <Typography component="h1" variant="h5">
            Change Email
          </Typography>

          <form className={classes.form} noValidate onSubmit={this.submit}>
            <TextField
              type="email"
              error={this.state.emailErrorState}
              helperText={this.state.emailValidationError}
              name="email"
              onChange={this.handleChange}
              label="Email"
              value={this.state.email}
              onBlur={this.validateEmail.bind(this)}
              variant="outlined"
              margin="normal"
              fullWidth
              autoFocus
            ></TextField>
            <SubmitButton
              type="submit"
              disabled={this.state.submitting}
              variant="contained"
              color="primary"
              className={classes.submit}
              fullWidth
            >
              Submit
            </SubmitButton>
          </form>
        </div>
      </Container>
    );
  }
}
ChangeEmail.propTypes = {
  classes: PropTypes.object.isRequired,
};
export default withStyles(styles)(ChangeEmail);
