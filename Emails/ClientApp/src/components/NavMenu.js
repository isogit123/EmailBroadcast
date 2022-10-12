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

  async signOutFromOtherDevices() {
    let resp = await fetch("api/users/SignOutFromOtherDevices");
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
                          Change email
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
                        id="logoutlink"
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
                            await this.signOutFromOtherDevices()
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
            <Link to="/login" ref={this.setLogoutRef}></Link>
          </Container>
        </Navbar>
      </header>
    );
  }
}
