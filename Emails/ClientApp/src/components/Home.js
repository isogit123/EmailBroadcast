import React, { Component } from "react";
import { Link } from "react-router-dom";
import { removeConfirmation, viewError, getCookie } from "./util";
//Design
import Avatar from "@material-ui/core/Avatar";
import CssBaseline from "@material-ui/core/CssBaseline";
import FormControlLabel from "@material-ui/core/FormControlLabel";
import Checkbox from "@material-ui/core/Checkbox";
import CircularProgress from "@material-ui/core/CircularProgress";
import IconButton from "@material-ui/core/IconButton";
import Grid from "@material-ui/core/Grid";
import Box from "@material-ui/core/Box";
import Email from "@material-ui/icons/Email";
import Add from "@material-ui/icons/Add";
import Delete from "@material-ui/icons/Delete";
import Group from "@material-ui/icons/Group";
import Typography from "@material-ui/core/Typography";
import { makeStyles, withStyles } from "@material-ui/core/styles";
import Container from "@material-ui/core/Container";
import Button from "@material-ui/core/Button";
import PropTypes from "prop-types";
import Card from "@material-ui/core/Card";
import CardActions from "@material-ui/core/CardActions";
import CardContent from "@material-ui/core/CardContent";
import SubmitButton from "./SubmitButton";
import { Fab } from "@material-ui/core";

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
    margin: theme.spacing(1),
  },
  fab: {
    textAlign: "right",
  },
});

class Home extends Component {
  static displayName = Home.name;
  constructor() {
    super();
  }
  state = {
    groups: [],
    loading: true,
  };

  async getGroups() {
    const url = "api/group/get";
    const response = await fetch(url);
    if (response.status == 401) this.props.history.push("/intro");
    else if (response.status == 200) {
      const data = await response.json();
      const newState = { groups: data };
      this.setState(newState);
    } else viewError("Website not available", "Please try again later");
  }
  async componentDidMount() {
    await this.getGroups();
    this.setState({ loading: false });
  }

  async removeGroup(groupId) {
    let answer = await removeConfirmation();
    if (answer.isConfirmed) {
      fetch("api/group/delete", {
        method: "POST",
        body: JSON.stringify({ groupId: groupId }),
        headers: {
          "Content-Type": "application/json",
          "X-CSRF-TOKEN": getCookie("CSRF-TOKEN"),
        },
      }).then(async (response) =>
        response.status == 200
          ? await this.getGroups()
          : viewError("Error deleting group")
      );
    }
  }

  render() {
    const { classes } = this.props;
    return (
      <Container component="main" maxWidth="">
        <CssBaseline />
        <div className={classes.paper}>
          <Avatar className={classes.avatar}>
            <Group />
          </Avatar>
          <Typography component="h1" variant="h5">
            Groups
          </Typography>
          {this.state.loading && (
            <CircularProgress style={{ marginTop: "2%" }} />
          )}
          <Grid container>
            <Grid item xs={12}>
              {!this.state.loading &&
                this.state.groups.map((item, index) => {
                  return (
                    <Card className={classes.card}>
                      <CardContent>
                        <Typography variant="h5" component="h2">
                          {item.name}
                        </Typography>
                      </CardContent>
                      <CardActions>
                        <Button
                          component={Link}
                          to={{
                            pathname: "/MailGroup",
                            state: {
                              groupId: item.id,
                            },
                          }}
                        >
                          Send Email
                        </Button>
                        <Button
                          component={Link}
                          to={{
                            pathname: "/EditGroup",
                            state: {
                              id: item.id,
                            },
                          }}
                        >
                          Edit
                        </Button>
                        <Button onClick={async () => this.removeGroup(item.id)}>
                          Remove
                        </Button>
                      </CardActions>
                    </Card>
                  );
                })}
              {!this.state.loading && (
                <div className={classes.fab}>
                  <Link
                    to={{
                      pathname: "/AddGroup",
                    }}
                  >
                    <Fab color="primary" aria-label="add">
                      <Add />
                    </Fab>
                  </Link>
                </div>
              )}
            </Grid>
          </Grid>
        </div>
      </Container>
    );
  }
}
Home.propTypes = {
  classes: PropTypes.object.isRequired,
};
export default withStyles(styles)(Home);
