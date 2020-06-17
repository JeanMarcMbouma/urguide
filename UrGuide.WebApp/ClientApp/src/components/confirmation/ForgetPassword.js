import React, { useState } from 'react';
import {
    Container,
    Grid,
    Button
} from '@material-ui/core';
import { makeStyles } from '@material-ui/core/styles';
import Input from '@material-ui/core/Input';
import InputLabel from '@material-ui/core/InputLabel';
import InputAdornment from '@material-ui/core/InputAdornment';
import FormControl from '@material-ui/core/FormControl';
import TextField from '@material-ui/core/TextField';
import AccountCircle from '@material-ui/icons/AccountCircle';
import { HttpClientFactory } from '../../httpclient';
import { AccountClient } from '../../api';
import { Link } from 'react-router-dom';

const useStyles = makeStyles((theme) => ({
    margin: {
        margin: theme.spacing(1),
        width: "20%",
        marginLeft:"40%"
    },
    home: {
        color: "white"
    }
}));

const ForgetPassword = () => {
    const [email, setEmail] = useState();
    const [done, setDone] = useState(false);
    const gotoResetPassword = (email) => {
        // It's important that we do a replace here so that we remove the callback uri with the
        // fragment containing the tokens from the browser history.
        //window.location.replace(`${window.location.origin}/sign-in`);

        const api = HttpClientFactory.get(AccountClient);
        api.forgetpassword(email).then(() => {
            setDone(true);
        })
        //email.message = true;
        //api.resetpassword(new ResetPasswordModel)
    }
    const classes = useStyles();

    if (done) {
        return <Grid container xs={12}>
            <p className='col-12 text-center text-success'>
                Please, check your email!
            </p>
            <p className='col-12 text-center'>
                <Button
                    variant="contained"
                    color="primary"
                >
                    <Link to='/' className="btn-primery">Go to Home</Link>
                </Button>
                
            </p>
        </Grid>
    }
    //{email.message ?
    return (
        <Grid container xs={12}>
            <p className='col-12 text-center text-success'>
                Enter your email adress
            </p>
            <TextField className='col-12 align-center'
                className={classes.margin}
                id="EmailInput"
                type="text"
                fullWidth
                label="Email adress"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
            />
            <p className='col-12 text-center'>
                <Button
                    disabled={!email}
                    variant="contained"
                    color="primary"
                    onClick={() => gotoResetPassword(email)}
                >
                    Reset
                </Button>
            </p>
        </Grid>
    );
        //) : (
        //        <Grid>
        //            <h3>Check your email adress</h3>
        //        </Grid>
        //        )
    
}

export default ForgetPassword;