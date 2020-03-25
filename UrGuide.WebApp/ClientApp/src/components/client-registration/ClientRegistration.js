import React, { Component, useReducer, useContext } from "react";
import {
  Grid,
  Box,
  makeStyles,
  FormHelperText,
  Button,
  Typography,
  TextField,
  FormControlLabel,
  Link,
  Checkbox,
  Container,
  CssBaseline
} from "@material-ui/core";

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

const userStyles = makeStyles(theme => ({
  paper: {
    marginTop: theme.spacing(8),
    display: "flex",
    flexDirection: "column",
    alignItems: "center"
  },
  avatar: {
    margin: theme.spacing(1),
    backgroundColor: theme.palette.secondary.main
  },
  form: {
    width: "100%", // Fix IE 11 issue.
    marginTop: theme.spacing(3)
  },
  submit: {
    margin: theme.spacing(3, 0, 2)
  }
}));

const ClientRegister = () => {
  const ctx = useContext(ClientContext);
  const [state, dispatch] = useReducer(ClientReducer, ctx);
  const firstNameTextField = state.fnameError ? (
    <TextField
      autoComplete="fname"
      name="firstName"
      variant="outlined"
      required
      fullWidth
      id="client-fname"
      label="First Name"
      autoFocus
      error
      helperText="This field is required."
    />
  ) : (
    <TextField
      autoComplete="fname"
      name="firstName"
      variant="outlined"
      required
      fullWidth
      id="client-fname"
      label="First Name"
      autoFocus
    />
  );

  const lastNameTextField = state.lnameError ? (
    <TextField
      autoComplete="lname"
      name="lastName"
      variant="outlined"
      required
      fullWidth
      id="client-lname"
      label="Last Name"
      autoFocus
      error
      helperText="This field is required."
    />
  ) : (
    <TextField
      autoComplete="lname"
      name="lastName"
      variant="outlined"
      required
      fullWidth
      id="client-lname"
      label="Last Name"
      autoFocus
    />
  );

  const emailTextField = state.emailError ? (
    <TextField
      variant="outlined"
      required
      fullWidth
      id="client-email"
      label="Email Address"
      name="email"
      autoComplete="email"
      error
      helperText="please enter a valid email address."
    />
  ) : (
    <TextField
      variant="outlined"
      required
      fullWidth
      id="client-email"
      label="Email Address"
      name="email"
      autoComplete="email"
    />
  );

  const passwordTextField = state.passwordError ? (
    <TextField
      variant="outlined"
      required
      fullWidth
      name="password"
      label="Password"
      type="password"
      id="client-password"
      autoComplete="current-password"
      error
      helperText="your password must contains at least 8 alpha-numeric characters."
    />
  ) : (
    <TextField
      variant="outlined"
      required
      fullWidth
      name="password"
      label="Password"
      type="password"
      id="client-password"
      autoComplete="current-password"
      helperText=""
    />
  );

  const CheckBoxErrorText = state.isChecked ? (
    <></>
  ) : (
    <FormHelperText error>
      please check to agree with our Terms and Conditions.
    </FormHelperText>
  );

  return (
    <div>
      <Grid container spacing={2}>
        <Grid item xs={12} sm={6}>
          {firstNameTextField}
        </Grid>
        <Grid item xs={12} sm={6}>
          {lastNameTextField}
        </Grid>
        <Grid item xs={12}>
          {emailTextField}
        </Grid>
        <Grid item xs={12}>
          {passwordTextField}
        </Grid>
        <Grid item xs={12}>
          <FormControlLabel
            control={
              <Checkbox
                value="allowExtraEmails"
                color="primary"
                id="client-checkbox"
              />
            }
            label="I agree with UrGuide's Terms and Conditons."
          />
          {CheckBoxErrorText}
        </Grid>
      </Grid>
      <Box mt={4}>
        <Button
          type="button"
          fullWidth
          variant="contained"
          color="primary"
          className={userStyles.submit}
          onClick={() =>
            dispatch({
              type: "validate",
              data: {
                firstName: document.getElementById("client-fname").value,
                lastName: document.getElementById("client-lname").value,
                email: document.getElementById("client-email").value,
                password: document.getElementById("client-password").value,
                isChecked: document.getElementById("client-checkbox").checked
              }
            })
          }
        >
          Sign Up
        </Button>
      </Box>
    </div>
  );
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
          <div className={userStyles.paper}>
            <form className={userStyles.form} noValidate>
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
