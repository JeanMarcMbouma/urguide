import React from 'react';
import {
    Container,
    Grid,
    Button
} from '@material-ui/core';

const RegistrationConfirmation = () => {

    return (
        <Grid container xs={12}>
            <p className='col-12 text-center text-success'>
                Congratulation, you've successfully registered to UrGuide!
            </p>
            <p className='col-12 text-center text-primary'>
                A confirmation email was sent to your email account.
            </p>
        </Grid>
    );
}

export default RegistrationConfirmation;