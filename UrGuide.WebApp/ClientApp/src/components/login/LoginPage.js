import React, { useReducer, useContext } from "react";
import {
    Grid,
    Box,
    makeStyles,
    FormHelperText,
    Button,
    Typography,
    IconButton,
    InputAdornment,
    FormControl,
    FormControlLabel,
    Link,
    Checkbox,
    Container,
    Input,
    Paper,
    CssBaseline,
    InputLabel
} from "@material-ui/core";
import clsx from "clsx";
import { Visibility, VisibilityOff, AccountCircle } from "@material-ui/icons";
import LoginContext from "./LoginContext";
import LoginReducer from "./LoginReducer";
import "./LoginPage.css";
import Login from './../api-authorization/Login'

import {  QueryParameterNames } from './../api-authorization/ApiAuthorizationConstants';

function Copyright() {
    return (
        <Typography variant="body2" color="textSecondary" align="center">
            {"Copyright © "}
            <Link color="inherit" href="https://material-ui.com/">
                UrGuide
      </Link>{" "}
            {new Date().getFullYear()}
            {"."}
        </Typography>
    );
}

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
        backgroundColor: "red"
    },
    form: {
        width: "100%", // Fix IE 11 issue.
        marginTop: theme.spacing(1)
    }
}));

const LoginForm = () => {
    const classes = useStyles();
    const [values, setValues] = React.useState({
        amount: "",
        password: "",
        email: "",
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

    const ctx = useContext(LoginContext);
    const [state, dispatch] = useReducer(LoginReducer, ctx);

    const getReturnUrl = (state) => {
        const params = new URLSearchParams(window.location.search);
        const fromQuery = params.get(QueryParameterNames.ReturnUrl);
        if (fromQuery && !fromQuery.startsWith(`${window.location.origin}/`)) {
            // This is an extra check to prevent open redirects.
            throw new Error("Invalid return url. The return url needs to have the same origin as the current page.")
        }
        return (state && state.returnUrl) || fromQuery || `${window.location.origin}/`;
    }
    const navigateToReturnUrl = (returnUrl) => {
        // It's important that we do a replace here so that we remove the callback uri with the
        // fragment containing the tokens from the browser history.
        window.location.replace(returnUrl);
    }
    const login = async function (state) {
        const returnUrl = getReturnUrl();
        const response = await fetch('/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                userName: state.email,
                password: state.password,
                persist: state.isRemembered,
                returnUrl
            })
        });
        if (response.status == 200 || response.status == 304) {
            navigateToReturnUrl(returnUrl);
        }
    }

    const emailTextField = state.emailError ? (
        <Grid item xs={12}>
            <FormControl
                fullWidth
                className={clsx(classes.margin, classes.textField)}
                variant="outlined"
            >
                <InputLabel error htmlFor="adornment-text">
                    Email address
        </InputLabel>
                <Input
                    id="EmailInput"
                    type="text"
                    value={values.email}
                    onChange={handleChange("email")}
                    error
                    endAdornment={
                        <InputAdornment position="end">
                            <AccountCircle />
                        </InputAdornment>
                    }
                />
            </FormControl>
            <FormHelperText error>please enter a valid email address.</FormHelperText>
        </Grid>
    ) : (
            <Grid item xs={12}>
                <FormControl
                    fullWidth
                    className={clsx(classes.margin, classes.textField)}
                    variant="outlined"
                >
                    <InputLabel htmlFor="adornment-text">Email address</InputLabel>
                    <Input
                        id="EmailInput"
                        type="text"
                        value={values.email}
                        onChange={handleChange("email")}
                        endAdornment={
                            <InputAdornment position="end">
                                <AccountCircle />
                            </InputAdornment>
                        }

                    />
                </FormControl>
            </Grid>
        );

    const passwordTextField = state.passwordError ? (
        <Grid item xs={12}>
            <FormControl
                fullWidth
                className={clsx(classes.margin, classes.textField)}
                variant="outlined"
            >
                <InputLabel error htmlFor="adornment-password">
                    Password
        </InputLabel>
                <Input
                    error
                    id="PasswordInput"
                    type={values.showPassword ? "text" : "password"}
                    value={values.password}
                    onChange={handleChange("password")}
                    endAdornment={
                        <InputAdornment position="end">
                            <IconButton
                                aria-label="toggle password visibility"
                                onClick={handleClickShowPassword}
                                onMouseDown={handleMouseDownPassword}
                                edge="end"
                            >
                                {values.showPassword ? <Visibility /> : <VisibilityOff />}
                            </IconButton>
                        </InputAdornment>
                    }

                />
            </FormControl>
            <FormHelperText error>
                your password must contains at least 8 alpha-numeric characters.
      </FormHelperText>
        </Grid>
    ) : (
            <Grid item xs={12}>
                <FormControl
                    fullWidth
                    className={clsx(classes.margin, classes.textField)}
                    variant="outlined"
                >
                    <InputLabel htmlFor="adornment-password">Password</InputLabel>
                    <Input
                        id="PasswordInput"
                        type={values.showPassword ? "text" : "password"}
                        value={values.password}
                        onChange={handleChange("password")}
                        endAdornment={
                            <InputAdornment position="end">
                                <IconButton
                                    aria-label="toggle password visibility"
                                    onClick={handleClickShowPassword}
                                    onMouseDown={handleMouseDownPassword}
                                    edge="end"
                                >
                                    {values.showPassword ? <Visibility /> : <VisibilityOff />}
                                </IconButton>
                            </InputAdornment>
                        }

                    />
                </FormControl>
            </Grid>
        );

    return (
        <>
            {emailTextField}
            {passwordTextField}
            <Grid item xs={12}>
                <FormControlLabel
                    control={
                        <Checkbox value="remember" id="remember-me" color="primary" />
                    }
                    label="Remember me"
                />
            </Grid>
            <Grid item xs={12}>
                <div className="submit-button-div">
                    <Button
                        fullWidth
                        variant="contained"
                        color="primary"
                        onClick={() =>
                            dispatch({
                                type: "validate-login",
                                data: {
                                    email: values.email,
                                    password: values.password,
                                    isRemembered: document.getElementById("remember-me").checked,
                                    callback: login
                                }
                            })
                        }
                    >
                        Sign In
          </Button>
                </div>
            </Grid>
        </>
    );
};
export function LoginPage() {
    return (
        <div className="wrapper">
            <Grid container component="main" className={useStyles.root}>
                <CssBaseline />
                <Grid item xs={false} sm={4} md={7} className="back-img"></Grid>
                <Grid
                    item
                    xs={12}
                    sm={8}
                    md={5}
                    component={Paper}
                    className="login-side"
                    elevation={0}
                    square
                >
                    <div className="login-paper">
                        <div className="login-avatar-wrapper">
                            <div className="login-avatar"></div>
                        </div>
                        <Typography component="h1" variant="h5" className="text-center">
                            Sign in
            </Typography>
                        <form className="login-form" noValidate>
                            <Container component="main" maxWidth="xs">
                                <CssBaseline />
                                <Grid container spacing={2}>
                                    <LoginForm />
                                    <Grid item xs={12} className="bottom-form" container>
                                        <Grid item xs>
                                            <Link href="#" variant="body2">
                                                Forgot password?
                      </Link>
                                        </Grid>
                                        <Grid item>
                                            <Link href="/sign-up" variant="body2">
                                                {"Don't have an account? Sign Up"}
                                            </Link>
                                        </Grid>
                                    </Grid>
                                </Grid>
                            </Container>
                            <Box mt={5}>
                                <Copyright />
                            </Box>
                        </form>
                    </div>
                </Grid>
            </Grid>
        </div>
    );
}
