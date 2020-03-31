import React from 'react';
import {
    Container,
    Grid,
    Button
} from '@material-ui/core';

const EmailConfirmation = () => {
    const gotoSignIn = () => {
        // It's important that we do a replace here so that we remove the callback uri with the
        // fragment containing the tokens from the browser history.
        window.location.replace(`${window.location.origin}/sign-in`);
    }
    return (
        <Grid container xs={12}>
            <p className='col-12 text-center text-success'>
                Congratulation, you've successfully activated your account!
            </p>
            <p className='col-12 text-center'>
                <Button onClick={gotoSignIn}
                    variant="contained"
                    color="primary"
                >
                    {"Go to Sign In"}
                </Button>
            </p>
        </Grid>
    );
}

export default EmailConfirmation;