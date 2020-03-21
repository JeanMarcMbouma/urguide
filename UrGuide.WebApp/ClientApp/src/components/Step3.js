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

export class Step3 extends Component {
  constructor(props) {
    super(props);
    this.state = { country: "Select your country...", file: null };

    this.handleChange = this.handleChange.bind(this);
  }

  handleChange(event) {
    var file = URL.createObjectURL(event.target.files[0]);
    var elementId = String(event.target.id);
    elementId = elementId.substring(0, elementId.length - 2);
    this.setState({
      file: elementId //file
    });

    document.getElementById(elementId).style.backgroundImage =
      "url('" + file + "')";
  }

  render() {
    return (
      <Container component="main">
        <CssBaseline />
        <Grid item xs={12}>
          <Box mb={5}>
            <div>
              <h6>
                3.Now upload photos of places you'd want your guests to visit.
              </h6>
            </div>
          </Box>
        </Grid>
        <Grid container spacing={2}>
          <Grid item xs={12} sm={6} md={4}>
            <Box>
              <div
                className="photo-box text-center"
                id="pic-1"
                onClick={e => document.getElementById("pic-1-i").click()}
              >
                <span>Upload an image here</span>
                <input
                  className="input-file"
                  id="pic-1-i"
                  type="file"
                  accept=".png,.jpg"
                  onChange={this.handleChange}
                />
              </div>
            </Box>
          </Grid>
          <Grid item xs={12} sm={6} md={4}>
            <Box>
              <div
                className="photo-box text-center"
                id="pic-2"
                onClick={e => document.getElementById("pic-2-i").click()}
              >
                <span>Upload an image here</span>
                <input
                  className="input-file"
                  id="pic-2-i"
                  type="file"
                  accept=".png,.jpg"
                  onChange={this.handleChange}
                />
              </div>
            </Box>
          </Grid>
          <Grid item xs={12} sm={6} md={4}>
            <Box>
              <div
                className="photo-box text-center"
                id="pic-3"
                onClick={e => document.getElementById("pic-3-i").click()}
              >
                <span>Upload an image here</span>
                <input
                  className="input-file"
                  id="pic-3-i"
                  type="file"
                  accept=".png,.jpg"
                  onChange={this.handleChange}
                />
              </div>
            </Box>
          </Grid>
          <Grid item xs={12} sm={6} md={4}>
            <Box>
              <div
                className="photo-box text-center"
                id="pic-4"
                onClick={e => document.getElementById("pic-4-i").click()}
              >
                <span>Upload an image here</span>
                <input
                  className="input-file"
                  id="pic-4-i"
                  type="file"
                  accept=".png,.jpg"
                  onChange={this.handleChange}
                />
              </div>
            </Box>
          </Grid>
          <Grid item xs={12} sm={6} md={4}>
            <Box>
              <div
                className="photo-box text-center"
                id="pic-5"
                onClick={e => document.getElementById("pic-5-i").click()}
              >
                <span>Upload an image here</span>
                <input
                  className="input-file"
                  id="pic-5-i"
                  type="file"
                  accept=".png,.jpg"
                  onChange={this.handleChange}
                />
              </div>
            </Box>
          </Grid>
          <Grid item xs={12} sm={6} md={4}>
            <Box>
              <div
                className="photo-box text-center"
                id="pic-6"
                onClick={e => document.getElementById("pic-6-i").click()}
              >
                <span>Upload an image here</span>
                <input
                  className="input-file"
                  id="pic-6-i"
                  type="file"
                  accept=".png,.jpg"
                  onChange={this.handleChange}
                />
              </div>
            </Box>
          </Grid>
          <Grid item xs={12}>
            <Box mt={5}>
              <FormControlLabel
                control={<Checkbox value="allowExtraEmails" color="primary" />}
                label="I agree with the Terms and Conditons."
              />
            </Box>
          </Grid>
        </Grid>
      </Container>
    );
  }
}
