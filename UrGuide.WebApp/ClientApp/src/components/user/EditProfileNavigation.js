import React from "react";
import { Link } from 'react-router-dom';
import "./UserStyle.css";
import PaymentIcon from '@material-ui/icons/Payment';
import VisibilityIcon from '@material-ui/icons/Visibility';
import PersonIcon from '@material-ui/icons/Person';
import HighlightOffIcon from '@material-ui/icons/HighlightOff';


export default function EditProfileNavigation() {

    return (
        <div className="container-fluid edit-panel-card" >
            <div className='row justify-content-center'>
                <div className='col-11'>
                    <h5 className='text-muted'>Account Settings</h5>
                    <br />
                </div>
                <div className='col-11 edit-panel-link'>
                    <Link to='/user/edit/profile' style={{ textDecoration: `none` }} tag={Link} className="text-dark" ><PersonIcon fontSize="small" /> <span className='btn-title'>Personal Information</span></Link>
                </div>
                <div className='col-11 edit-panel-link'>
                    <Link to='/user/edit/password' style={{ textDecoration: `none` }} tag={Link} className="text-dark" ><VisibilityIcon fontSize="small" /> <span className='btn-title'>Change Password</span></Link>
                </div>
                <div className='col-11 edit-panel-link'>
                    <Link to='/Home' style={{ textDecoration: `none` }} tag={Link} className="text-dark" ><PaymentIcon fontSize="small" /> <span className='btn-title'>Credit Card Details</span></Link>
                </div>
                <div className='col-11 edit-panel-link'>
                    <Link to='/Home' style={{ textDecoration: `none` }} tag={Link} className="text-dark" ><HighlightOffIcon fontSize="small" /> <span className='btn-title'>Delete My Account</span></Link>
                </div>
            </div>
        </div>

    );

}
