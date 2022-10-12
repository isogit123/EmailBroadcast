import React, { Component } from "react";
import { Link, withRouter } from "react-router-dom";
import {
  removeConfirmation,
  viewError,
  getCookie,
  viewSuccess,
  checkSession,
} from "./util";
import {
  TextField,
  Button,
  FormControl,
  IconButton,
  CircularProgress,
} from "@material-ui/core";
//Design
import Avatar from "@material-ui/core/Avatar";
import CssBaseline from "@material-ui/core/CssBaseline";
import FormControlLabel from "@material-ui/core/FormControlLabel";
import Checkbox from "@material-ui/core/Checkbox";
import Grid from "@material-ui/core/Grid";
import Box from "@material-ui/core/Box";
import Email from "@material-ui/icons/Email";
import Add from "@material-ui/icons/Add";
import Delete from "@material-ui/icons/Delete";
import Typography from "@material-ui/core/Typography";
import { makeStyles, withStyles } from "@material-ui/core/styles";
import Container from "@material-ui/core/Container";
import PropTypes from "prop-types";
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
    marginTop: theme.spacing(1),
  },
  submit: {
    margin: theme.spacing(3, 0, 2),
  },
});

class MailGroup extends Component {
  constructor() {
    super();
    this.addFile = this.addFile.bind(this);
  }
  state = {
    subject: "",
    htmlContent: "",
    attachments: new Map(),
    loading: true,
    submitting: false,
    groupName: "...",
    attachmentsSize: 0,
    groupId: 0,
  };
  componentDidMount() {
    checkSession();
    let groupId = this.props.match.params.groupId;
    this.state.groupId = groupId;
    fetch("api/group/getbyid/?groupId=" + groupId, {})
      .then((response) => {
        if (response.status == 200) return response.json();
        else if (response.status != 401) {
          viewError("Error fetching data");
          this.setState({ loading: false });
        }
      })
      .then((json) => {
        if (typeof json != "undefined")
          this.setState({ groupName: json.name, loading: false });
      });
  }
  handleChange = (event) => {
    const enteredValueName = event.target.name;
    this.setState({ [enteredValueName]: event.target.value });
  };
  addFile(event) {
    const { attachments } = this.state;
    let { attachmentsSize } = this.state;
    const chosenFiles = event.target.files;
    for (let i = 0; i < chosenFiles.length; i++) {
      const fileSize = chosenFiles[i].size;
      if (attachmentsSize + fileSize < 10485760) {
        //Attachments size should not exceed 10 MB.
        attachments.set(chosenFiles[i].name, chosenFiles[i]);
        attachmentsSize += fileSize;
      } else {
        viewError("Attachments size should not exceed 10 MB");
      }
    }
    this.setState({ attachments });
  }
  async removeFile(fileKey) {
    const answer = await removeConfirmation();
    if (answer.isConfirmed) {
      const { attachments } = this.state;
      attachments.delete(fileKey);
      this.setState({ attachments });
    }
  }
  submitMail(event) {
    event.preventDefault();
    this.setState({ submitting: true });
    const emailData = new FormData();
    emailData.append("groupId", this.state.groupId);
    emailData.append("subject", this.state.subject);
    emailData.append("htmlContent", this.state.htmlContent);
    for (let file of this.state.attachments.values()) {
      emailData.append("attachments", file);
    }
    fetch("api/group/SendMailToGroup", {
      method: "POST",
      body: emailData,
      headers: {
        "X-CSRF-TOKEN": getCookie("CSRF-TOKEN"),
      },
    })
      .then((response) => {
        this.setState({ submitting: true });
        if (response.status == 200) {
          return response.text();
        } else {
          viewError("Error sending email");
          this.setState({ submitting: false });
        }
      })
      .then((data) => {
        if (
          typeof data != "undefined" &&
          data.toLowerCase().startsWith("acc")
        ) {
          viewSuccess("Sending email");
          this.props.history.push("/");
        } else {
          viewError("Error sending email");
          this.setState({ submitting: false });
        }
      });
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
            Email <b>{this.state.groupName}</b>
          </Typography>
          <CircularProgress
            style={{ marginTop: "2%" }}
            hidden={!this.state.loading}
          />
          {!this.state.loading && <form
            className={classes.form}
            noValidate
            onSubmit={this.submitMail.bind(this)}
          >
            <TextField
              name="subject"
              label="Subject"
              onChange={this.handleChange}
              variant="outlined"
              margin="normal"
              fullWidth
              autoFocus
            ></TextField>
            <TextField
              name="htmlContent"
              label="Body"
              multiline
              rows={10}
              variant="outlined"
              onChange={this.handleChange}
              margin="normal"
              fullWidth
            ></TextField>
            <Card className={classes.card}>
              <CardContent>
                <Typography variant="h5" component="h2">
                  Attachments{" "}
                  <IconButton component="label">
                    <Add />{" "}
                    <input
                      type="file"
                      multiple
                      onChange={this.addFile}
                      style={{ display: "none" }}
                    />
                  </IconButton>
                </Typography>
                <Grid container className={classes.pos}>
                  <Grid
                    item
                    xs={12}
                    style={{ overflow: "auto", height: "200px" }}
                  >
                    <table>
                      <thead></thead>
                      <tbody>
                        {Array.from(this.state.attachments.keys()).map(
                          (item, index) => {
                            return (
                              <tr>
                                <td>{item}</td>
                                <td>
                                  <IconButton
                                    onClick={async () =>
                                      await this.removeFile(item)
                                    }
                                  >
                                    <Delete />
                                  </IconButton>
                                </td>
                              </tr>
                            );
                          }
                        )}
                      </tbody>
                    </table>
                  </Grid>
                </Grid>
              </CardContent>
              <CardActions></CardActions>
            </Card>

            <SubmitButton
              type="submit"
              fullWidth
              variant="contained"
              color="primary"
              className={classes.submit}
              disabled={this.state.submitting}
            >
              Send
            </SubmitButton>
          </form>}
        </div>
      </Container>
    );
  }
}
MailGroup.propTypes = {
  classes: PropTypes.object.isRequired,
};
export default withRouter(withStyles(styles)(MailGroup));
