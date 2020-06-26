import React, { Component, useMemo } from "react";
import {
    makeStyles,
    Button,
    IconButton,
    Avatar,
    Grid
} from "@material-ui/core";
import { Link, useRouteMatch } from 'react-router-dom';
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
import { LookupClient } from "../../api";
import { useDataContext } from "../../data/GlobalDataContext";

const useStyles = makeStyles((theme) => ({
    root: {
        display: 'flex',
        flexDirection: 'column',
        '& > * + *': {
            marginTop: theme.spacing(1),
        },
  
     
    }
}));



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
                                        <h2 className='user-name'><Skeleton animation="wave" variant="rect" style={{ width: `250px`, height: `20px`, borderRadius: `8px` }} /></h2>
                                        <span className='user-profile-location'><Skeleton animation="wave" variant="rect" style={{ width: `200px`, height: `20px`, borderRadius: `8px` }} /></span>
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
                        <div className='row nav-btn-row justify-content-start' >
                        <div className='col-2 col-md-2 col-lg-2 col-xl-2 nav-col text-center'>
                                <Skeleton animation="wave" variant="rect" style={{ width: `100%`, height: `20px`, borderRadius: `8px`, marginBottom: `10px`, }} />
                            </div>
                            <div className='col-2 col-md-2 col-lg-2 col-xl-2 nav-col text-center'>
                                <Skeleton animation="wave" variant="rect" style={{ width: `100%`, height: `20px`, borderRadius: `8px`, marginBottom: `10px`, }} />
                            </div>
                            <div className='col-2 col-md-2 col-lg-2 col-xl-2 nav-col text-center'>
                                <Skeleton animation="wave" variant="rect" style={{ width: `100%`, height: `20px`, borderRadius: `8px`, marginBottom: `10px`, }} />
                            </div>
                            <div className='col-2 col-md-2 col-lg-2 col-xl-2 nav-col text-center'>
                                <Skeleton animation="wave" variant="rect" style={{ width: `100%`, height: `20px`, borderRadius: `8px`, marginBottom: `10px`, }} />
                            </div>
                            <div className='col-2 col-md-2 col-lg-2 col-xl-2 nav-col text-center'>
                                <Skeleton animation="wave" variant="rect" style={{ width: `100%`, height: `20px`, borderRadius: `8px`, marginBottom: `10px`, }} />
                            </div>
                        </div>
                    </div>

                </div>
            </div>
        </div>
    );
}


