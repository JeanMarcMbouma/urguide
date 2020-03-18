import React, { Component } from "react";
import Avatar from "@material-ui/core/Avatar";
import Button from "@material-ui/core/Button";
import CssBaseline from "@material-ui/core/CssBaseline";
import TextField from "@material-ui/core/TextField";
import FormControlLabel from "@material-ui/core/FormControlLabel";
import FormControl from "@material-ui/core/FormControl";
import Checkbox from "@material-ui/core/Checkbox";
import Link from "@material-ui/core/Link";
import Grid from "@material-ui/core/Grid";
import Box from "@material-ui/core/Box";
// import LockOutlinedIcon from '@material-ui/icons/LockOutlined';
import Typography from "@material-ui/core/Typography";
import { makeStyles } from "@material-ui/core/styles";
import Container from "@material-ui/core/Container";
import Select from "@material-ui/core/Select";
import MenuItem from "@material-ui/core/MenuItem";
import InputLabel from "@material-ui/core/InputLabel";
import Stepper from "@material-ui/core/Stepper";
import Step from "@material-ui/core/Step";
import StepLabel from "@material-ui/core/StepLabel";
//import DateFnsUtils from "@date-io/date-fns";
//import MuiPickersUtilsProvider from "@material-ui/pickers/MuiPickersUtilsProvider";
//import KeyboardTimePicker from "@material-ui/pickers/DateTimePicker/KeyboardTimePicker";
//import KeyboardDatePicker from "@material-ui/pickers/DatePicker/DatePicker";
import "./RegisterLayout.css";

export class Step1 extends Component {
  static displayName = Step1.name;

  render() {
    return (
      <Container component="main" maxWidth="xs">
        <CssBaseline />
        <Grid container spacing={2}>
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
              id="password"
              label="Password"
              name="password"
              autoComplete="password"
            />
          </Grid>
          <Grid item xs={12}>
            <TextField
              variant="outlined"
              required
              fullWidth
              id="password-confirm"
              label="Password Confirmation"
              name="passwordconfirmation"
              autoComplete="passwordconfirmation"
            />
          </Grid>
        </Grid>
      </Container>
    );
  }
}
