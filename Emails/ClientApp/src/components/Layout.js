import React, { Component } from "react";
import { Container } from "reactstrap";
import { NavMenu } from "./NavMenu";
import Typography from "@material-ui/core/Typography";
import Box from "@material-ui/core/Box";
import { Link } from "react-router-dom";
import LoggedInUserName from "./Context";
import { viewError } from "./util";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { brands } from "@fortawesome/fontawesome-svg-core/import.macro";

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
            {
              <a
                href="https://github.com/isogit123/EmailBroadcast"
                target="_blank"
              >
                <FontAwesomeIcon icon={brands("github")} size="2x" />
              </a>
            }
          </Typography>
          <Typography variant="body2" color="textSecondary" align="center">
            {"Licensed under MIT License"}
          </Typography>
          <Typography variant="body2" color="textSecondary" align="center">
            {"Designed using "}
            <a href="https://material-ui.com/" target="_blank">
              Material UI
            </a>{" "}
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
