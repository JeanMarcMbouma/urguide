import React from 'react';
import {
    Grid,
    Button
} from '@material-ui/core';
import { useAuthContext } from './AuthService';

const LogoutCallback = () => {
    const { manager } = useAuthContext();
    const gotoSignIn = () => {
        // It's important that we do a replace here so that we remove the callback uri with the
        // fragment containing the tokens from the browser history.
        
        manager.signIn(window.location.href);
    }
    return (
        <Grid container>
            <p className='col-12 text-center text-success'>
                You're logged out!
            </p>
            <p className='col-12 text-center'>
                <Button onClick={gotoSignIn}
                    variant="contained"
                    color="primary"
                >
                    {"Sign In"}
                </Button>
            </p>
        </Grid>
    );
}

export default LogoutCallback;