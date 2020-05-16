import React, { Component, useMemo, useState, useContext, useReducer } from "react";
import {
    Grid,
    InputLabel,
    FormControl,
    MenuItem,
    Select,
    Input,
    Button,
    FormHelperText,
    Avatar
} from "@material-ui/core";
import {
    MuiPickersUtilsProvider,
    KeyboardDatePicker
} from "@material-ui/pickers";
import EditProfileContext from "./profile/EditProfileContext";
import EditProfileReducer from "./profile/EditProfileReducer";
import MomentUtils from "@date-io/moment";
import CreateIcon from "@material-ui/icons/Create";
import Countries from "./../guide-registration/Countries";
import Alert from '@material-ui/lab/Alert';
import EditProfileNavigation from "./EditProfileNavigation";
import "./UserStyle.css";
import { useAuthUser } from "../api-authorization/AuthService";
import { UpdateGuideModel } from './../../api';
import { HttpClientFactory } from './../../httpclient';
import { BlobToBase64 } from "../../helpers/fileHelpers";

function Profile() {

    const user = useAuthUser();

    const [values, setValues] = React.useState({
        id:null,
        firstName: '',
        lastName: '',
        gender:'',
        birthDay:new Date(),
        country: '',
        city: '',
        phoneNumber: '',
        address: '',
        description: '',
        profileImage: '',
    });

    const [status, setStatus] = useState(0);

    async function editProfile(state) {

        const client = HttpClientFactory.getClient(user);

        const model = new UpdateGuideModel({
            id: state.id,
            firstName: state.firstName,
            lastName: state.lastName,
            birthDay: state.birthDay,
            gender:state.gender,
            country: state.country,
            city: state.city,
            phone: state.phoneNumber,
            address: state.address,
            description: state.description,
            profileImage: state.profileImage
        });

        try {

            await client.updateguide(model);
            setStatus(200);
            return 200;
        }
        catch (e) {

            setStatus(400);
            return 400;

        }

    }

    const ctx = useContext(EditProfileContext);
    const [state, dispatch] = useReducer(EditProfileReducer, ctx);

    useMemo(async () => {
        if (!user)
            return;
        var client = HttpClientFactory.getClient(user);
        var data = await client.getdetails();
        // console.log(data);
        setValues({
            id: data.id,
            firstName: data.firstName,
            lastName: data.lastName,
            birthDay: data.birthDay,
            gender: data.gender,
            country: data.country,
            city: data.city,
            phoneNumber: data.phoneNumber,
            address: data.address,
            description: data.description,
            profileImage: data.profileImage
        });
    }, [user]);

    const handleChangedValue = prop => event => {
        setValues({ ...values, [prop]: event.target.value });
    };

    const handleDateChange = date => {
         setValues({ ...values, ['birthDay']: date });
    };

    function CountriesList(props) {
        return (<Select
            fullWidth
            id="country"
            labelId="residence-country"
            name="country"
            value={props.country}
            defaultValue={props.country}
            autoComplete="country"
            onChange={handleChangedValue("country")}

        >
            {
                Countries.map((country, index) => (
                    <MenuItem key={index} value={country}>{country}</MenuItem>))
            }
        </Select>);
    }

    function handleChange(event) {
        const blob = event.target.files[0];
        BlobToBase64(blob, (fileName, base64Url, blobUrl) => {
            document.getElementById("pic-previewer").src = blobUrl;

            setValues({ ...values, ['profileImage']: base64Url });
        });
    }


    const profilePicGrid =  (
                <Grid item xs={12}>
            <div className="edit-avatar-wrapper">
                {values.profileImage ?

                    <>
                       <Avatar id="pic-previewer" className='user-avatar' alt={values.firstName} src={values.profileImage} />
                        <div
                            className="create-icon"
                            onClick={e => document.getElementById("profile-pic-input").click()}
                        >
                            <span>
                                <CreateIcon style={{ fontSize: 21, marginTop: `-6px`, }} />
                            </span>
                        </div>  
                    </>
                    
                    : 
                    <>

                        <Avatar id="pic-previewer" className='user-avatar' alt={values.firstName} src={values.profileImage} />
                        <div
                            className="create-icon"
                            onClick={e => document.getElementById("profile-pic-input").click()}
                        >
                            <span>
                                <CreateIcon style={{ fontSize: 21, marginTop: `-6px`, }} />
                            </span>
                        </div>
                        <br />
                        <br />
                        {state.profileImageError ? <FormHelperText error>
                            {state.requiredErrorMessage}
                        </FormHelperText> : null}
                    </>
                     
                    }
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

    return (
        <div className='edit-profile-card' component="main">

            <h5 className='text-muted'>Change your informations.</h5>
            <br />
            <br />
            {status == 200 ? <Alert severity="success">Your informations have been successfully changed!</Alert> : null}
            {status == 400 ? <Alert severity="error">Oops sorry ! something went wrong. Please try again.</Alert> : null}
            <br />
            <br />
            <form >
                <Grid container spacing={2}>
                    {profilePicGrid}
                    <Grid item xs={12} sm={6}>
                        <FormControl fullWidth variant="outlined">
                            <InputLabel htmlFor="input-with-icon-adornment">
                                First Name
          </InputLabel>
                            <Input id="firstName" value={values.firstName}
                                onChange={handleChangedValue("firstName")} />
                        </FormControl>
                        {state.fnameError ? <FormHelperText error>
                            {state.requiredNameErrorMessage}
                        </FormHelperText> : null}
                    </Grid>
                    <Grid item xs={12} sm={6}>
                        <FormControl fullWidth variant="outlined">
                            <InputLabel htmlFor="input-with-icon-adornment">Last Name</InputLabel>
                            <Input id="lastName" value={values.lastName}
                                onChange={handleChangedValue("lastName")} />
                        </FormControl>
                        {state.lnameError ? <FormHelperText error>
                            {state.requiredNameErrorMessage}
                        </FormHelperText> : null}
                    </Grid>
                    <Grid item xs={12} sm={6}>
                        <InputLabel shrink id="gender-label">
                            Gender
      </InputLabel>
                        <Select
                            fullWidth
                            id="gender"
                            labelId="gender-label"
                            name="gender"
                            value={values.gender}
                            defaultValue={values.gender}
                            autoComplete="gender"
                            onChange={handleChangedValue("gender")}
                        >
                            <MenuItem value="Female">Female</MenuItem>
                            <MenuItem value="Male">Male</MenuItem>
                        </Select>
                    </Grid>
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
                                value={values.birthDay}
                                onChange={handleDateChange}
                                KeyboardButtonProps={{
                                    "aria-label": "change date"
                                }}
                                style={{ marginTop: 8 }}
                            />
                        </MuiPickersUtilsProvider>
                    </Grid>
                    <Grid item xs={12} sm={6}>
                        <InputLabel shrink id="residence-country">
                            Country of residence
      </InputLabel>
                        <CountriesList country={values.country} />
                    </Grid>
                    <Grid item xs={12} sm={6} style={{ marginTop: 8 }}>
                        <FormControl fullWidth variant="outlined">
                            <InputLabel htmlFor="input-with-icon-adornment">
                                City of residence
          </InputLabel>
                            <Input id="city" value={values.city}
                                onChange={handleChangedValue("city")} />
                        </FormControl>
                        {state.cityError ? <FormHelperText error>
                            {state.requiredErrorMessage}
                        </FormHelperText> : null}
                    </Grid>
                    <Grid item xs={12} sm={6}>
                        <FormControl fullWidth variant="outlined">
                            <InputLabel htmlFor="input-with-icon-adornment"> Phone Number</InputLabel>
                            <Input id="phone" value={values.phoneNumber}
                                onChange={handleChangedValue("phoneNumber")} />
                        </FormControl>
                        {state.phoneNumberError ? <FormHelperText error>
                            {state.requiredErrorMessage}
                        </FormHelperText> : null}
                    </Grid>
                    <Grid item xs={12} sm={6}>
                        <FormControl fullWidth variant="outlined">
                            <InputLabel htmlFor="input-with-icon-adornment"> Address</InputLabel>
                            <Input id="address" value={values.address}
                                onChange={handleChangedValue("address")} />
                        </FormControl>
                        {state.addressError ? <FormHelperText error>
                            {state.requiredErrorMessage}
                        </FormHelperText> : null}
                    </Grid>
                    <Grid item xs={12}>
                        <FormControl fullWidth variant="outlined">
                            <InputLabel htmlFor="input-with-icon-adornment">
                                Short Description
          </InputLabel>
                            <Input id="description" value={values.description}
                                onChange={handleChangedValue("description")} multiline rows={6} rowsMax={6} />
                        </FormControl>
                    {state.descriptionError ? <FormHelperText error>
                        {state.descriptionErrorMessage }
                    </FormHelperText> : null}
                    </Grid>
                </Grid>
                <br />
                <br />
                <Button
                    variant="contained"
                    color="primary"
                    type="button"
                    onClick={() =>
                        dispatch({
                            type: "editProfile",
                            data: {
                                id:values.id,
                                profileImage: values.profileImage,
                                firstName: values.firstName,
                                lastName: values.lastName,
                                gender: values.gender,
                                birthDay: values.birthDay,
                                country: values.country,
                                city: values.city,
                                phoneNumber: values.phoneNumber,
                                address: values.address,
                                description: values.description,
                                callback: editProfile,
                            }
                        })}
                >
                    Save Changes
              </Button>
            </form>
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
            <div className="row">
                <div className="col-12 lower-section">
                    <Layout />
                </div>
            </div>
        )
    }
}