import React, { Component } from "react";
import {
  Collapse,
  Container,
  DropdownItem,
  DropdownMenu,
  DropdownToggle,
  Navbar,
  NavbarBrand,
  NavbarToggler,
  NavItem,
  NavLink,
  UncontrolledDropdown,
} from "reactstrap";
import { Link, Redirect } from "react-router-dom";
import "./NavMenu.css";
import { viewError, viewSuccess } from "./util";
import LoggedInUserName from "./Context";
import PersonOutline from "@material-ui/icons/PersonOutline";
import "../github_corner.css";

export class NavMenu extends Component {
  static displayName = NavMenu.name;

  constructor(props) {
    super(props);

    this.toggleNavbar = this.toggleNavbar.bind(this);
    this.logOut = this.logOut.bind(this);
    this.state = {
      collapsed: true,
    };
    this.logoutLink = null;
    this.setLogoutRef = (element) => (this.logoutLink = element);
  }
  static contextType = LoggedInUserName;

  toggleNavbar() {
    this.setState({
      collapsed: !this.state.collapsed,
    });
  }

  async logOut() {
    let resp = await fetch("api/users/logout");
    if (resp.status == 200) {
      this.logoutLink.click();
      await this.context.setUserName("");
    } else viewError("Error logging out");
  }

  async signOutFromAllDevices() {
    let resp = await fetch("api/users/SignOutFromAllDevices");
    if (resp.status == 200) {
      viewSuccess("Done");
    } else viewError("Error logging out");
  }

  async componentDidMount() {
    await this.context.setUserName();
  }

  render() {
    return (
      <header>
        <Navbar
          className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3"
          light
        >
          <Container>
            <NavbarBrand tag={Link} to="/">
              Email Broadcast
            </NavbarBrand>
            {this.context.userName && (
              <NavbarToggler onClick={this.toggleNavbar} className="ml-auto" />
            )}

            <Collapse
              className="d-sm-inline-flex "
              isOpen={!this.state.collapsed}
              navbar
            >
              <ul
                className="navbar-nav flex-grow"
                hidden={!this.context.userName}
              >
                <NavItem>
                  <NavLink tag={Link} className="text-dark" to="/">
                    View Groups
                  </NavLink>
                </NavItem>
                <NavItem>
                  <NavLink tag={Link} className="text-dark" to="/EmailsList">
                    View Sent Emails
                  </NavLink>
                </NavItem>
              </ul>
              <ul
                className="navbar-nav flex-grow ml-auto"
                hidden={!this.context.userName}
              >
                <UncontrolledDropdown nav inNavbar>
                  <DropdownToggle nav caret>
                    <PersonOutline></PersonOutline> {this.context.userName}
                  </DropdownToggle>
                  <DropdownMenu style={{ left: "-177%" }}>
                    <DropdownItem>
                      <NavItem>
                        <NavLink
                          tag={Link}
                          className="text-dark"
                          to="/changeemail"
                        >
                          Edit email
                        </NavLink>
                      </NavItem>
                    </DropdownItem>
                    <DropdownItem>
                      <NavItem>
                        <NavLink
                          tag={Link}
                          className="text-dark"
                          to="/resetpasswordrequest"
                        >
                          Reset Password
                        </NavLink>
                      </NavItem>
                    </DropdownItem>
                    <DropdownItem>
                      <NavLink
                        tag={Link}
                        className="text-dark"
                        onClick={async () => await this.logOut()}
                      >
                        Log out
                      </NavLink>
                    </DropdownItem>
                    <DropdownItem>
                      <NavItem>
                        <NavLink
                          className="text-dark"
                          onClick={async () =>
                            await this.signOutFromAllDevices()
                          }
                        >
                          Log out from other devices
                        </NavLink>
                      </NavItem>
                    </DropdownItem>
                  </DropdownMenu>
                </UncontrolledDropdown>
              </ul>
            </Collapse>
            {window.location.href.includes("int") && (
              <a
                target="_blank"
                className="ml-auto github-corner"
                href="https://github.com/isogit123/EmailBroadcast"
              >
                <svg
                  width="80"
                  height="80"
                  viewBox="0 0 250 250"
                  style={{
                    fill: "#151513",
                    color: "#fff",
                    position: "absolute",
                    top: 0,
                    border: 0,
                    right: 0,
                  }}
                  aria-hidden="true"
                >
                  <path d="M0,0 L115,115 L130,115 L142,142 L250,250 L250,0 Z"></path>
                  <path
                    d="M128.3,109.0 C113.8,99.7 119.0,89.6 119.0,89.6 C122.0,82.7 120.5,78.6 120.5,78.6 C119.2,72.0 123.4,76.3 123.4,76.3 C127.3,80.9 125.5,87.3 125.5,87.3 C122.9,97.6 130.6,101.9 134.4,103.2"
                    fill="currentColor"
                    style={{ transformOrigin: "130px 106px" }}
                    class="octo-arm"
                  ></path>
                  <path
                    d="M115.0,115.0 C114.9,115.1 118.7,116.5 119.8,115.4 L133.7,101.6 C136.9,99.2 139.9,98.4 142.2,98.6 C133.8,88.0 127.5,74.4 143.8,58.0 C148.5,53.4 154.0,51.2 159.7,51.0 C160.3,49.4 163.2,43.6 171.4,40.1 C171.4,40.1 176.1,42.5 178.8,56.2 C183.1,58.6 187.2,61.8 190.9,65.4 C194.5,69.0 197.7,73.2 200.1,77.6 C213.8,80.2 216.3,84.9 216.3,84.9 C212.7,93.1 206.9,96.0 205.4,96.6 C205.1,102.4 203.0,107.8 198.3,112.5 C181.9,128.9 168.3,122.5 157.7,114.1 C157.9,116.9 156.7,120.9 152.7,124.9 L141.0,136.5 C139.8,137.7 141.6,141.9 141.8,141.8 Z"
                    fill="currentColor"
                    class="octo-body"
                  ></path>
                </svg>
              </a>
            )}
            <Link to="/login" ref={this.setLogoutRef}></Link>
          </Container>
        </Navbar>
      </header>
    );
  }
}
