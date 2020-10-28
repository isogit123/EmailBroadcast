import React, { Component } from 'react';
import { withStyles } from '@material-ui/core/styles';
import CircularProgress from '@material-ui/core/CircularProgress';
import Button from '@material-ui/core/Button';
import PropTypes from 'prop-types';

const styles = theme => ({
    wrapper: {
        position: 'relative',
    },
    buttonProgress: {
        position: 'absolute',
        top: '55%',
        left: '50%',
        marginTop: -12,
        marginLeft: -12,
    },
});

class SubmitButton extends Component {
    render() {
        const { classes } = this.props
        return (
            <div>
                <div className={classes.wrapper}>
                    <Button
                        {...this.props}
                    >
        </Button>
                    {this.props.disabled && <CircularProgress style={{ color: this.props.color} } size={24} className={classes.buttonProgress} />}
                </div>
            </div>
        );

    }
}
SubmitButton.propTypes = {
    classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(SubmitButton)

