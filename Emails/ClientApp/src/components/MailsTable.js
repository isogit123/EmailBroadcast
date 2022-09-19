import React, { Component } from 'react';
import { Button, IconButton } from '@material-ui/core';
import Delete from '@material-ui/icons/Delete';

class MailsTable extends Component {
    state = {
        emails: this.props.emails
    }
    classes = {
        middle: {
            display: 'flex',
            alignItems: 'center',
            height: '100%',
            justifyContent: 'center'
        }
    }
    componentWillReceiveProps(nextProps) {
        this.setState({ emails: nextProps.emails });
    }
    render() {
        if (this.state.emails.length === 0)
            return (
                <div style={this.classes.middle }>
                    <b>No Emails Added</b>
                </div>
            )
        return (
            <table>
                <thead>
                </thead>
                <tbody>
                    {
                        this.state.emails.map((item, index) => {
                            return (
                                <tr key={index}>
                                    <td>
                                        {item}
                                    </td>
                                    <td>
                                        <IconButton onClick={async () => await this.props.removeEmail(item)}><Delete /></IconButton>
                                    </td>
                                </tr>
                            )
                        })
                    }

                </tbody>
            </table>

        )
    }
}
export default MailsTable