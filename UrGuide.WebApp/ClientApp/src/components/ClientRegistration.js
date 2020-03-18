import React, { Component } from "react";
import Avatar from "@material-ui/core/Avatar";
import Button from "@material-ui/core/Button";
import CssBaseline from "@material-ui/core/CssBaseline";
import TextField from "@material-ui/core/TextField";
import FormControlLabel from "@material-ui/core/FormControlLabel";
import Checkbox from "@material-ui/core/Checkbox";
import Link from "@material-ui/core/Link";
import Grid from "@material-ui/core/Grid";
import Box from "@material-ui/core/Box";
// import LockOutlinedIcon from '@material-ui/icons/LockOutlined';
import Typography from "@material-ui/core/Typography";
import { makeStyles } from "@material-ui/core/styles";
import Container from "@material-ui/core/Container";
import "./RegisterLayout.css";

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
            <Grid container spacing={2}>
              <Grid item xs={12} sm={6}>
                <TextField
                  autoComplete="fname"
                  name="firstName"
                  variant="outlined"
                  required
                  fullWidth
                  id="firstName"
                  label="First Name"
                  autoFocus
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <TextField
                  variant="outlined"
                  required
                  fullWidth
                  id="lastName"
                  label="Last Name"
                  name="lastName"
                  autoComplete="lname"
                />
              </Grid>
              <Grid item xs={12}>
                <TextField
                  variant="outlined"
                  required
                  fullWidth
                  id="email"
                  label="Email Address"
                  name="email"
                  autoComplete="email"
                />
              </Grid>
              <Grid item xs={12}>
                <TextField
                  variant="outlined"
                  required
                  fullWidth
                  name="password"
                  label="Password"
                  type="password"
                  id="password"
                  autoComplete="current-password"
                />
              </Grid>
              <Grid item xs={12}>
                <FormControlLabel
                  control={
                    <Checkbox value="allowExtraEmails" color="primary" />
                  }
                  label="I agree with the Terms and Conditons."
                />
              </Grid>
            </Grid>
            <Box mt={4}>
              <Button
                type="submit"
                fullWidth
                variant="contained"
                color="primary"
                className={userStyles.submit}
              >
                Sign Up
              </Button>
            </Box>
            <Grid container justify="flex-end">
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
