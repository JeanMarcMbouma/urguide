import React, { Component } from "react";
import Button from "@material-ui/core/Button";
import CssBaseline from "@material-ui/core/CssBaseline";
import TextField from "@material-ui/core/TextField";
import clsx from 'clsx';
import FormControlLabel from "@material-ui/core/FormControlLabel";
import Checkbox from "@material-ui/core/Checkbox";
import Link from "@material-ui/core/Link";
import Paper from "@material-ui/core/Paper";
import Box from "@material-ui/core/Box";
import Grid from "@material-ui/core/Grid";
import FormControl from '@material-ui/core/FormControl';
import IconButton from '@material-ui/core/IconButton';
import Input from '@material-ui/core/Input';
import InputAdornment from '@material-ui/core/InputAdornment';
import InputLabel from '@material-ui/core/InputLabel';
import { Visibility, VisibilityOff, AccountCircle } from '@material-ui/icons';
import Container from "@material-ui/core/Container";
//import LockOutlinedIcon from "@material-ui/core/Icons/LockOutline";
import Typography from "@material-ui/core/Typography";
import { makeStyles } from "@material-ui/core/styles";
import "./LoginPage.css";

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

export function LoginPage  () {
  const classes = useStyles();

  const [values, setValues] = React.useState({
    amount: '',
    password: '',
    email:'',
    weight: '',
    weightRange: '',
    showPassword: false,
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

//   const CheckInputs = () =>
// {
//   let email = document.getElementById("EmailInput")
//   let password = document.getElementById("PasswordInput")
//     let regexEmail = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/
//     let validEmail = regexEmail.test(email);
//     let regexPassword = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A -Za-z\d@$!%*?&]{ 8, 24 }$/
//     let validPassword = regexPassword.test(password);
//    if(!validEmail){
//      alert("incorrect Inputs")
//    }
// }

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
            elevation={5}
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
                <Container component="main" maxWidth="xs" >
                  <CssBaseline />
                  <Grid container spacing={2}>
                    <Grid item xs={12}>
                      <FormControl fullWidth className={clsx(classes.margin, classes.textField)} variant="outlined">
                        <InputLabel htmlFor="adornment-text">Your email</InputLabel>
                        <Input
                          id="EmailInput"
                          type='text'
                          endAdornment={
                            <InputAdornment position="end">
                              <AccountCircle />
                            </InputAdornment>
                          }
                          labelWidth={70}
                        />
                      </FormControl>
                    </Grid>
                    <Grid item xs={12}>
                      <FormControl fullWidth className={clsx(classes.margin, classes.textField)} variant="outlined">
                        <InputLabel htmlFor="adornment-password">Password</InputLabel>
                        <Input
                          id="PasswordInput"
                          type={values.showPassword ? 'text' : 'password'}
                          value={values.password}
                          onChange={handleChange('password')}
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
                          labelWidth={70}
                        />
                      </FormControl>
                    </Grid>
                    <Grid item xs={12}>
                      <FormControlLabel
                        control={<Checkbox value="remember" color="primary" />}
                        label="Remember me"
                      />
                    </Grid>
                  </Grid>
                </Container>
                
                <div className="submit-button-div">
                  <Button
                    type="submit"
                    fullWidth
                    variant="contained"
                    color="primary"
                  >
                    Sign In
                  </Button>
                </div>
                <Grid className="bottom-form" container>
                  <Grid item xs>
                    <Link href="#" variant="body2">
                      Forgot password?
                    </Link>
                  </Grid>
                  <Grid item>
                    <Link
                      href="/sign-up"
                      variant="body2"
                    >
                      {"Don't have an account? Sign Up"}
                    </Link>
                  </Grid>
                </Grid>
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
