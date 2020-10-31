import React, { Component } from "react";
import { Container } from "reactstrap";
import { NavMenu } from "./NavMenu";
import Typography from "@material-ui/core/Typography";
import Box from "@material-ui/core/Box";
import { Link } from "react-router-dom";
import LoggedInUserName from "./Context";
import { viewError } from "./util";

const styles = {};

export class Layout extends Component {
  static displayName = Layout.name;
  state = {
    userName: "",
    setUserName: async (userName = null) => {
      if (userName != null) this.setState({ userName: userName });
      else {
        let resp = await fetch("api/users/GetLoggedInUsername");
        if (resp.status == 200) {
          let name = await resp.text();
          this.setState({ userName: name });
        } else viewError("Error logging out");
      }
    },
  };
  render() {
    return (
      <div>
        <LoggedInUserName.Provider value={this.state}>
          <NavMenu />
          <Container>{this.props.children}</Container>
        </LoggedInUserName.Provider>
        <Box mt={5}>
          <Typography variant="body2" color="textSecondary" align="center">
            {"Licensed under MIT License"}
          </Typography>
          <Typography variant="body2" color="textSecondary" align="center">
            {"Designed using "}
            <Link href="https://material-ui.com/">Material UI</Link>{" "}
            {"templates"}
          </Typography>
          <Link
            id="loginlink"
            to={{
              pathname: "/Login",
            }}
          ></Link>
        </Box>
      </div>
    );
  }
}
