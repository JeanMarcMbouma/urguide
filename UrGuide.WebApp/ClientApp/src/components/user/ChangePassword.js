import React, { Component, useContext, useReducer, useState } from "react";
import {
        Grid,
    Box,
    makeStyles,
    FormHelperText,
    IconButton,
    Input,
    InputLabel,
    InputAdornment,
    FormControl,
    Container,
    CssBaseline,
    Button,

} from "@material-ui/core";
import ChangePasswordContext from "./profile/ChangePasswordContext";
import ChangePasswordReducer from "./profile/ChangePasswordReducer";
import EditProfileNavigation from "./EditProfileNavigation";
import { Visibility, VisibilityOff, AccountCircle } from "@material-ui/icons";
import { useAuthUser } from '../api-authorization/AuthService';
import Alert from '@material-ui/lab/Alert';
import { ChangePasswordModel } from './../../api';
import { HttpClientFactory } from './../../httpclient';
import clsx from "clsx";
import "./UserStyle.css";

const useStyles = makeStyles(theme => ({
    root: {
        minHeight: "100vh"
    },
    paper: {
        display: "flex",
        flexDirection: "column",
        alignItems: "center"
    },
    avatar: {
        margin: theme.spacing(1),
      
    },
    form: {
        width: "100%", // Fix IE 11 issue.
        marginTop: theme.spacing(1)
    }
}));


