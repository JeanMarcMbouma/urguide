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
import ChangePasswordContext from "./changepassword/ChangePasswordContext";
import ChangePasswordReducer from "./changepassword/ChangePasswordReducer";
import EditProfileNavigation from "./EditProfileNavigation";
import { Visibility, VisibilityOff, AccountCircle } from "@material-ui/icons";
import { useAuthContext, useAuth, useAuthUser } from '../api-authorization/AuthService';
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

    state.email = profile['name'];
    state.user = user;

    const [values, setValues] = useState({
        email: state.email,
        password: '',
        confirmPassword: '',
        currentPassword: '',
        weight: "",
        weightRange: "",
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


    return (
        <div className='edit-profile-card' component="main">
            <form 
            >
            <h5 className='text-muted'>Change your login details</h5>
            <br />
            <br />
            <br />
            <Grid container spacing={4}>
                    <Grid item xs={12} sm={6}>
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
                    </Grid>
                    <Grid item xs={12} sm={6}>
                        <FormControl
                            emailTextField
                            fullWidth
                            className={clsx(classes.margin, classes.textField)}
                            variant="outlined"
                        >
                            <InputLabel htmlFor="standard-adornment-password">
                                Current Password
          </InputLabel>
                            <Input
                                id="current-password"
                                type={values.showPassword ? "text" : "password"}
                                onChange={handleChange("currentPassword")}
                                value={values.currentPassword}
                                endAdornment={
                                    <InputAdornment position="end">
                                        <IconButton
                                            aria-label="toggle password visibility"
                                            onClick={handleClickShowPassword}
                                            onMouseDown={handleMouseDownPassword}
                                        >
                                            {values.showPassword ? <Visibility /> : <VisibilityOff />}
                                        </IconButton>
                                    </InputAdornment>
                                }
                            />
                        </FormControl>
                    </Grid>
                <Grid item xs={12} sm={6}>
                    <FormControl
                        emailTextField
                        fullWidth
                        className={clsx(classes.margin, classes.textField)}
                        variant="outlined"
                    >
                        <InputLabel htmlFor="standard-adornment-password">
                            Password
          </InputLabel>
                        <Input
                            id="guide-password"
                            type={values.showPassword ? "text" : "password"}
                            onChange={handleChange("password")}
                            value={values.password}
                            endAdornment={
                                <InputAdornment position="end">
                                    <IconButton
                                        aria-label="toggle password visibility"
                                        onClick={handleClickShowPassword}
                                        onMouseDown={handleMouseDownPassword}
                                    >
                                        {values.showPassword ? <Visibility /> : <VisibilityOff />}
                                    </IconButton>
                                </InputAdornment>
                            }
                        />
                    </FormControl>
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
                            type={values.showPassword ? "text" : "password"}
                            onChange={handleChange("confirmPassword")}
                            value={values.confirmPassword}
                            endAdornment={
                                <InputAdornment position="end">
                                    <IconButton
                                        aria-label="toggle password visibility"
                                        onClick={handleClickShowPassword}
                                        onMouseDown={handleMouseDownPassword}
                                    >
                                        {values.showPassword ? <Visibility /> : <VisibilityOff />}
                                    </IconButton>
                                </InputAdornment>
                            }
                        />
                    </FormControl>
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
                                email: values.email,
                                password: values.password,
                                confirmPassword: values.confirmPassword,
                                currentPassword: values.currentPassword,
                                user: state.user,
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