import React, { Component, useState } from "react";
import {
    makeStyles,
    IconButton,
    Button
} from "@material-ui/core";
import LocationOnIcon from '@material-ui/icons/LocationOn';
import MoreHorizIcon from '@material-ui/icons/MoreHoriz';
import { Link } from 'react-router-dom';
import "./UserStyle.css";


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


function GalleryCard() {

    return (
        <div className="container-fluid gallery-card">
            <div className='row justify-content-between'>
                <div className='col-3 col-md-4'>
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
            <div className='row justify-content-start'>
                <div className='col-12 col-sm-6 col-md-4 col-xl-4 photo-div'>
                    <div className='photo'></div>
                </div>
                <div className='col-12 col-sm-6 col-md-4 col-xl-4 photo-div'>
                    <div className='photo'></div>
                </div>
                <div className='col-12 col-sm-6 col-md-4 col-xl-4 photo-div'>
                    <div className='photo'></div>
                </div>
                <div className='col-12 col-sm-6 col-md-4 col-xl-4 photo-div'>
                    <div className='photo'></div>
                </div>
                <div className='col-12 col-sm-6 col-md-4 col-xl-4 photo-div'>
                    <div className='photo'></div>
                </div>
                <div className='col-12 col-sm-6 col-md-4 col-xl-4 photo-div'>
                    <div className='photo'></div>
                </div>
                <div className='col-12 col-sm-6 col-md-4 col-xl-4 photo-div'>
                    <div className='photo'></div>
                </div>
                <div className='col-12 col-sm-6 col-md-4 col-xl-4 photo-div'>
                    <div className='photo'></div>
                </div>

            </div>
        </div>
    );
           
}


function Layout() {

    return (<div className='row justify-content-center'>
        <div className="col-12">
            <GalleryCard />
            <GalleryCard />
            <GalleryCard />
        </div>
        </div>);
}


export default class Galleries extends Component {
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