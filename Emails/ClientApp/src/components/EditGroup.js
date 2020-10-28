import React, { Component } from 'react';
import { viewError } from './util';
import GroupForm from './GroupForm'
import CircularProgress from '@material-ui/core/CircularProgress';

export class EditGroup extends Component {
    state = {
        name: '',
        nameErrorState: false,
        email: '',
        emailErrorState: false,
        emailValidationError: '',
        emails: [{}],
        apiEndPoint: 'edit',
        id: '',
        loading: true,
        loadingFailure: false
    }
    componentDidMount() {
        const { id } = this.props.location.state;
        fetch('api/group/getbyid/?groupId=' + id)
            .then(response => {
                if (response.status == 200) {
                    return response.json();
                }
                else if (response.status == 401)
                    this.props.history.push("/login")
                else{
                    viewError('Error fetching group data')
                    this.setState({ loading: false, loadingFailure: true })
                }
            })
            .then(data => {
                if (typeof (data) != 'undefined'){
                    this.setState({ name: data.name, emails: data.emails, id: id })
                    this.setState({ loading: false })
                }
            })
    }
    render() {
        return (
            <GroupForm data={this.state} history={this.props.history} title='Edit Group' />
        )
    }
}