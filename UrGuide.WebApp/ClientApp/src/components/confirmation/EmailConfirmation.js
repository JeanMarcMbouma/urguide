import React from 'react';
import {
    Container,
    Grid,
    Button
} from '@material-ui/core';

const EmailConfirmation = () => {

    return (
        <Grid container xs={12}>
            <p className='col-12 text-center text-success'>
                Congratulation, you've successfully activated your account!
            </p>
            <p className='col-12 text-center'>
                <Button onClick={() => window.location.href = '/sign-in'}
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