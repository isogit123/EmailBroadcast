import React, { Component } from "react";
import Signup from "./Signup";
import { makeStyles, withStyles } from "@material-ui/core/styles";
import { CssBaseline, Container, Typography } from "@material-ui/core";

const styles = (theme) => ({
  paper: {
    marginTop: theme.spacing(8),
    display: "flex",
    flexDirection: "column",
    alignItems: "center",
  },
});

class Intro extends Component {
  render() {
    return <Signup intro></Signup>;
  }
}
export default Intro;
