import React from "react";
import {
    makeStyles,
    IconButton,
    Button
} from "@material-ui/core";
import "./UserStyle.css";
import MailOutlineIcon from '@material-ui/icons/MailOutline';
import LocationOnIcon from '@material-ui/icons/LocationOn';
import MoreHorizIcon from '@material-ui/icons/MoreHoriz';



const buttonStyles = makeStyles(theme => ({
    root: {
        '& > *': {
            margin: theme.spacing(1),
        },
        title: {
            marginLeft:'10px',
            fontSize:'14px',
        },
    },
}));

function AboutCard() {

    const classes = buttonStyles();

    return (
        <div className="container-fluid about-card">
            <div>
                <h4>Allyson w.</h4>
                <span className="text-muted">Los angeles, USA</span>
                <br />
                <br />
                <p>
                    Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged.
                </p>
                <div className={classes.root} >
                    <Button variant="contained"  color="secondary"><MailOutlineIcon fontSize="small" />   <span className='btn-title'>Message</span></Button>
                </div>
            </div>
        </div>
    );

}


function GalleryCard() {

    return (
        <div className="container-fluid gallery-card">
            <div className='row justify-content-between'>
                <div className='col-3 col-md-3'>
                    <div>
                        <LocationOnIcon fontSize="large" />
                        <div className='location' >
                            <span>Venice beach, CA</span>
                        </div>
                        <div className='date' >
                            <span>Added on 11/10/2019</span>
                        </div>
                    </div>
                </div>
                <div className='col-3 col-md-2 col-lg-1'>
                    <IconButton aria-label="more" >
                        <MoreHorizIcon className='more-btn'  />
                    </IconButton>
                </div>
            </div>
            <br />
            <div className='row justify-content-center'>
                <div className='col-12 col-sm-6 col-md-4 col-xl-3 photo-div'>
                    <div className='photo'></div>
                </div>
                <div className='col-12 col-sm-6 col-md-4 col-xl-3 photo-div'>
                    <div className='photo'></div>
                </div>
                <div className='col-12 col-sm-6 col-md-4 col-xl-3 photo-div'>
                    <div className='photo'></div>
                </div>
                <div className='col-12 col-sm-6 col-md-4 col-xl-3 photo-div'>
                    <div className='photo'></div>
                </div>
                <div className='col-12 col-sm-6 col-md-4 col-xl-3 photo-div'>
                    <div className='photo'></div>
                </div>
                <div className='col-12 col-sm-6 col-md-4 col-xl-3 photo-div'>
                    <div className='photo'></div>
                </div>
                <div className='col-12 col-sm-6 col-md-4 col-xl-3 photo-div'>
                    <div className='photo'></div>
                </div>
                <div className='col-12 col-sm-6 col-md-4 col-xl-3 photo-div'>
                    <div className='photo'></div>
                </div>

            </div>
        </div>
    );
           
}


export function LowerSection() {


    return (
        <div>
            <div className="row">
                <div className="col-12 col-lg-4 about">
                    <AboutCard />
                </div>
                <div className="col-12 col-lg-8">
                    <GalleryCard />
                    <GalleryCard />
                    <GalleryCard />
                </div>
            </div>
        </div>
    );
}