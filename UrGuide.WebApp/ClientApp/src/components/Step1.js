import React, { Fragment } from "react";
import IconButton from '@material-ui/core/IconButton';
import Input from '@material-ui/core/Input';
import InputLabel from '@material-ui/core/InputLabel';
import InputAdornment from '@material-ui/core/InputAdornment';
import Visibility from '@material-ui/icons/Visibility';
import VisibilityOff from '@material-ui/icons/VisibilityOff';
import AccountCircle from '@material-ui/icons/AccountCircle';
// import Avatar from "@material-ui/core/Avatar";
// import Button from "@material-ui/core/Button";
import CssBaseline from "@material-ui/core/CssBaseline";
// import TextField from "@material-ui/core/TextField";
// import FormControlLabel from "@material-ui/core/FormControlLabel";
// import FormControl from "@material-ui/core/FormControl";
// import Checkbox from "@material-ui/core/Checkbox";
// import Link from "@material-ui/core/Link";
// import Grid from "@material-ui/core/Grid";
// import Box from "@material-ui/core/Box";
// import LockOutlinedIcon from '@material-ui/icons/LockOutlined';
// import Typography from "@material-ui/core/Typography";
// import { makeStyles } from "@material-ui/core/styles";
import Container from "@material-ui/core/Container";
// import Select from "@material-ui/core/Select";
// import MenuItem from "@material-ui/core/MenuItem";
// import InputLabel from "@material-ui/core/InputLabel";
// import Stepper from "@material-ui/core/Stepper";
// import Step from "@material-ui/core/Step";
// import StepLabel from "@material-ui/core/StepLabel";
//import DateFnsUtils from "@date-io/date-fns";
//import MuiPickersUtilsProvider from "@material-ui/pickers/MuiPickersUtilsProvider";
//import KeyboardTimePicker from "@material-ui/pickers/DateTimePicker/KeyboardTimePicker";
//import KeyboardDatePicker from "@material-ui/pickers/DatePicker/DatePicker";
import "./RegisterLayout.css";
import { Grid } from "@material-ui/core";

export function Step1 () {

// const OnChangeConfirmation = (e) => {
//   let password = document.getElementById("password")
//   if(e.value!==password){
//     alert("not the same passwords")
//   }
// }

// const onChangeEmail = (e) => {
//   let email = e.value
//   let regex = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
//   let valid = regex.test(email);
//   if(!valid){
//     alert("incorrect email")
//   }
// }
//   const onChangePassword = (e) => {
//     let password = e.value
//     let regex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A -Za-z\d@$!%*?&]{ 8, 24 }$/
//     let valid = regex.test(password);
//     if (!valid) {
//       alert("incorrect password")
//     }
//   }

  const [values, setValues] = React.useState({
    amount: '',
    password: '',
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

  return (
    <Container component="main" maxWidth="xs" >
      <CssBaseline />
      <Grid container spacing={2}>
        <Grid item xs={12}>
          <InputLabel htmlFor="input-with-icon-adornment">Email</InputLabel>
          <Input
            endAdornment={
              <InputAdornment position="start">
                <AccountCircle />
              </InputAdornment>
            }
          />
        </Grid>
        <Grid item xs={12}>
          <InputLabel htmlFor="standard-adornment-password">Password</InputLabel>
          <Input
            type={values.showPassword ? 'text' : 'password'}
            onChange={handleChange('password')}
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
        </Grid>
        <Grid item xs={12}>
          <InputLabel htmlFor="standard-adornment-password">Password Confirm</InputLabel>
          <Input
            type={values.showPassword ? 'text' : 'password'}
            onChange={handleChange('password')}
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
        </Grid>
      </Grid>
    </Container>
  )
      // <Container component="main" maxWidth="xs">
      //   <CssBaseline />
      //   <Grid container spacing={2}>
      //     <Grid item xs={12}>
      //       <TextField
      //         variant="outlined"
      //         required
      //         fullWidth
      //         id="email"
      //         label="Email Address"
      //         name="email"
      //         autoComplete="email"
      //         onChange={this.onChangeEmail}
      //       />
      //     </Grid>
      //     <Grid item xs={12}>
      //       <TextField
      //         variant="outlined"
      //         required
      //         fullWidth
      //         id="password"
      //         label="Password"
      //         name="password"
      //         autoComplete="password"
      //         onChange={this.onChangePassword}
      //       />
      //     </Grid>
      //     <Grid item xs={12}>
      //       <TextField
      //         variant="outlined"
      //         required
      //         fullWidth
      //         id="password-confirm"
      //         label="Password Confirmation"
      //         name="passwordconfirmation"
      //         autoComplete="passwordconfirmation"
      //         onChange={this.OnChangeConfirmation}
      //       />
      //     </Grid>
      //   </Grid>
      // </Container>;
}
