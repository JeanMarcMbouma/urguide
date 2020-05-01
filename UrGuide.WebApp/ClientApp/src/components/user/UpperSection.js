import React from "react";
import {
    makeStyles,
    Button,
    IconButton
} from "@material-ui/core";
import { Link } from 'react-router-dom';
import Rating from '@material-ui/lab/Rating';
import EditIcon from '@material-ui/icons/Edit';
import AddCircleIcon from '@material-ui/icons/AddCircle';
import AppsIcon from '@material-ui/icons/Apps';
import PhotoCameraIcon from '@material-ui/icons/PhotoCamera';
import NotesIcon from '@material-ui/icons/Notes';
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
                            <div className="avatar"></div>
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

function ActivateLink(event) {

    var buttons = document.querySelectorAll("a");

    [].forEach.call(buttons, function (el) {
        el.classList.remove("active-nav-link");
    });

    var divs = document.querySelectorAll("div");

    [].forEach.call(divs, function (el) {
        el.classList.remove("active-nav-col");
    });

    var target = event.target;
    var icon = target.closest("a");
    var div = target.closest("div div");
    //alert(target);
    icon.className += ' active-nav-link';
    div.className += ' active-nav-col';

}

export function UpperSection() {

    const classes = buttonStyles();

    return (
        <div>
            <div className="row upper-card">
                <div className="col-12 col-lg-3">
                <ProfilePicture/>
            </div>
                <div className="col-12 col-lg-9">
                    <div className='row' >
                        <div className='col-12'>
                            <h4>Allyson w.</h4>
                            <span className="text-muted">Los angeles, USA</span>
                            <br />
                            <br />
                            <p>
                                Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged.
                            </p>
                        </div>
                    </div>
                    <div className="nav-btn-div container">
                        <div className='row nav-btn-row justify-content-center' >
                            <div className='col-12 col-sm-3 col-xl-2 nav-col text-center'>
                                <Link style={{ textDecoration: `none` }} tag={Link}  to="/user" color="primary" onClick={(e) => ActivateLink(e)} >
                                    <NotesIcon fontSize="small" /> <span className='btn-title'>Posts (104)</span>
                                </Link>
                            </div>
                            <div className='col-12 col-sm-3 col-xl-2 nav-col text-center'>
                                <Link style={{ textDecoration: `none` }} tag={Link}  to="/user/galleries" color="primary" onClick={(e) => ActivateLink(e)} >
                                    <AppsIcon fontSize="small" /> <span className='btn-title'>Galleries (7)</span>
                                </Link>
                            </div>
                            <div className='col-12 col-sm-3 col-xl-2 nav-col text-center'>
                                <Link style={{ textDecoration: `none` }} tag={Link}  to="/user/edit/profile" color="primary" onClick={(e) => ActivateLink(e)} >
                                    <EditIcon fontSize="small" /> <span className='btn-title'>Edit profile</span>
                                </Link>
                            </div>
                            <div className='col-12 col-sm-3 col-xl-2 nav-col text-center'>
                                <Link style={{ textDecoration: `none` }} tag={Link}  to="/user/gallery/new" color="primary" onClick={(e) => ActivateLink(e)} >
                                    <PhotoCameraIcon fontSize="small" /> <span className='btn-title'>New Gallery</span>
                                </Link>
                            </div>
                        </div>
                    </div>
                  
            </div>
            </div>
        </div>
    );
}
