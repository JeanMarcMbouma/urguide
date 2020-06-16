import React, { useState } from "react";
import { Button } from '@material-ui/core';
import Modal from 'react-bootstrap/Modal'
import { Link } from 'react-router-dom';
import "./UserStyle.css";
import PaymentIcon from '@material-ui/icons/Payment';
import VisibilityIcon from '@material-ui/icons/Visibility';
import PersonIcon from '@material-ui/icons/Person';
import HighlightOffIcon from '@material-ui/icons/HighlightOff';
import { HttpClientFactory } from "../../httpclient";
import { AccountClient, Client } from '../../api';
import LogoutCallback from "../api-authorization/LogoutCallback";
import { useAuthContext } from "../api-authorization/AuthService";

function Example() {
    const [show, setShow] = useState(false);

    const handleClose = () => setShow(false);
    const handleShow = () => setShow(true);
    const { manager, user} = useAuthContext();

    const deleteAccount = async () => {
        const api = HttpClientFactory.get(AccountClient, user);
        await api.delete(window.location.origin);
        await manager.signOut();
    }

    return (
        <>
            <Button style={{ textDecoration: `none` }} className="text-dark p-0" onClick={handleShow}><HighlightOffIcon fontSize="small" /> <span className='btn-title'>Delete My Account</span></Button>

            <Modal show={show} onHide={handleClose} centered>
                <Modal.Header closeButton>
                    <Modal.Title error>Attention</Modal.Title>
                </Modal.Header>
                <Modal.Body>Do you really want to delete your account?</Modal.Body>
                <Modal.Footer >
                    <div className="align-items-center">
                        <Button color='secondary' variant='outlined' onClick={deleteAccount}>
                            Yes
                        </Button>
                        <Button color="primary" variant='outlined' onClick={handleClose}>
                            No
                        </Button>
                    </div>
                </Modal.Footer>
            </Modal>
        </>
    );
}

export default function EditProfileNavigation(props) {

    return (
        <div className="container-fluid edit-panel-card" >
            <div className='row justify-content-center'>
                <div className='col-11'>
                    <h5 className='text-muted'>Account Settings</h5>
                    <br />
                </div>
                {props.isGuide ? <>
                    <div className='col-11 edit-panel-link'>
                        <Link to='/profile/details' style={{ textDecoration: `none` }} tag={Link} className="text-dark" ><PersonIcon fontSize="small" /> <span className='btn-title'>Personal Information</span></Link>
                    </div>
                    <div className='col-11 edit-panel-link'>
                        <Link to='/profile/password' style={{ textDecoration: `none` }} tag={Link} className="text-dark" ><VisibilityIcon fontSize="small" /> <span className='btn-title'>Change Password</span></Link>
                    </div>
                    <div className='col-11 edit-panel-link'>
                        <Link to='/' style={{ textDecoration: `none` }} tag={Link} className="text-dark" ><PaymentIcon fontSize="small" /> <span className='btn-title'>Credit Card Details</span></Link>
                    </div>
                    <div className='col-11 edit-panel-link'> 
                        <Example />
                    </div>
                </>
                      :

                    <>
                        <div className='col-11 edit-panel-link'>
                            <Link to='/account/details' style={{ textDecoration: `none` }} tag={Link} className="text-dark" ><PersonIcon fontSize="small" /> <span className='btn-title'>Personal Information</span></Link>
                        </div>
                        <div className='col-11 edit-panel-link'>
                            <Link to='/account/password' style={{ textDecoration: `none` }} tag={Link} className="text-dark" ><VisibilityIcon fontSize="small" /> <span className='btn-title'>Change Password</span></Link>
                        </div>
                        <div className='col-11 edit-panel-link'>
                            <Link to='/' style={{ textDecoration: `none` }} tag={Link} className="text-dark" ><PaymentIcon fontSize="small" /> <span className='btn-title'>Credit Card Details</span></Link>
                        </div>
                        <div className='col-11 edit-panel-link'>
                            <Example />
                        </div>
                    </>
                     
                    }
              
            </div>
        </div>

    );

}

