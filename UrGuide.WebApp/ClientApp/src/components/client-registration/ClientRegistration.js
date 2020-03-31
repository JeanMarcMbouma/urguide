import React, { Component, useReducer, useContext } from "react";
import {
  Grid,
  Box,
  makeStyles,
  FormHelperText,
  Button,
  Typography,
    IconButton,
    Input,
    InputLabel,
    InputAdornment,
    FormControl,
    FormControlLabel,
  Link,
  Checkbox,
  Container,
  CssBaseline
} from "@material-ui/core";
import { Visibility, VisibilityOff, AccountCircle } from "@material-ui/icons";
import clsx from "clsx";

import ClientContext from "./ClientContext";
import ClientReducer from "./ClientReducer";
import "./ClientRegistration.css";


function Navigation() {
  return (
    <Container>
      <div className="register-avatar-wrapper">
        <div className="register-avatar"></div>
      </div>
      <div className="typo">
        <Typography component="h1" variant="h5" className="text-center">
          Sign up as
        </Typography>
      </div>
      <div className="navigation">
        <Grid container spacing={2}>
          <Grid item xs={7} sm={6} className="client-btn text-center">
            <Link
              color="inherit"
              className="navigator-link"
              href="sign-up"
              style={{ textDecoration: "none", color: "black" }}
            >
              A TOURIST
            </Link>
          </Grid>
          <Grid item xs={5} sm={6} className="guide-btn text-center">
            <Link
              color="inherit"
              className="navigator-link"
              href="guide/sign-up"
              style={{ textDecoration: "none", color: "whitesmoke" }}
            >
              A GUIDE
            </Link>
          </Grid>
        </Grid>
      </div>
    </Container>
  );
}

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


const getReturnUrl = (state) => {
    const params = new URLSearchParams(window.location.search);
    const fromQuery = params.get('ReturnUrl');
    if (fromQuery && !fromQuery.startsWith(`${window.location.origin}/`)) {
        var url = `${window.location.origin}${fromQuery}`;
        return url;
    }
    return (state && state.returnUrl) || fromQuery || `${window.location.origin}/`;
}
const navigateToReturnUrl = (returnUrl) => {
    // It's important that we do a replace here so that we remove the callback uri with the
    // fragment containing the tokens from the browser history.
    window.location.replace(returnUrl);
}

