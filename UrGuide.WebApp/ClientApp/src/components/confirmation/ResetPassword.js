import React, { useState, useReducer, useContext } from 'react';
import {
    Container,
    Grid,
    Button,
    FormHelperText,
    IconButton
} from '@material-ui/core';
import clsx from "clsx";
import { makeStyles } from '@material-ui/core/styles';
import Input from '@material-ui/core/Input';
import InputLabel from '@material-ui/core/InputLabel';
import InputAdornment from '@material-ui/core/InputAdornment';
import FormControl from '@material-ui/core/FormControl';
import TextField from '@material-ui/core/TextField';
import AccountCircle from '@material-ui/icons/AccountCircle';
import { HttpClientFactory } from '../../httpclient';
import { Visibility, VisibilityOff } from "@material-ui/icons";
import { AccountClient, ResetPasswordModel } from '../../api';
import ResetPasswordContext from './ResetPasswordContext';
import ResetPasswordReducer from './ResetPasswordReducer';
import { Redirect } from 'react-router-dom';

const useStyles = makeStyles((theme) => ({
    margin: {
        margin: theme.spacing(1),
        width: "20%",
        marginLeft: "40%"
    },
}));





const ResetPassword = () => {

    const [values, setValues] = React.useState({
        passwordNew: "",
        passwordConfirm: "",
        showNewPassword: false,
        showConfirmPassword: false,
        done: false
    });

    const gotoResetPassword = () => {
        // It's important that we do a replace here so that we remove the callback uri with the
        // fragment containing the tokens from the browser history.
        //window.location.replace(`${window.location.origin}/sign-in`);
        const params = new URLSearchParams(window.location.search);
        let email = params.get('Email'),
            confirmationToken = params.get('ConfirmationToken');
        const api = HttpClientFactory.get(AccountClient);
        api.resetpassword(new ResetPasswordModel({
            confirmPassword: values.passwordConfirm,
            password: values.passwordNew,
            email: email,
            confirmationToken: confirmationToken 
        })).then(() => {
            setValues({ ...values, done: true });
        })
    }

    const classes = useStyles();

    const handleChange = prop => event => {
        setValues({ ...values, [prop]: event.target.value });
    };

    const handleClickShowNewPassword = () => {
        setValues({ ...values, showNewPassword: !values.showNewPassword });
    };

    const handleClickShowConfirmPassword = () => {
        setValues({ ...values, showConfirmPassword: !values.showConfirmPassword });
    };

    const handleMouseDownPassword = event => {
        event.preventDefault();
    };

    const ctx = useContext(ResetPasswordContext);
    const [state, dispatch] = useReducer(ResetPasswordReducer, ctx)

    if (values.done) {
        return <Redirect to='/'/>
    }

    return (
        <Grid container xs={12}>
            <p className='col-12 text-center text-success'>
                Enter new password
            </p>
            <Grid item xs={12}>
                <FormControl
                    fullWidth
                    className={clsx(classes.margin, classes.textField)}
                    variant="outlined"
                >
                    <InputLabel htmlFor="adornment-password">
                        Password
                </InputLabel>
                    <Input
                        type={values.showNewPassword ? "text" : "password"}
                        value={values.passwordNew}
                        onChange={handleChange("passwordNew")}
                        endAdornment={
                            <InputAdornment position="end">
                                <IconButton
                                    aria-label="toggle password visibility"
                                    onClick={handleClickShowNewPassword}
                                    onMouseDown={handleMouseDownPassword}
                                    edge="end"
                                >
                                    {values.showNewPassword ? <Visibility /> : <VisibilityOff />}
                                </IconButton>
                            </InputAdornment>
                        }

                    />
                </FormControl>
                <FormHelperText error>
                    {state.passwordErrorMessage}
                </FormHelperText>
            </Grid>
            <p className='col-12 text-center text-success'>
                Confirm your password
            </p>
            <Grid item xs={12}>
                <FormControl
                    fullWidth
                    className={clsx(classes.margin, classes.textField)}
                    variant="outlined"
                >
                    <InputLabel htmlFor="adornment-password">
                        Password
                </InputLabel>
                    <Input
                        type={values.showConfirmPassword ? "text" : "password"}
                        value={values.passwordConfirm}
                        onChange={handleChange("passwordConfirm")}
                        endAdornment={
                            <InputAdornment position="end">
                                <IconButton
                                    aria-label="toggle password visibility"
                                    onClick={handleClickShowConfirmPassword}
                                    onMouseDown={handleMouseDownPassword}
                                    edge="end"
                                >
                                    {values.showConfirmPassword ? <Visibility /> : <VisibilityOff />}
                                </IconButton>
                            </InputAdornment>
                        }

                    />
                </FormControl>
                <FormHelperText error>
                    {state.passwordErrorMessage}
                </FormHelperText>
            </Grid>
            <p className='col-12 text-center'>
                <Button
                    disabled={values.passwordConfirm != values.passwordNew || !values.passwordNew}
                    variant="contained"
                    color="primary"
                    onClick={() => 
                        dispatch({
                            type: "confirm-password",
                            data: {
                                newPassword: values.passwordNew,
                                confirmPassword: values.passwordConfirm,
                                callback: gotoResetPassword
                            }
                        })}
                >
                    Reset
                </Button>
            </p>
        </Grid>
    );
}

export default ResetPassword;

