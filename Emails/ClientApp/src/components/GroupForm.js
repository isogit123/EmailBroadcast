import React, { Component } from "react";
import { Button, TextField, IconButton } from "@material-ui/core";
import {
  getCookie,
  testEmail,
  viewError,
  removeConfirmation,
  checkSession,
} from "./util";
//Design
import Avatar from "@material-ui/core/Avatar";
import CssBaseline from "@material-ui/core/CssBaseline";
import FormControlLabel from "@material-ui/core/FormControlLabel";
import Checkbox from "@material-ui/core/Checkbox";
import Grid from "@material-ui/core/Grid";
import CircularProgress from "@material-ui/core/CircularProgress";
import Add from "@material-ui/icons/Add";
import Group from "@material-ui/icons/Group";
import Typography from "@material-ui/core/Typography";
import { makeStyles, withStyles } from "@material-ui/core/styles";
import Container from "@material-ui/core/Container";
import PropTypes from "prop-types";
import MailsTable from "./MailsTable";
import Card from "@material-ui/core/Card";
import CardActions from "@material-ui/core/CardActions";
import CardContent from "@material-ui/core/CardContent";
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
  separator: {
    margin: theme.spacing(0),
    width: "100%",
  },
  card: {
    width: "100%",
  },
  title: {
    fontSize: 14,
  },
  pos: {
    marginBottom: 12,
    marginTop: 12,
  },
});

class GroupForm extends Component {
  constructor(props) {
    super(props);
    this.removeEmail = this.removeEmail.bind(this);
    this.submitGroup = this.submitGroup.bind(this);
    this.addEmail = this.addEmail.bind(this);
    this.emailInput = null;
    this.setEmailInputRef = (element) => {
      this.emailInput = element;
    };
  }
  state = {
    name: "",
    nameErrorState: false,
    email: "",
    emailErrorState: false,
    emailValidationError: "",
    emails: new Array(),
    apiEndPoint: "add",
    id: "",
    submitting: false,
    loading: true,
    loadingFailure: false,
  };
  componentDidMount() {
    checkSession();
    if (typeof this.props.add != "undefined") this.setState({ loading: false });
  }
  componentWillReceiveProps(nextProps) {
    this.setState(nextProps.data);
  }
  handleChange = (event) => {
    const enteredValueName = event.target.name;
    this.setState({ [enteredValueName]: event.target.value });
  };
  validateGroupName() {
    let isValid = true;
    if (!this.state.name) {
      isValid = false;
      this.setState({ nameErrorState: true });
    } else {
      this.setState({ nameErrorState: false });
    }
    return isValid;
  }
  validateEmail() {
    let isValid = true;
    if (!this.state.email && this.state.emails.length === 0) {
      isValid = false;
      this.setState({ emailErrorState: true, emailValidationError: "" });
    } else if (this.state.email && !testEmail(this.state.email)) {
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
  addEmail = () => {
    if (!this.validateEmail()) {
      return;
    }
    const emails = [...this.state.emails];
    let isFound = false;
    emails.some((item) => {
      let enteredEmail = this.state.email.toLowerCase()
      if (item == enteredEmail) {
        isFound = true;
        return true;
      }
    });
    if (!isFound) {
      emails.push(this.state.email.toLowerCase());
      this.setState({
        emails: emails,
        emailErrorState: false,
        emailValidationError: "",
        email: "",
      });
    } else {
      this.setState({
        emailErrorState: true,
        emailValidationError: "Email already added",
      });
    }
    this.emailInput.querySelector("input").focus();
  };
  async removeEmail(email) {
    let answer = await removeConfirmation();
    if (answer.isConfirmed)
      this.setState({
        emails: this.state.emails.filter((e) => e != email),
      });
  }
  validateForm() {
    let isValid = true;
    if (this.state.emails.length === 0) {
      this.setState({ emailErrorState: true, emailValidationError: "" });
      isValid = false;
    }
    return this.validateGroupName() && isValid;
  }
  submitGroup = (event) => {
    event.preventDefault();
    if (!this.validateForm()) return;
    this.setState({ submitting: true });
    let payload = {};
    if (this.state.apiEndPoint.startsWith("e"))
      payload = {
        id: this.state.id,
        name: this.state.name,
        emails: this.state.emails,
      };
    else payload = { name: this.state.name, emails: this.state.emails };
    fetch("api/group/" + this.state.apiEndPoint, {
      method: "POST",
      body: JSON.stringify(payload),
      headers: {
        "Content-Type": "application/json",
        "X-CSRF-TOKEN": getCookie("CSRF-TOKEN"),
      },
    }).then((response) => {
      if (response.status == 200) this.props.history.push("/");
      else {
        viewError("Unexpected Failure");
        this.setState({ submitting: false });
      }
    });
  };
  render() {
    const { classes } = this.props;
    return (
      <Container component="main" maxWidth="xs">
        <CssBaseline />
        <div className={classes.paper}>
          <Avatar className={classes.avatar}>
            <Group />
          </Avatar>
          <Typography component="h1" variant="h5">
            {this.props.title}
          </Typography>
          {this.state.loading && !this.state.loadingFailure && (
            <CircularProgress style={{ marginTop: "2%" }} />
          )}
          {!this.state.loading && !this.state.loadingFailure && (
            <form
              className={classes.form}
              noValidate
              onSubmit={this.submitGroup}
            >
              <Grid container spacing={2}>
                <Grid item xs={12}>
                  <TextField
                    type="text"
                    name="name"
                    error={this.state.nameErrorState}
                    onChange={this.handleChange}
                    label="Group name"
                    onBlur={this.validateGroupName.bind(this)}
                    required
                    variant="outlined"
                    fullWidth
                    autoFocus
                    value={this.state.name}
                  ></TextField>
                </Grid>
                <Card className={classes.card}>
                  <CardContent>
                    <Typography variant="h5" component="h2">
                      Group Emails
                    </Typography>
                    <Grid container className={classes.pos}>
                      <Grid item xs={11}>
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
                          fullWidth
                          ref={this.setEmailInputRef}
                        ></TextField>
                      </Grid>
                      <Grid item xs={1}>
                        <IconButton
                          type="button"
                          onClick={this.addEmail}
                          fullWidth
                        >
                          <Add />
                        </IconButton>
                      </Grid>
                      <Grid
                        item
                        xs={12}
                        style={{ overflow: "auto", height: "200px" }}
                      >
                        <MailsTable
                          emails={this.state.emails}
                          removeEmail={this.removeEmail}
                        />
                      </Grid>
                    </Grid>
                  </CardContent>
                  <CardActions></CardActions>
                </Card>
                <div style={{ textAlign: "center", width: "100%" }}>
                  <SubmitButton
                    type="submit"
                    variant="contained"
                    color="primary"
                    className={classes.submit}
                    disabled={this.state.submitting}
                    fullWidth
                  >
                    {this.props.title}
                  </SubmitButton>
                </div>
              </Grid>
            </form>
          )}
        </div>
      </Container>
    );
  }
}

GroupForm.propTypes = {
  classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(GroupForm);
