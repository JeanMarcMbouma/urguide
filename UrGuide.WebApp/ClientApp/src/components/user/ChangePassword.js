import React, { Component } from "react";
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
    CssBaseline,
    Button
} from "@material-ui/core";
import EditProfileNavigation from "./EditProfileNavigation";
import { Visibility, VisibilityOff, AccountCircle } from "@material-ui/icons";
import clsx from "clsx";
import "./UserStyle.css";

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
      
    },
    form: {
        width: "100%", // Fix IE 11 issue.
        marginTop: theme.spacing(1)
    }
}));


function ChangePasswordForm() {
    const classes = useStyles();

    const [values, setValues] = React.useState({
        email: '',
        password: '',
        confirmPassword: '',
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

    const emailTextField =  (
        <Grid item xs={12} sm={6}>
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
                            value={values.email}
                            onChange={handleChange("email")}
                            endAdornment={
                                <InputAdornment position="start">
                                    <AccountCircle />
                                </InputAdornment>
                            }
                        />
                    </FormControl>
                </Grid>
            );

    const passwordTextField =  (
        <Grid item xs={12} sm={6}>
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
                            value={values.password}
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

    const passwordConfirmation =  (
        <Grid item xs={12} sm={6} >
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
                            onChange={handleChange("confirmPassword")}
                            value={values.confirmPassword}
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
        <div className='edit-profile-card' component="main">
            <h5 className='text-muted'>Change your login details</h5>
            <br />
            <br />
            <br />
            <Grid container spacing={4}>
                {emailTextField}
                {passwordTextField}
                {passwordConfirmation}
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





function Layout() {

    return (
        <div className='row justify-content-center'>
            <div className="col-12 col-lg-4 about">
                <EditProfileNavigation />
            </div>
            <div className="col-12 col-lg-7">
                <ChangePasswordForm />
            </div>
        </div>
 );
}


export default class ChangePassword extends Component {
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