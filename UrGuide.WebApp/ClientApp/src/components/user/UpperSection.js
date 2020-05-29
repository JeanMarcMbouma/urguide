import React, { Component, useMemo } from "react";
import {
    makeStyles,
    Button,
    IconButton,
    Avatar,
    Grid
} from "@material-ui/core";
import { Link } from 'react-router-dom';
import Rating from '@material-ui/lab/Rating';
import EditIcon from '@material-ui/icons/Edit';
import AddCircleIcon from '@material-ui/icons/AddCircle';
import AppsIcon from '@material-ui/icons/Apps';
import PhotoCameraIcon from '@material-ui/icons/PhotoCamera';
import NotesIcon from '@material-ui/icons/Notes';
import { FaRegCommentAlt } from 'react-icons/fa';
import Skeleton from '@material-ui/lab/Skeleton';
import { setTimeout } from 'timers';
import { useAuthUser } from "../api-authorization/AuthService";
import { HttpClientFactory } from './../../httpclient';
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

function UpperSectionSkeleton() {

    return (
        <div>
            <div className="row upper-card">
                <div className="col-12">
                    <div className='row' >
                        <div className='col-12'>
                            <div>
                                <div className="row justify-content-center">

                                    <div className="col-12 col-xl-3">
                                        <div className='container'>
                                            <Grid
                                                container
                                                spacing={0}
                                                direction="column"
                                                alignItems="center"
                                                justify="center"

                                            >
                                                <Grid item xs={6}>
                                                    <Skeleton animation="wave" variant="rect" style={{ width: `150px`, height: `150px`, borderRadius: `100px` }} />
                                                </Grid>
                                            </Grid>
                                        </div>
                                    </div>
                                    <div className="col-12 col-xl-8" >
                                        <br />
                                        <br />
                                        <h2 className='user-name'><Skeleton animation="wave" variant="rect" style={{ width: `250px`, height: `35px`, borderRadius: `0px` }} /></h2>
                                        <span className='user-profile-location'><Skeleton animation="wave" variant="rect" style={{ width: `200px`, height: `20px`, borderRadius: `0px` }} /></span>
                                        <br />
                                        <br />
                                        <p>
                                            <Skeleton variant="text" style={{ width: `100%` }} />
                                            <Skeleton variant="text" style={{ width: `100%` }} />
                                            <Skeleton variant="text" style={{ width: `100%` }} />
                    </p>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div className="nav-btn-div container">
                        <div className='row nav-btn-row justify-content-center' >
                            <Skeleton animation="wave" variant="rect" style={{ width: `100%`, height: `30px`, borderRadius: `0px`, marginBottom: `10px`, }} />
                        </div>
                    </div>

                </div>
            </div>
        </div>
    );
}


function RealUpperSection(props) {


    return (
        <div>
            <div className="row upper-card">
                <div className="col-12">
                    <div className='row' >
                        <div className='col-12'>
                            <div>
                                <div className="row justify-content-center">

                                    <div className="col-12 col-xl-3">
                                        <div className='container'>
                                            <Grid
                                                container
                                                spacing={0}
                                                direction="column"
                                                alignItems="center"
                                                justify="center"

                                            >
                                                <Grid item xs={6}>
                                                    <Avatar className='user-avatar' alt={props.name} src={props.profileImage} />
                                                </Grid>
                                            </Grid>
                                        </div>
                                    </div>
                                    <div className="col-12 col-xl-8" >
                                        <br />
                                        <br />
                                        <h2 className='user-name'>{props.name}</h2>
                                        <span className='user-profile-location' >{props.location}</span>
                                        <br />
                                        <br />
                                        <Rating name="read-only" value={props.rating} readOnly />
                                        <br />
                                        <br />
                                        <p>
                                            {props.description}
                                         </p>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div className="nav-btn-div container">
                        <div className='row nav-btn-row justify-content-center' >
                            <div className='col-12 col-sm-6 col-md-2 col-lg-2 col-xl-2 nav-col text-center'>
                                <Link style={{ textDecoration: `none` }} tag={Link}  to="/user" color="primary" onClick={(e) => ActivateLink(e)} >
                                    <NotesIcon fontSize="small" /> <span className='btn-title'>Posts (104)</span>
                                </Link>
                            </div>
                   <div className='col-12 col-sm-6 col-md-2 col-lg-2 col-xl-2 nav-col text-center'>
                                <Link style={{ textDecoration: `none` }} tag={Link} to="/user/galleries" color="primary" onClick={(e) => ActivateLink(e)} >
                                    <AppsIcon fontSize="small" /> <span className='btn-title'>Galleries (7)</span>
                                </Link>
                            </div>
                            
                                <div className='col-12 col-sm-6 col-md-2 col-lg-2 col-xl-2 nav-col text-center'>
                                    <Link style={{ textDecoration: `none` }} tag={Link} to="/user" color="primary" onClick={(e) => ActivateLink(e)} >
                                        <FaRegCommentAlt fontSize="large" /> <span className='btn-title'>Reviews (104)</span>
                                    </Link>
                                </div>
                               
                            <div className='col-12 col-sm-6 col-md-2 col-lg-2 col-xl-2 nav-col text-center'>
                                <Link style={{ textDecoration: `none` }} tag={Link}  to="/user/edit" color="primary" onClick={(e) => ActivateLink(e)} >
                                    <EditIcon fontSize="small" /> <span className='btn-title'>Edit profile</span>
                                </Link>
                            </div>
                                    <div className='col-12 col-sm-6 col-md-2  col-lg-2 col-xl-2 nav-col text-center'>
                                        <Link style={{ textDecoration: `none` }} tag={Link} to="/user/gallery/new" color="primary" onClick={(e) => ActivateLink(e)} >
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

export default function UpperSection(props)
{

    const user = props.user;
    const userId = props.userId;

    const [values, setValues] = React.useState({
        userId: null,
        profileImage: null,
        username: null,
        location: null,
        description: null,
        isGuide:false,
        loading: true,
    });

    useMemo(async () => {
        if (!user)
            return;
        var client = HttpClientFactory.getClient(user);
        var data = await client.getdetails();
        const timer = setTimeout(() => {
            setValues({
                userId: data.id,
                profileImage: data.profileImage,
                username: `${data.firstName} ${data.lastName}`,
                location: `${data.city}, ${data.country}`,
                description: data.description,
                isGuide: data.isGuide,
                loading: false
            });

        }, 4100);

        return () => clearTimeout(timer);

    }, [user]);

    const content = <RealUpperSection userId={values.userId} profileImage={values.profileImage} name={values.username} location={values.location} description={values.description} isGuide={values.isGuide} />

        return (
              <>
                {values.loading ? <UpperSectionSkeleton />
                    : content}
            </>
        );

}