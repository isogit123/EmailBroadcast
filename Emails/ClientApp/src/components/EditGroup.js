import React, { Component } from "react";
import { viewError } from "./util";
import GroupForm from "./GroupForm";
import { withRouter } from "react-router-dom";
import CircularProgress from "@material-ui/core/CircularProgress";

class EditGroup extends Component {
  state = {
    name: "",
    nameErrorState: false,
    email: "",
    emailErrorState: false,
    emailValidationError: "",
    emails: [{}],
    apiEndPoint: "edit",
    id: "",
    loading: true,
    loadingFailure: false,
  };
  componentDidMount() {
    let id = this.props.match.params.id;
    fetch("api/group/getbyid/?groupId=" + id)
      .then((response) => {
        if (response.status == 200) {
          return response.json();
        } else if (response.status != 401) {
          viewError("Error fetching group data");
          this.setState({ loading: false, loadingFailure: true });
        }
      })
      .then((data) => {
        if (typeof data != "undefined") {
          this.setState({ name: data.name, emails: data.emails, id: id });
          this.setState({ loading: false });
        }
      });
  }
  render() {
    return (
      <GroupForm
        data={this.state}
        history={this.props.history}
        title="Edit Group"
      />
    );
  }
}
export default withRouter(EditGroup);
