import React from "react";
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
  CssBaseline
} from "@material-ui/core";
import { Visibility, VisibilityOff, AccountCircle } from "@material-ui/icons";
import clsx from "clsx";
import "./GuideRegistration.css";

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

export function Step1(props) {
  const classes = useStyles();

  const [values, setValues] = React.useState({
    amount: "",
    password: "",
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

  const emailTextField =
    props.emailError && !props.newly ? (
      <Grid item xs={12}>
        <FormControl
          fullWidth
          className={clsx(classes.margin, classes.textField)}
          variant="outlined"
        >
          <InputLabel error htmlFor="input-with-icon-adornment">
            Your email
          </InputLabel>
          <Input
            error
            id="guide-email"
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
      </Grid>
    ) : (
      <Grid item xs={12}>
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
            endAdornment={
              <InputAdornment position="start">
                <AccountCircle />
              </InputAdornment>
            }
          />
        </FormControl>
      </Grid>
    );

  const passwordTextField =
    props.passwordError && !props.newly ? (
      <Grid item xs={12}>
        <FormControl
          emailTextField
          fullWidth
          className={clsx(classes.margin, classes.textField)}
          variant="outlined"
        >
          <InputLabel error htmlFor="standard-adornment-password">
            Password
          </InputLabel>
          <Input
            id="guide-password"
            error
            type={values.showPassword ? "text" : "password"}
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
          your password must contains at least 8 alpha-numeric characters.
        </FormHelperText>
      </Grid>
    ) : (
      <Grid item xs={12}>
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
    );

  const passwordConfirmation =
    props.passwordsDontMatch && !props.newly ? (
      <Grid item xs={12}>
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
          The password and its confirmation do not match.
        </FormHelperText>
      </Grid>
    ) : (
      <Grid item xs={12}>
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
      </Grid>
    );

  return (
    <Container component="main" maxWidth="xs">
      <CssBaseline />
      <Grid container spacing={4}>
        <Grid item xs={12}>
          <Box mb={5}>
            <div>
              <h6>1.Enter your details for signing in the app.</h6>
            </div>
          </Box>
        </Grid>
        {emailTextField}
        {passwordTextField}
        {passwordConfirmation}
      </Grid>
    </Container>
  );
}