function RealUpperSection(props) {


    let { path } = useRouteMatch();

    const { dataContext } = useDataContext();

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
                                        <Rating name="read-only" value={+props.rating} readOnly />
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
                        <div className='row nav-btn-row justify-content-start' >
                           
                            {
                                !props.visitor ?

                                    <>
                                        {dataContext.profileUrl === "/Reviews" ? <div className='col-2 col-md-2 col-lg-2 col-xl-2 nav-col text-center active-nav-col'>
                                            <Link className="active-nav-link" style={{ textDecoration: `none` }} tag={Link} to={`/profile`} color="primary"  >
                                                <FaRegCommentAlt fontSize="large" /> <span className='btn-title title-nav'>Reviews</span>
                                            </Link>
                                        </div> : <div className='col-2 col-md-2 col-lg-2 col-xl-2 nav-col text-center'>
                                                <Link style={{ textDecoration: `none` }} tag={Link} to={`/profile`} color="primary"  >
                                                    <FaRegCommentAlt fontSize="large" /> <span className='btn-title title-nav'>Reviews</span>
                                                </Link>
                                            </div>
                                        }
                                        {
                                            dataContext.profileUrl === "/Posts" ? <div className='col-2 col-md-2 col-lg-2 col-xl-2 nav-col text-center active-nav-col'>
                                                <Link className="active-nav-link" style={{ textDecoration: `none` }} tag={Link} to={`/profile/posts`} color="primary" >
                                                    <NotesIcon fontSize="small" /> <span className='btn-title title-nav'>Posts</span>
                                                </Link>
                                            </div> : <div className='col-2 col-md-2 col-lg-2 col-xl-2 nav-col text-center'>
                                                    <Link style={{ textDecoration: `none` }} tag={Link} to={`/profile/posts`} color="primary" >
                                                        <NotesIcon fontSize="small" /> <span className='btn-title title-nav'>Posts</span>
                                                    </Link>
                                                </div>
                                        }
                                        {
                                            dataContext.profileUrl === "/Galleries" ? <div className='col-2 col-md-2 col-lg-2 col-xl-2 nav-col text-center active-nav-col'>
                                                <Link className="active-nav-link" style={{ textDecoration: `none` }} tag={Link} to={`/profile/galleries`} color="primary"  >
                                                    <AppsIcon fontSize="small" /> <span className='btn-title title-nav'>Galleries</span>
                                                </Link>
                                            </div> : <div className='col-2 col-md-2 col-lg-2 col-xl-2 nav-col text-center'>
                                                    <Link style={{ textDecoration: `none` }} tag={Link} to={`/profile/galleries`} color="primary"  >
                                                        <AppsIcon fontSize="small" /> <span className='btn-title title-nav'>Galleries</span>
                                                    </Link>
                                                </div>
                                        }
                                        {
                                            dataContext.profileUrl === "/Edit" ? <div className='col-2 col-md-2 col-lg-2 col-xl-2 nav-col text-center active-nav-col'>
                                                <Link className="active-nav-link" style={{ textDecoration: `none` }} tag={Link} to={`/profile/details`} color="primary" >
                                                    <EditIcon fontSize="small" /> <span className='btn-title title-nav'>Edit profile</span>
                                                </Link>
                                            </div> : <div className='col-2 col-md-2 col-lg-2 col-xl-2 nav-col text-center'>
                                                    <Link style={{ textDecoration: `none` }} tag={Link} to={`/profile/details`} color="primary" >
                                                        <EditIcon fontSize="small" /> <span className='btn-title title-nav'>Edit profile</span>
                                                    </Link>
                                                </div>
                                        }
                                        {
                                            dataContext.profileUrl === "/NewGallery" ? <div className='col-2 col-md-2  col-lg-2 col-xl-2 nav-col text-center active-nav-col'>
                                                <Link className="active-nav-link" style={{ textDecoration: `none` }} tag={Link} to={`/profile/creategallery`} color="primary"  >
                                                    <PhotoCameraIcon fontSize="small" /> <span className='btn-title title-nav'>New Gallery</span>
                                                </Link>
                                            </div> : <div className='col-2 col-md-2  col-lg-2 col-xl-2 nav-col text-center'>
                                                    <Link style={{ textDecoration: `none` }} tag={Link} to={`/profile/creategallery`} color="primary"  >
                                                        <PhotoCameraIcon fontSize="small" /> <span className='btn-title title-nav'>New Gallery</span>
                                                    </Link>
                                                </div>
                                        }
                                    </>

                                    :
                                <>
                                    {
                                    dataContext.profileUrl === "/Reviews" ? <div className='col-2 col-md-2 col-lg-2 col-xl-2 nav-col text-center active-nav-col'>
                                        <Link className="active-nav-link" style={{ textDecoration: `none` }} tag={Link} to={`/g/${props.userId}`} color="primary"  >
                                                    <FaRegCommentAlt fontSize="large" /> <span className='btn-title title-nav'>Reviews</span>
                                        </Link>
                                    </div> : <div className='col-2 col-md-2 col-lg-2 col-xl-2 nav-col text-center'>
                                            <Link style={{ textDecoration: `none` }} tag={Link} to={`/g/${props.userId}`} color="primary"  >
                                                        <FaRegCommentAlt fontSize="large" /> <span className='btn-title title-nav'>Reviews</span>
                                            </Link>
                                        </div>
                            }
                                   {
                                dataContext.profileUrl === "/Posts" ? <div className='col-2 col-md-2 col-lg-2 col-xl-2 nav-col text-center active-nav-col'>
                                    <Link className="active-nav-link" style={{ textDecoration: `none` }} tag={Link} to={`/g/${props.userId}/posts`} color="primary" >
                                                    <NotesIcon fontSize="small" /> <span className='btn-title title-nav'>Posts</span>
                                    </Link>
                                </div> : <div className='col-2 col-md-2 col-lg-2 col-xl-2 nav-col text-center'>
                                        <Link style={{ textDecoration: `none` }} tag={Link} to={`/g/${props.userId}/posts`} color="primary" >
                                                        <NotesIcon fontSize="small" /> <span className='btn-title title-nav'>Posts</span>
                                        </Link>
                                    </div>
                            }
                            {
                                dataContext.profileUrl === "/Galleries" ? <div className='col-2 col-md-2 col-lg-2 col-xl-2 nav-col text-center active-nav-col'>
                                    <Link className="active-nav-link" style={{ textDecoration: `none` }} tag={Link} to={`/g/${props.userId}/galleries`} color="primary"  >
                                                    <AppsIcon fontSize="small" /> <span className='btn-title title-nav'>Galleries</span>
                                    </Link>
                                </div> : <div className='col-2 col-md-2 col-lg-2 col-xl-2 nav-col text-center'>
                                        <Link style={{ textDecoration: `none` }} tag={Link} to={`/g/${props.userId}/galleries`} color="primary"  >
                                                        <AppsIcon fontSize="small" /> <span className='btn-title title-nav'>Galleries</span>
                                        </Link>
                                    </div>
                                        }
                                        </>
                                   
                            }
                        </div>
                    </div>
                  
            </div>
            </div>
        </div>
    );
}

export default function UpperSection(props)
{

    const content = <RealUpperSection visitor={props.visitor} rating={props.values.rating} userId={props.values.userId} profileImage={props.values.profileImage} name={props.values.username} location={props.values.location} description={props.values.description} />;

        return (
            <>
                {props.values.loading ? <UpperSectionSkeleton />
                    : content}
            </>
        );

}

