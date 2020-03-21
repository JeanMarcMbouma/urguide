import React, { Component, Fragment, useState } from "react";
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
import DateFnsUtils from "@date-io/date-fns";
import CreateIcon from "@material-ui/icons/Create";
import "./RegisterLayout.css";

export class Step2 extends Component {
  constructor(props) {
    super(props);
    this.state = { country: "Select your country...", file: null };

    this.handleChange = this.handleChange.bind(this);
  }

  handleChange(event) {
    var file = URL.createObjectURL(event.target.files[0]);

    this.setState({
      file: file
    });

    document.getElementById("pic-previewer").style.backgroundImage =
      "url('" + file + "')";
  }

  render() {
    return (
      <Container component="main">
        <CssBaseline />
        <Grid item xs={12}>
          <Box mb={10}>
            <div>
              <h6>
                2.Add personal informations that will lead people to find
                you.
              </h6>
            </div>
          </Box>
        </Grid>
        <Grid container spacing={2}>
          <Grid item xs={12}>
            <div className="avatar-wrapper">
              <div className="avatar" id="pic-previewer"></div>
              <div
                className="create-icon"
                onClick={e =>
                  document.getElementById("profile-pic-input").click()
                }
              >
                <span>
                  <CreateIcon style={{ fontSize: 21 }} />
                </span>
              </div>
            </div>
            <input
              type="file"
              className="input-file"
              id="profile-pic-input"
              accept=".png,.jpg"
              onChange={this.handleChange}
            />
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
              fullWidth
              id="gender"
              label="Gender"
              name="gender"
              defaultValue="default-gender"
              autoComplete="gender"
            >
              <MenuItem value="default-gender">Select your gender...</MenuItem>
              <MenuItem value="Female">Female</MenuItem>
              <MenuItem value="Male">Male</MenuItem>
            </Select>
          </Grid>
          <Grid item xs={12} sm={6}>
            <TextField
              variant="outlined"
              required
              fullWidth
              id="birthday"
              label="Birthday"
              name="birthday"
              autoComplete="birthday"
            />
          </Grid>
          <Grid item xs={12}>
            <Select
              variant="outlined"
              fullWidth
              id="country"
              label="Country"
              name="country"
              defaultValue="default-country"
              autoComplete="country"
            >
              <MenuItem value="default-country">
                Select your country...
              </MenuItem>
              <MenuItem value="afghanistan">Afghanistan</MenuItem>
            </Select>
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
