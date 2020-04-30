import React, { Component, useState } from "react";
import {
    makeStyles,
    IconButton,
    Button
} from "@material-ui/core";
import LocationOnIcon from '@material-ui/icons/LocationOn';
import MoreHorizIcon from '@material-ui/icons/MoreHoriz';
import Modal from 'react-bootstrap/Modal';
import { Link } from 'react-router-dom';
import { UpperSection } from "./UpperSection";
import Post from "../post/Post";
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

function ItemModal() {
    const [show, setShow] = useState(false);

    return (
        <>
            <Button variant="primary" onClick={() => setShow(true)}>
                Custom Width Modal
      </Button>

            <Modal
                animation={false}
                show={show}
                onHide={() => setShow(false)}
            
                dialogClassName="my-modal-dialog"
                aria-labelledby="example-custom-modal-styling-title"
            >
                <Modal.Header closeButton>
                </Modal.Header>
                <Modal.Body>
                    <Post/>
                </Modal.Body>
            </Modal>
        </>
    );
}


function FollowingCard() {

    
    return (
        <div className="container-fluid following-card">
            <div>
                <h6 className='text-muted'>Following (1200)</h6>
                <br/>
            </div>
            <div className='row'>
               
                <div className='col-4 follower-div'>
                    <div className='follower-div-photo'>
                    </div>
                </div>
                <div className='col-4 follower-div'>
                    <div className='follower-div-photo'>
                    </div>
                </div>
                <div className='col-4 follower-div'>
                    <div className='follower-div-photo'>
                    </div>
                </div>
                <div className='col-4 follower-div'>
                    <div className='follower-div-photo'>
                    </div>
                </div>
                <div className='col-4 follower-div'>
                    <div className='follower-div-photo'>
                    </div>
                </div>
                <div className='col-4 follower-div'>
                    <div className='follower-div-photo'>
                    </div>
                </div>
                <div className='col-4 follower-div'>
                    <div className='follower-div-photo'>
                    </div>
                </div>
                <div className='col-4 follower-div'>
                    <div className='follower-div-photo'>
                    </div>
                </div>
                <div className='col-4 follower-div'>
                    <div className='follower-div-photo'>
                    </div>
                </div>
            </div>
        </div>
    );

}


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
        <div className="col-12 col-lg-8">
            <GalleryCard />
            <GalleryCard />
            <GalleryCard />
        </div>
        </div>);
}


export default class Galleries extends Component {
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
                <ItemModal />
            </div>
        )
    }
}