const createUser = async function (state) {
    const returnUrl = getReturnUrl();
    const response = await fetch(`/register?returnUrl=${returnUrl}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        credentials: 'include',
        body: JSON.stringify({
            userName: state.email,
            password: state.password,
            confirmPassword: state.confirmPassword,
            firstName: state.firstName,
            lastName: state.lastName,

        
        })
    });

    if (response.status == 200 || response.status == 304) {
        navigateToReturnUrl(`${window.location.origin}/sign-up-confirm`);
    } else {
        // we got an error
        if (response.status == 400) // BadRequest
        {
            var errors = await response.json();
            console.log(errors);
        } else {
            // Account has certainly been locked-out
        }
    }
}


const ClientRegister = () => {

    const ctx = useContext(ClientContext);
    const [state, dispatch] = useReducer(ClientReducer, ctx);

    const classes = useStyles();

    const [values, setValues] = React.useState({
        firstName: '',
        lastName: '',
        email: '',
        password: '',
        confirmPassword: '',
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


    const fnameGrid =
        state.fnameError ? (
            <Grid item xs={12} sm={6} className="client-field">
                <Box mb={2}>
                <FormControl fullWidth variant="outlined" className={clsx(classes.margin, classes.textField)} >
                    <InputLabel error htmlFor="input-with-icon-adornment">
                        First Name
          </InputLabel>
                    <Input error id="firstName" value={values.firstName}
                        onChange={handleChange("firstName")} />
                </FormControl>
                    <FormHelperText error>This field is required.</FormHelperText>
                    </Box>
            </Grid>
        ) : (
                <Grid item xs={12} sm={6}  >
                    <Box mb={2}>
                    <FormControl fullWidth variant="outlined">
                        <InputLabel htmlFor="input-with-icon-adornment">
                            First Name
          </InputLabel>
                        <Input id="firstName" value={values.firstName}
                            onChange={handleChange("firstName")} />
                        </FormControl>
                        </Box>
                </Grid>
            );

    const lnameGrid =
        state.lnameError ? (
            <Grid item xs={12} sm={6} >
                <Box mb={2}>
                <FormControl fullWidth variant="outlined" className={clsx(classes.margin, classes.textField)} >
                    <InputLabel error htmlFor="input-with-icon-adornment">
                        Last Name
          </InputLabel>
                    <Input error id="lastName" value={values.lastName}
                        onChange={handleChange("lastName")} />
                </FormControl>
                    <FormHelperText error>This field is required.</FormHelperText>
                    </Box>
            </Grid>
        ) : (
                <Grid item xs={12} sm={6} >
                    <Box mb={2}>
                    <FormControl fullWidth variant="outlined">
                        <InputLabel htmlFor="input-with-icon-adornment">Last Name</InputLabel>
                        <Input id="lastName" value={values.lastName}
                            onChange={handleChange("lastName")} />
                        </FormControl>
                        </Box>
                </Grid>
            );

    const emailGrid =
        state.emailError ? (
            <Grid item xs={12}>
                <Box mb={2}>
                <FormControl
                    fullWidth
                    className="client-form"
                    variant="outlined"
                    className={clsx(classes.margin, classes.textField)}
                >
                    <InputLabel error htmlFor="input-with-icon-adornment">
                        Your email
          </InputLabel>
                    <Input
                        error
                        id="client-email"
                        value={values.email}
                        onChange={handleChange("email")}
                        endAdornment={
                            <InputAdornment position="start">
                                <AccountCircle />
                            </InputAdornment>
                        }
                    />
                </FormControl>
                <FormHelperText error>
                    please enter a valid email address.
        </FormHelperText>
                    </Box>
            </Grid>
        ) : (
                <Grid item xs={12}>
                    <Box mb={2}>
                        <FormControl
                            fullWidth
                            className="client-form"
                            variant="outlined"
                           
                        >
                            <InputLabel htmlFor="input-with-icon-adornment">
                                Your email
          </InputLabel>
                            <Input
                                id="client-email"
                                value={values.email}
                                onChange={handleChange("email")}
                                endAdornment={
                                    <InputAdornment position="start">
                                        <AccountCircle />
                                    </InputAdornment>
                                }
                            />
                        </FormControl>
                    </Box>
                   
                </Grid>
            );

    const passwordGrid =
        state.passwordError ? (
            <Grid item xs={12}>
                <Box mb={2}>
                <FormControl
                    emailTextField
                    fullWidth
                    className="client-form"
                    variant="outlined"
                >
                    <InputLabel error htmlFor="standard-adornment-password">
                        Password
          </InputLabel>
                    <Input
                        id="client-password"
                        error
                        type={values.showPassword ? "text" : "password"}
                        value={values.password}
                        onChange={handleChange("password")}
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
                <FormHelperText error>
                    your password must contains minimum eight characters, at least one uppercase letter, one lowercase letter, one number and one special character.
        </FormHelperText>
                    </Box>
            </Grid>
        ) : (
                <Grid item xs={12}>
                    <Box mb={2}>
                    <FormControl
                        emailTextField
                        fullWidth
                        className="client-form"
                        variant="outlined"
                    >
                        <InputLabel htmlFor="standard-adornment-password">
                            Password
          </InputLabel>
                        <Input
                            id="client-password"
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
                        </Box>
                </Grid>
            );

    const passwordConfirmationGrid =
         state.passwordsDontMatch  ? (
            <Grid item xs={12}>
                <Box mb={2}>
                <FormControl
                    fullWidth
                    className={clsx(classes.margin, classes.textField)}
                    variant="outlined"
                >
                    <InputLabel error htmlFor="standard-adornment-password">
                        Password Confirmation
          </InputLabel>
                    <Input
                        id="confirm-password"
                        error
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
                <FormHelperText error>
                    The password and its confirmation do not match.
        </FormHelperText>
                    </Box>
            </Grid>
        ) : (
                <Grid item xs={12}>
                    <Box mb={2}>
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
                        </Box>
                </Grid>
            );

    const consent =
         !state.isChecked ? (
            <Grid item xs={12}>
                <Box mt={2}>
                    <FormControlLabel
                        control={
                            <Checkbox
                                value="allowExtraEmails"
                                id="client-checkbox"
                                color="primary"
                            />
                        }
                        label="I agree with the Terms and Conditons."
                    />
                </Box>
                <FormHelperText error>
                    please check to agree with the Terms and Conditons for signing up.
        </FormHelperText>
            </Grid>
        ) : (
                <Grid item xs={12}>
                    <Box mt={5}>
                        <FormControlLabel
                            control={
                                <Checkbox
                                    value="allowExtraEmails"
                                    id="client-checkbox"
                                    color="primary"
                                />
                            }
                            label="I agree with the Terms and Conditons."
                        />
                    </Box>
                </Grid>
            );

    const submitButtonGrid = (<Grid item xs={12}>
        <div className="submit-button-div">
            <Button

                fullWidth
                variant="contained"
                color="primary"
                onClick={() =>
                    dispatch({
                        type: "validate",
                        data: {
                            email: document.getElementById("client-email").value,
                            password: document.getElementById("client-password").value,
                            confirmPassword: document.getElementById("confirm-password")
                                .value,
                            firstName: document.getElementById("firstName").value,
                            lastName: document.getElementById("lastName").value,
                            isChecked: document.getElementById("client-checkbox").checked,
                            sendData: createUser
                        }
                    })
                }
            >
               Sign In
      </Button>
        </div>
    </Grid>);

     

    return (<>
        {fnameGrid}
        {lnameGrid}
        {emailGrid}
        {passwordGrid}
        {passwordConfirmationGrid}
        {consent}
        {submitButtonGrid}
    </>);

  
};

export class ClientRegistration extends Component {
    static displayName = ClientRegistration.name;


    render() {

    return (
      <div>
        <Box mb={5}>
          <Navigation />
        </Box>
        <Container component="main" maxWidth="xs">
          <CssBaseline />
          <div className={useStyles.paper}>
            <form className={useStyles.form} noValidate>
              <Grid container justify="flex-end">
                <ClientRegister />
                <Box mt={3} item>
                  <Link href="#" variant="body2">
                    Already have an account? Sign in
                  </Link>
                </Box>
              </Grid>
            </form>
            <Box mt={5} mb={5}>
              <Copyright />
            </Box>
          </div>
        </Container>
      </div>
    );
  }
}
