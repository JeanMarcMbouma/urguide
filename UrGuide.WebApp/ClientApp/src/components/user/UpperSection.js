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
    CssBaseline,
    Button
} from "@material-ui/core";
import Rating from '@material-ui/lab/Rating';
import EditIcon from '@material-ui/icons/Edit';
import AddCircleIcon from '@material-ui/icons/AddCircle';
import clsx from "clsx";
import "./UserStyle.css";

const useStyles = makeStyles((theme) => ({
    root: {
        display: 'flex',
        flexDirection: 'column',
        '& > * + *': {
            marginTop: theme.spacing(1),
        },
     
    }
}));

const buttonStyles = makeStyles(theme => ({
    root: {
        '& > *': {
            margin: theme.spacing(1),
        },
    },
}));

function ProfilePicture() {

const classes = useStyles();

    return (

        <div>
            <div className="row">
                <div className="col-12">
                         <div className="avatar-wrapper">
                            <div className="avatar" id="pic-previewer"></div>
                        </div>
                </div>
                <div className="col-12" >
                        <div className="rating-and-reviews">
                        <div className={classes.root}>
                            <h3 className='text-center'>
                                <Rating name="half-rating-read" defaultValue={2.5} precision={0.5} readOnly />
                            </h3>

                            </div>
                            <div className="reviews text-center">
                                <span> reviews (250)</span>
                            </div>
                        </div>
                 </div>
          
            </div>
        </div>);

}


export function UpperSection() {

    const classes = buttonStyles();

    return (
        <div className="container-fluid">
        <div className="row">
            <div className="col-12 col-lg-4">
                <ProfilePicture/>
            </div>
                <div className="col-12 col-lg-8">
                    <div className="nav-btn-div">
                        <div className={classes.root} >
                            <Button variant="contained" color="default"><EditIcon fontSize="small" /> <span className='btn-title'>Edit profile</span></Button>
                            <Button variant="contained" color="default" ><AddCircleIcon fontSize="small" /> <span className='btn-title'>New gallery</span></Button>
                            </div>
                    </div>
                  
            </div>
            </div>
        </div>
    );
}
