import React from "react";
import {
  Grid,
  Box,
  FormHelperText,
  InputLabel,
  FormControl,
  MenuItem,
  Select,
  Input,
  Container,
  CssBaseline
} from "@material-ui/core";
import {
  MuiPickersUtilsProvider,
  KeyboardDatePicker
} from "@material-ui/pickers";
import MomentUtils from "@date-io/moment";
import CreateIcon from "@material-ui/icons/Create";
import Countries from "./Countries";
import "./GuideRegistration.css";

export function Step2(props) {
  function handleChange(event) {
    var file = URL.createObjectURL(event.target.files[0]);

    document.getElementById("pic-previewer").style.backgroundImage =
      "url('" + file + "')";
  }

  const [selectedDate, setSelectedDate] = React.useState(new Date());

  const handleDateChange = date => {
    setSelectedDate(date);
  };

  const profilePicGrid =
    props.profilePicError && !props.newly ? (
      <Grid item xs={12}>
        <div className="avatar-wrapper">
          <div className="avatar-error" id="pic-previewer"></div>
          <div
            className="create-icon-error"
            onClick={e => document.getElementById("profile-pic-input").click()}
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
          onChange={handleChange}
        />
        <FormHelperText error>
          please upload your profile picture.
        </FormHelperText>
      </Grid>
    ) : (
      <Grid item xs={12}>
        <div className="avatar-wrapper">
          <div className="avatar" id="pic-previewer"></div>
          <div
            className="create-icon"
            onClick={e => document.getElementById("profile-pic-input").click()}
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
          onChange={handleChange}
        />
      </Grid>
    );

  const fnameGrid =
    props.fnameError && !props.newly ? (
      <Grid item xs={12} sm={6}>
        <FormControl fullWidth variant="outlined">
          <InputLabel error htmlFor="input-with-icon-adornment">
            First Name
          </InputLabel>
          <Input error id="firstName" />
        </FormControl>
        <FormHelperText error>This field is required.</FormHelperText>
      </Grid>
    ) : (
      <Grid item xs={12} sm={6}>
        <FormControl fullWidth variant="outlined">
          <InputLabel htmlFor="input-with-icon-adornment">
            First Name
          </InputLabel>
          <Input id="firstName" />
        </FormControl>
      </Grid>
    );

  const lnameGrid =
    props.lnameError && !props.newly ? (
      <Grid item xs={12} sm={6}>
        <FormControl fullWidth variant="outlined">
          <InputLabel error htmlFor="input-with-icon-adornment">
            Last Name
          </InputLabel>
          <Input error id="lastName" />
        </FormControl>
        <FormHelperText error>This field is required.</FormHelperText>
      </Grid>
    ) : (
      <Grid item xs={12} sm={6}>
        <FormControl fullWidth variant="outlined">
          <InputLabel htmlFor="input-with-icon-adornment">Last Name</InputLabel>
          <Input id="lastName" />
        </FormControl>
      </Grid>
    );

  const genderGrid = (
    <Grid item xs={12} sm={6}>
      <InputLabel shrink id="gender-label">
        Gender
      </InputLabel>
      <Select
        fullWidth
        id="gender"
        labelId="gender-label"
        name="gender"
        defaultValue="Female"
        autoComplete="gender"
      >
        <MenuItem value="Female">Female</MenuItem>
        <MenuItem value="Male">Male</MenuItem>
      </Select>
    </Grid>
  );

  const birthdayGrid = (
    <Grid item xs={12} sm={6}>
      <MuiPickersUtilsProvider utils={MomentUtils}>
        <KeyboardDatePicker
          disableToolbar
          variant="inline"
          fullWidth
          margin="normal"
          format="MM/DD/YYYY"
          id="date-picker-inline"
          label="Birthday"
          value={selectedDate}
          onChange={handleDateChange}
          KeyboardButtonProps={{
            "aria-label": "change date"
          }}
          style={{ marginTop: 8 }}
        />
      </MuiPickersUtilsProvider>
    </Grid>
  );

  const countriesList = Countries.map(country => (
    <MenuItem value={country}>{country}</MenuItem>
  ));

  const countryGrid = (
    <Grid item xs={12} sm={6}>
      <InputLabel shrink id="residence-country">
        Country of residence
      </InputLabel>
      <Select
        fullWidth
        id="country"
        labelId="residence-country"
        name="country"
        defaultValue="Afghanistan"
        autoComplete="country"
      >
        {countriesList}
      </Select>
    </Grid>
  );

  const cityGrid =
    props.cityError && !props.newly ? (
      <Grid item xs={12} sm={6}>
        <FormControl fullWidth variant="outlined" style={{ marginTop: 8 }}>
          <InputLabel error htmlFor="input-with-icon-adornment">
            City of residence
          </InputLabel>
          <Input error id="city" />
        </FormControl>
        <FormHelperText error>Please enter your city.</FormHelperText>
      </Grid>
    ) : (
      <Grid item xs={12} sm={6} style={{ marginTop: 8 }}>
        <FormControl fullWidth variant="outlined">
          <InputLabel htmlFor="input-with-icon-adornment">
            City of residence
          </InputLabel>
          <Input id="city" />
        </FormControl>
      </Grid>
    );

  const phoneGrid =
    props.phoneError && !props.newly ? (
      <Grid item xs={12} sm={6}>
        <FormControl fullWidth variant="outlined">
          <InputLabel error htmlFor="input-with-icon-adornment">
            Phone Number
          </InputLabel>
          <Input error id="phone" />
        </FormControl>
        <FormHelperText error>
          Please enter a valid phone number.
        </FormHelperText>
      </Grid>
    ) : (
      <Grid item xs={12} sm={6}>
        <FormControl fullWidth variant="outlined">
          <InputLabel htmlFor="input-with-icon-adornment">
            Phone Number
          </InputLabel>
          <Input id="phone" />
        </FormControl>
      </Grid>
    );

  const addressGrid =
    props.addressError && !props.newly ? (
      <Grid item xs={12} sm={6}>
        <FormControl fullWidth variant="outlined">
          <InputLabel error htmlFor="input-with-icon-adornment">
            Address
          </InputLabel>
          <Input error id="address" />
        </FormControl>
        <FormHelperText error>Please enter your address.</FormHelperText>
      </Grid>
    ) : (
      <Grid item xs={12} sm={6}>
        <FormControl fullWidth variant="outlined">
          <InputLabel htmlFor="input-with-icon-adornment"> Address</InputLabel>
          <Input id="address" />
        </FormControl>
      </Grid>
    );

  const descriptionGrid =
    props.descriptionError && !props.newly ? (
      <Grid item xs={12}>
        <FormControl fullWidth variant="outlined">
          <InputLabel error htmlFor="input-with-icon-adornment">
            Short Description
          </InputLabel>
          <Input error id="description" multiline rows={6} rowsMax={6} />
        </FormControl>
        <FormHelperText error>
          Your description must have a minimum of 100 characters and a maximum
          of 500.
        </FormHelperText>
      </Grid>
    ) : (
      <Grid item xs={12}>
        <FormControl fullWidth variant="outlined">
          <InputLabel htmlFor="input-with-icon-adornment">
            Short Description
          </InputLabel>
          <Input id="description" multiline rows={6} rowsMax={6} />
        </FormControl>
      </Grid>
    );
  return (
    <Container component="main">
      <CssBaseline />
      <Grid item xs={12}>
        <Box mb={10}>
          <div>
            <h6>
              2.Add personal informations that will lead people to find you.
              {props.gender}
            </h6>
          </div>
        </Box>
      </Grid>
      <Grid container spacing={2}>
        {profilePicGrid}
        {fnameGrid}
        {lnameGrid}
        {genderGrid}
        {birthdayGrid}
        {countryGrid}
        {cityGrid}
        {phoneGrid}
        {addressGrid}
        {descriptionGrid}
      </Grid>
    </Container>
  );
}
