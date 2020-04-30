import React, { Component } from "react";
import {
    Grid,
    InputLabel,
    FormControl,
    MenuItem,
    Select,
    Input,
    Button,
    CssBaseline
} from "@material-ui/core";
import {
    MuiPickersUtilsProvider,
    KeyboardDatePicker
} from "@material-ui/pickers";
import MomentUtils from "@date-io/moment";
import CreateIcon from "@material-ui/icons/Create";
import Countries from "./../guide-registration/Countries";
import { UpperSection } from "./UpperSection";
import EditProfileNavigation from "./EditProfileNavigation";
import "./UserStyle.css";

function Profile() {
    function handleChange(event) {
        var file = URL.createObjectURL(event.target.files[0]);

        document.getElementById("pic-previewer").style.backgroundImage =
            "url('" + file + "')";

    }


    const [values, setValues] = React.useState({
        firstName: '',
        lastName: '',
        gender: 'Female',
        birthday: '',
        country: 'Afghanistan',
        city: '',
        phone: '',
        address: '',
        description: '',
        picture: '',
    });

    const handleChangedValue = prop => event => {
        setValues({ ...values, [prop]: event.target.value });
    };

    const [selectedDate, setSelectedDate] = React.useState(new Date());

    const handleDateChange = date => {
        setSelectedDate(date);

    };

    const profilePicGrid =  (
                <Grid item xs={12}>
                    <div className="edit-avatar-wrapper">
                        <div className="edit-avatar" id="pic-previewer"></div>
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
         (
                <Grid item xs={12} sm={6}>
                    <FormControl fullWidth variant="outlined">
                        <InputLabel htmlFor="input-with-icon-adornment">
                            First Name
          </InputLabel>
                        <Input id="firstName" value={values.firstName}
                            onChange={handleChangedValue("firstName")} />
                    </FormControl>
                </Grid>
            );

    const lnameGrid =
       (
                <Grid item xs={12} sm={6}>
                    <FormControl fullWidth variant="outlined">
                        <InputLabel htmlFor="input-with-icon-adornment">Last Name</InputLabel>
                        <Input id="lastName" value={values.lastName}
                            onChange={handleChangedValue("lastName")} />
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
                value={values.gender}
                onChange={handleChangedValue("gender")}
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
         (
                <Grid item xs={12} sm={6} style={{ marginTop: 8 }}>
                    <FormControl fullWidth variant="outlined">
                        <InputLabel htmlFor="input-with-icon-adornment">
                            City of residence
          </InputLabel>
                        <Input id="city" value={values.city}
                            onChange={handleChangedValue("city")} />
                    </FormControl>
                </Grid>
            );

    const phoneGrid =
         (
                <Grid item xs={12} sm={6}>
                    <FormControl fullWidth variant="outlined">
                        <InputLabel htmlFor="input-with-icon-adornment">
                            Phone Number
          </InputLabel>
                        <Input id="phone" value={values.phone}
                            onChange={handleChangedValue("phone")} />
                    </FormControl>
                </Grid>
            );

    const addressGrid =
  (
                <Grid item xs={12} sm={6}>
                    <FormControl fullWidth variant="outlined">
                        <InputLabel htmlFor="input-with-icon-adornment"> Address</InputLabel>
                        <Input id="address" value={values.address}
                            onChange={handleChangedValue("address")} />
                    </FormControl>
                </Grid>
            );

    const descriptionGrid =
     (
                <Grid item xs={12}>
                    <FormControl fullWidth variant="outlined">
                        <InputLabel htmlFor="input-with-icon-adornment">
                            Short Description
          </InputLabel>
                        <Input id="description" value={values.description}
                            onChange={handleChangedValue("description")} multiline rows={6} rowsMax={6} />
                    </FormControl>
                </Grid>
            );
    return (
        <div className='edit-profile-card' component="main">
            <CssBaseline />
            <h5 className='text-muted'>Change your information.</h5>
            <br />
            <br />
            <br />
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
            <br />
            <br />
            <Button
                variant="contained"
                color="primary"
            >
                Save Changes
              </Button>
        </div>
    );
}



function Layout(){

        return (
            <div className='row justify-content-center'>
                <div className="col-12 col-lg-4 about">
                    <EditProfileNavigation />
                </div>
                <div className="col-12 col-lg-7">
                    <Profile />
                </div>
            </div>
        );


}

export default class EditProfile extends Component {
    render() {
        return (
            <div className="container-fluid user-page-container">
                <div className="row">
                    <div className="col-12">
                        <UpperSection />
                    </div>
                </div>
                <div className="row">
                    <div className="col-12 lower-section">
                        <Layout />
                    </div>
                </div>
            </div>
        )
    }
}