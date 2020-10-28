import React, { Component } from "react";
import GroupForm from "./GroupForm";

class AddGroup extends Component {
  constructor() {
    super();
  }
  state = {
    name: "",
    nameErrorState: false,
    email: "",
    emailErrorState: false,
    emailValidationError: "",
    emails: [{}],
    apiEndPoint: "add",
    loading: false,
    loadingFailure: false,
  };
  render() {
    return <GroupForm add history={this.props.history} title="Add Group" />;
  }
}

export default AddGroup;
