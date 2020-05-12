import React from "react";
import {
    makeStyles,
    Button,
    IconButton,
    Avatar
} from "@material-ui/core";
import { Link } from 'react-router-dom';
import Rating from '@material-ui/lab/Rating';
import EditIcon from '@material-ui/icons/Edit';
import AddCircleIcon from '@material-ui/icons/AddCircle';
import AppsIcon from '@material-ui/icons/Apps';
import PhotoCameraIcon from '@material-ui/icons/PhotoCamera';
import NotesIcon from '@material-ui/icons/Notes';
import { FaPlus } from 'react-icons/fa';
import { FaRegCommentAlt } from 'react-icons/fa';
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
                    <div>
                      
                        <div className='row justify-content-center'>
                            <div className='text-center col-12 col-sm-4 col-md-3 col-lg-2'>
                                <h3 className='text-center'> <Avatar className='user-avatar' /></h3>
                            </div>
                            <div className='col-12 col-sm-6 col-md-5 col-lg-3'>
                                <h2 className='user-name'>Jean Edgard Pilar</h2>
                                <div>
                                    <span className='user-profile-location'>Los Angeles, USA</span>
                                </div>
                                <h2 className='user-follow-button'>
                                    <Button variant="contained" color="default" type="button">
                                        <FaPlus /> <span className='btn-follow-title'> Follow</span>
                                    </Button>
                                </h2>
                            </div>
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
                <div className="col-12">
                    <div className='row' >
                        <div className='col-12'>
                            <ProfilePicture />
                        </div>
                    </div>
                    <div className="nav-btn-div container">
                        <div className='row nav-btn-row justify-content-center' >
                            <div className='col-12 col-lg-2 col-xl-2 nav-col text-center'>
                                <Link style={{ textDecoration: `none` }} tag={Link}  to="/user" color="primary" onClick={(e) => ActivateLink(e)} >
                                    <NotesIcon fontSize="small" /> <span className='btn-title'>Posts (104)</span>
                                </Link>
                            </div>
                            <div className='col-12 col-lg-2 col-xl-2 nav-col text-center'>
                                <Link style={{ textDecoration: `none` }} tag={Link}  to="/user/galleries" color="primary" onClick={(e) => ActivateLink(e)} >
                                    <AppsIcon fontSize="small" /> <span className='btn-title'>Galleries (7)</span>
                                </Link>
                            </div>
                            <div className='col-12 col-lg-2 col-xl-2 nav-col text-center'>
                                <Link style={{ textDecoration: `none` }} tag={Link} to="/user" color="primary" onClick={(e) => ActivateLink(e)} >
                                    <FaRegCommentAlt fontSize="large" /> <span className='btn-title'>Reviews (104)</span>
                                </Link>
                            </div>
                            <div className='col-12 col-lg-2 col-xl-2 nav-col text-center'>
                                <Link style={{ textDecoration: `none` }} tag={Link}  to="/user/edit/profile" color="primary" onClick={(e) => ActivateLink(e)} >
                                    <EditIcon fontSize="small" /> <span className='btn-title'>Edit profile</span>
                                </Link>
                            </div>
                            <div className='col-12  col-lg-2 col-xl-2 nav-col text-center'>
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
