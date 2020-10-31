import React, { Component } from "react";
import { Route } from "react-router";
import { Layout } from "./components/Layout";
import Home from "./components/Home";

import "./custom.css";
import AddGroup from "./components/AddGroup";
import EditGroup from "./components/EditGroup";
import Signup from "./components/Signup";
import Login from "./components/Login";
import MailGroup from "./components/MailGroup";
import ResetPassword from "./components/ResetPassword";
import ResetPasswordRequest from "./components/ResetPasswordRequest";
import ChangeEmail from "./components/ChangeEmail";
import Intro from "./components/Intro";
import EmailsList from "./components/EmailsList";
import EmailDetails from "./components/EmailDetails";

export default class App extends Component {
  static displayName = App.name;
  animationConfig = {
    offset: -100,
    stiffness: 120,
    damping: 17,
  };
  render() {
    return (
      <Layout>
        <Route exact path="/" component={Home} />
        <Route path="/AddGroup" component={AddGroup} />
        <Route path="/EditGroup/:id" component={EditGroup} />
        <Route path="/Signup" component={Signup} />
        <Route path="/Login" component={Login} />
        <Route path="/MailGroup/:groupId" component={MailGroup} />
        <Route path="/ResetPassword" component={ResetPassword} />
        <Route path="/ResetPasswordRequest" component={ResetPasswordRequest} />
        <Route path="/ChangeEmail" component={ChangeEmail} />
        <Route path="/Intro" component={Intro} />
        <Route path="/EmailsList" component={EmailsList} />
        <Route path="/EmailDetails/:emailId" component={EmailDetails} />
      </Layout>
    );
  }
}
