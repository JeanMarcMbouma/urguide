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
import DatePicker from "./Date/Date";
import CountryPicker from './CountryPicker'

function readURL(input) {
  if (input.files && input.files[0]) {
    var reader = new FileReader();
    reader.onload = function (e) {
      document.getElementById("imagePreview").style.backgroundImage =
        "url(" + e.target.result + ")";
      document.getElementById("imagePreview").style.display = "none";
      document.getElementById("imagePreview").style.opacity = 1;
    };
    reader.readAsDataURL(input.files[0]);
  }
}

export class Step2 extends Component {
  render() {
    return (
      <Container component="main">
        <CssBaseline />
        <Grid container spacing={2}>
          <Grid item xs={12}>
            <div className="avatar-wrapper"><div className="avatar"></div></div>
          </Grid>
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
          <Grid item xs={12} sm={6}>
            <Select
              variant="outlined"
              labelId="demo-simple-select-outlined-label"
              id="demo-simple-select-outlined"
              fullWidth
              placeholder="Select your gender"
            >
              <MenuItem value={10}>Female</MenuItem>
              <MenuItem value={20}>Male</MenuItem>
            </Select>
          </Grid>
          <Grid item xs={12} sm={6}>
            <DatePicker />
          </Grid>
          <Grid item xs={12}>
            <CountryPicker region='Europe'/>
          </Grid>
          <Grid item xs={12} sm={6} md={4}>
            <TextField
              variant="outlined"
              required
              fullWidth
              id="city"
              label="City of residence"
              name="city"
              autoComplete="city"
            />
          </Grid>
          <Grid item xs={12} sm={6} md={4}>
            <TextField
              variant="outlined"
              required
              fullWidth
              id="phone"
              label="Phone Number"
              name="phone"
              autoComplete="phone"
            />
          </Grid>
          <Grid item xs={12} sm={6} md={4}>
            <TextField
              variant="outlined"
              required
              fullWidth
              id="address"
              label="Address"
              name="address"
              autoComplete="address"
            />
          </Grid>
          <Grid item xs={12}>
            <TextField
              variant="outlined"
              required
              fullWidth
              id="description"
              label="Short Description"
              name="description"
              placeholder="Write a short description about you"
              multiline
              rows={6}
              rowsMax={6}
            />
          </Grid>
        </Grid>
      </Container>
    );
  }
}