function ChangePasswordForm() {
    const classes = useStyles();

    const user = useAuthUser();
    const { profile } = user || {
        profile: {}
    };

    const ctx = useContext(ChangePasswordContext);
    const [state, dispatch] = useReducer(ChangePasswordReducer, ctx);

    state.user = user;

    const [status, setStatus] = useState(0);

    const [values, setValues] = useState({
        email: profile.name,
        password: '',
        confirmPassword: '',
        currentPassword:'',
        showPassword: false
    });

    const handleChange = prop => event => {
        setValues({ ...values, [prop]: event.target.value });
    };

    const handleClickShowPassword = () => {
        setValues({ ...values, showPassword: !values.showPassword });
    };

    const handleMouseDownPassword = event => {
        event.preventDefault();
    };

    async function changePassword(state) {

        const client = HttpClientFactory.getAccountClient(state.user);

        const model = new ChangePasswordModel({
            email: state.email,
            password: state.password,
            confirmPassword: state.confirmPassword,
            currentPassword: state.currentPassword,
        });

        try {

            await client.changepassword(model);
            setStatus(200);
            return 200;
        }
        catch (e) {

            setStatus(400);
            return 400;

        }

    }


    return (
        <div className='edit-profile-card' component="main">
            <form 
            >
            <h5 className='text-muted'>Change your login details</h5>
            <br />
                <br />
                {status == 200 ? <Alert severity="success">Your login details have been successfully changed!</Alert> : null}
                {status == 400 ?  <Alert severity="error">Oops ! wrong login details provided !</Alert> : null }
                <br />
                <br />
            <Grid container spacing={4}>
                    <Grid item xs={12} sm={6} >
                        <FormControl
                            fullWidth
                            className={clsx(classes.margin, classes.textField)}
                            variant="outlined"
                        >
                            <InputLabel htmlFor="input-with-icon-adornment">
                                Your email
          </InputLabel>
                            <Input
                                id="guide-email"
                                value={values.email}
                                onChange={handleChange("email")}
                                endAdornment={
                                    <InputAdornment position="start">
                                        <AccountCircle />
                                    </InputAdornment>
                                }
                            />
                        </FormControl>
                        {state.emailError ? <FormHelperText error>
                            {state.emailErrorMessage}
        </FormHelperText> : null }
                    </Grid>
                    <Grid item xs={12} sm={6}>
                        <FormControl
                        
                            fullWidth
                           
                        >
                            <InputLabel htmlFor="standard-adornment-password">
                                Current Password
          </InputLabel>
                            <Input
                                id="current-password"
                                className={clsx(classes.margin, classes.textField)}
                                value={values.currentPassword}
                                onChange={handleChange("currentPassword")}
                                type={values.showPassword ? "text" : "password"}
                                endAdornment={
                                    < InputAdornment position="end" >
                                        <IconButton
                                            aria-label="toggle password visibility"
                                            onClick={handleClickShowPassword}
                                            onMouseDown={handleMouseDownPassword}
                                        >
                                            {values.showPassword ? <Visibility /> : <VisibilityOff />}
                                        </IconButton>
                                    </InputAdornment >}

                               
                                
                            />
                        </FormControl>
                        {state.currentPasswordError ? <FormHelperText error>
                            {state.currentPasswordErrorMessage}
                        </FormHelperText> : null}
                    </Grid>
                <Grid item xs={12} sm={6}>
                    <FormControl
                    
                        fullWidth
                            className={clsx(classes.margin, classes.textField)}

                        variant="outlined"
                    >
                        <InputLabel htmlFor="standard-adornment-password">
                            Password
          </InputLabel>
                        <Input
                                id="guide-password"
                                value={values.password}
                                onChange={handleChange("password")}
                                type={values.showPassword ? "text" : "password"}
                                endAdornment={
                                    < InputAdornment position="end" >
                                        <IconButton
                                            aria-label="toggle password visibility"
                                            onClick={handleClickShowPassword}
                                            onMouseDown={handleMouseDownPassword}
                                        >
                                            {values.showPassword ? <Visibility /> : <VisibilityOff />}
                                        </IconButton>
                                    </InputAdornment >}
                           
                           
                        />
                        </FormControl>
                        {state.passwordError ? <FormHelperText error>
                            {state.passwordErrorMessage}
                        </FormHelperText> : null}
                </Grid>
                <Grid item xs={12} sm={6} >
                    <FormControl
                        fullWidth
                        className={clsx(classes.margin, classes.textField)}
                        variant="outlined"
                    >
                        <InputLabel htmlFor="standard-adornment-password">
                            Password Confirmation
          </InputLabel>
                        <Input
                                id="confirm-password"
                                value={values.confirmPassword}
                                onChange={handleChange("confirmPassword")}
                                type={values.showPassword ? "text" : "password"}
                                endAdornment={
                                    < InputAdornment position="end" >
                                        <IconButton
                                            aria-label="toggle password visibility"
                                            onClick={handleClickShowPassword}
                                            onMouseDown={handleMouseDownPassword}
                                        >
                                            {values.showPassword ? <Visibility /> : <VisibilityOff />}
                                        </IconButton>
                                    </InputAdornment >}
                  
                        />
                        </FormControl>
                        {state.passwordsDontMatchError ? <FormHelperText error>
                            {state.passwordsDontMatchErrorMessage}
                        </FormHelperText> : null}
                    </Grid>
                </Grid>
            <br />        
            <br />        
            <Button
                    variant="contained"
                    color="primary"
                    type="button"
                    onClick={() =>
                        dispatch({
                            type: "changePassword",
                            data: {
                                email:values.email,
                                password: values.password,
                                confirmPassword: values.confirmPassword,
                                currentPassword: values.currentPassword,
                                user: state.user,
                                callback: changePassword,
                            }
                        })}
            >
                Save Changes
              </Button>
            </form>
        </div>
    );
}





function Layout() {

    return (
        <div className='row justify-content-center'>
            <div className="col-12 col-lg-4 about">
                <EditProfileNavigation />
            </div>
            <div className="col-12 col-lg-7">
                <ChangePasswordForm />
            </div>
        </div>
 );
}


export default class ChangePassword extends Component {
    render() {
        return (
          
                <div className="row">
                    <div className="col-12 lower-section">
                        <Layout />
                    </div>
                </div>
        )
    }
}

